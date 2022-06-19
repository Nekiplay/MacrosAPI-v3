using System;
using System.IO;
using System.Runtime.InteropServices;


//typedef void *InterceptionContext;
using InterceptionContext = System.IntPtr;

//typedef int InterceptionDevice;
using InterceptionDevice = System.Int32;

//typedef int InterceptionPrecedence;


public static class Interception
{
    //#define INTERCEPTION_MAX_KEYBOARD 10
    public const int MaxKeyboard = 10;

    //#define INTERCEPTION_MAX_MOUSE 10
    public const int MaxMouse = 10;

    //#define INTERCEPTION_MAX_DEVICE ((INTERCEPTION_MAX_KEYBOARD) + (INTERCEPTION_MAX_MOUSE))
    public const int MaxDevice = MaxKeyboard + MaxMouse;

    //#define INTERCEPTION_KEYBOARD(index) ((index) + 1)
    public static int Keyboard(int index)
    {
        return index + 1;
    }

    //#define INTERCEPTION_MOUSE(index) ((INTERCEPTION_MAX_KEYBOARD) + (index) + 1)
    public static int Mouse(int index)
    {
        return MaxKeyboard + index + 1;
    }

    //typedef unsigned short InterceptionFilter;
    [Flags]
    public enum Filter : ushort
    {
        None = FilterKeyState.None, // = FilterMouseState.None
        All = FilterKeyState.All, // = FilterMouseState.All

        KDown = FilterKeyState.Down,
        KUp = FilterKeyState.Up,
        KE0 = FilterKeyState.E0,
        KE1 = FilterKeyState.E1,
        KTermSrvSetLED = FilterKeyState.TermSrvSetLED,
        KTermSrvShadow = FilterKeyState.TermSrvShadow,
        KTermSrvVKPacket = FilterKeyState.TermSrvVKPacket,

        MLeftButtonDown = FilterMouseState.LeftButtonDown,
        MLeftButtonUp = FilterMouseState.LeftButtonUp,
        MRightButtonDown = FilterMouseState.RightButtonDown,
        MRightButtonUp = FilterMouseState.RightButtonUp,
        MMiddleButtonDown = FilterMouseState.MiddleButtonDown,
        MMiddleButtonUp = FilterMouseState.MiddleButtonUp,

        MButton1Down = FilterMouseState.Button1Down,
        MButton1Up = FilterMouseState.Button1Up,
        MButton2Down = FilterMouseState.Button2Down,
        MButton2Up = FilterMouseState.Button2Up,
        MButton3Down = FilterMouseState.Button3Down,
        MButton3Up = FilterMouseState.Button3Up,

        MButton4Down = FilterMouseState.Button4Down,
        MButton4Up = FilterMouseState.Button4Up,
        MButton5Down = FilterMouseState.Button5Down,
        MButton5Up = FilterMouseState.Button5Up,

        MWheel = FilterMouseState.Wheel,
        MHWheel = FilterMouseState.HWheel,

        MMove = FilterMouseState.Move
    }

    //typedef int (*InterceptionPredicate)(InterceptionDevice device);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int Predicate(int device);

    /*
    enum InterceptionKeyState
    {
        INTERCEPTION_KEY_DOWN             = 0x00,
        INTERCEPTION_KEY_UP               = 0x01,
        INTERCEPTION_KEY_E0               = 0x02,
        INTERCEPTION_KEY_E1               = 0x04,
        INTERCEPTION_KEY_TERMSRV_SET_LED  = 0x08,
        INTERCEPTION_KEY_TERMSRV_SHADOW   = 0x10,
        INTERCEPTION_KEY_TERMSRV_VKPACKET = 0x20
    };
    */
    [Flags]
    public enum KeyState : ushort
    {
        //Down = 0x00,
        Up = 0x01,
        E0 = 0x02, // https://docs.microsoft.com/en-us/windows/desktop/api/ntddkbd/ns-ntddkbd-keyboard_input_data
        E1 = 0x04,
        TermSrvSetLED = 0x08, // https://docs.microsoft.com/en-us/windows/desktop/api/ntddkbd/ns-ntddkbd-keyboard_indicator_parameters
        TermSrvShadow = 0x10,
        TermSrvVKPacket = 0x20
    }

    public static bool IsKeyDown(this KeyState keyState)
    {
        return (keyState & KeyState.Up) == 0;
    }

    /*
    enum InterceptionFilterKeyState
    {
        INTERCEPTION_FILTER_KEY_NONE             = 0x0000,
        INTERCEPTION_FILTER_KEY_ALL              = 0xFFFF,
        INTERCEPTION_FILTER_KEY_DOWN             = INTERCEPTION_KEY_UP,
        INTERCEPTION_FILTER_KEY_UP               = INTERCEPTION_KEY_UP << 1,
        INTERCEPTION_FILTER_KEY_E0               = INTERCEPTION_KEY_E0 << 1,
        INTERCEPTION_FILTER_KEY_E1               = INTERCEPTION_KEY_E1 << 1,
        INTERCEPTION_FILTER_KEY_TERMSRV_SET_LED  = INTERCEPTION_KEY_TERMSRV_SET_LED << 1,
        INTERCEPTION_FILTER_KEY_TERMSRV_SHADOW   = INTERCEPTION_KEY_TERMSRV_SHADOW << 1,
        INTERCEPTION_FILTER_KEY_TERMSRV_VKPACKET = INTERCEPTION_KEY_TERMSRV_VKPACKET << 1
    };
    */
    [Flags]
    public enum FilterKeyState : ushort
    {
        None = 0x0000,
        All = 0xFFFF,
        Down = KeyState.Up,
        Up = KeyState.Up << 1,
        E0 = KeyState.E0 << 1,
        E1 = KeyState.E1 << 1,
        TermSrvSetLED = KeyState.TermSrvSetLED << 1,
        TermSrvShadow = KeyState.TermSrvShadow << 1,
        TermSrvVKPacket = KeyState.TermSrvVKPacket << 1
    }

    /*
    enum InterceptionMouseState
    {
        INTERCEPTION_MOUSE_LEFT_BUTTON_DOWN   = 0x001,
        INTERCEPTION_MOUSE_LEFT_BUTTON_UP     = 0x002,
        INTERCEPTION_MOUSE_RIGHT_BUTTON_DOWN  = 0x004,
        INTERCEPTION_MOUSE_RIGHT_BUTTON_UP    = 0x008,
        INTERCEPTION_MOUSE_MIDDLE_BUTTON_DOWN = 0x010,
        INTERCEPTION_MOUSE_MIDDLE_BUTTON_UP   = 0x020,
        INTERCEPTION_MOUSE_BUTTON_1_DOWN      = INTERCEPTION_MOUSE_LEFT_BUTTON_DOWN,
        INTERCEPTION_MOUSE_BUTTON_1_UP        = INTERCEPTION_MOUSE_LEFT_BUTTON_UP,
        INTERCEPTION_MOUSE_BUTTON_2_DOWN      = INTERCEPTION_MOUSE_RIGHT_BUTTON_DOWN,
        INTERCEPTION_MOUSE_BUTTON_2_UP        = INTERCEPTION_MOUSE_RIGHT_BUTTON_UP,
        INTERCEPTION_MOUSE_BUTTON_3_DOWN      = INTERCEPTION_MOUSE_MIDDLE_BUTTON_DOWN,
        INTERCEPTION_MOUSE_BUTTON_3_UP        = INTERCEPTION_MOUSE_MIDDLE_BUTTON_UP,
        INTERCEPTION_MOUSE_BUTTON_4_DOWN      = 0x040,
        INTERCEPTION_MOUSE_BUTTON_4_UP        = 0x080,
        INTERCEPTION_MOUSE_BUTTON_5_DOWN      = 0x100,
        INTERCEPTION_MOUSE_BUTTON_5_UP        = 0x200,
        INTERCEPTION_MOUSE_WHEEL              = 0x400,
        INTERCEPTION_MOUSE_HWHEEL             = 0x800
    };
    */
    [Flags]
    public enum MouseState : ushort
    {
        LeftButtonDown = 0x001,
        LeftButtonUp = 0x002,
        RightButtonDown = 0x004,
        RightButtonUp = 0x008,
        MiddleButtonDown = 0x010,
        MiddleButtonUp = 0x020,

        Button1Down = LeftButtonDown,
        Button1Up = LeftButtonUp,
        Button2Down = RightButtonDown,
        Button2Up = RightButtonUp,
        Button3Down = MiddleButtonDown,
        Button3Up = MiddleButtonUp,

        Button4Down = 0x040,
        Button4Up = 0x080,
        Button5Down = 0x100,
        Button5Up = 0x200,

        Wheel = 0x400,
        HWheel = 0x800
    }

    /*
    enum InterceptionFilterMouseState
    {
        INTERCEPTION_FILTER_MOUSE_NONE               = 0x0000,
        INTERCEPTION_FILTER_MOUSE_ALL                = 0xFFFF,
        INTERCEPTION_FILTER_MOUSE_LEFT_BUTTON_DOWN   = INTERCEPTION_MOUSE_LEFT_BUTTON_DOWN,
        INTERCEPTION_FILTER_MOUSE_LEFT_BUTTON_UP     = INTERCEPTION_MOUSE_LEFT_BUTTON_UP,
        INTERCEPTION_FILTER_MOUSE_RIGHT_BUTTON_DOWN  = INTERCEPTION_MOUSE_RIGHT_BUTTON_DOWN,
        INTERCEPTION_FILTER_MOUSE_RIGHT_BUTTON_UP    = INTERCEPTION_MOUSE_RIGHT_BUTTON_UP,
        INTERCEPTION_FILTER_MOUSE_MIDDLE_BUTTON_DOWN = INTERCEPTION_MOUSE_MIDDLE_BUTTON_DOWN,
        INTERCEPTION_FILTER_MOUSE_MIDDLE_BUTTON_UP   = INTERCEPTION_MOUSE_MIDDLE_BUTTON_UP,
        INTERCEPTION_FILTER_MOUSE_BUTTON_1_DOWN      = INTERCEPTION_MOUSE_BUTTON_1_DOWN,
        INTERCEPTION_FILTER_MOUSE_BUTTON_1_UP        = INTERCEPTION_MOUSE_BUTTON_1_UP,
        INTERCEPTION_FILTER_MOUSE_BUTTON_2_DOWN      = INTERCEPTION_MOUSE_BUTTON_2_DOWN,
        INTERCEPTION_FILTER_MOUSE_BUTTON_2_UP        = INTERCEPTION_MOUSE_BUTTON_2_UP,
        INTERCEPTION_FILTER_MOUSE_BUTTON_3_DOWN      = INTERCEPTION_MOUSE_BUTTON_3_DOWN,
        INTERCEPTION_FILTER_MOUSE_BUTTON_3_UP        = INTERCEPTION_MOUSE_BUTTON_3_UP,
        INTERCEPTION_FILTER_MOUSE_BUTTON_4_DOWN      = INTERCEPTION_MOUSE_BUTTON_4_DOWN,
        INTERCEPTION_FILTER_MOUSE_BUTTON_4_UP        = INTERCEPTION_MOUSE_BUTTON_4_UP,
        INTERCEPTION_FILTER_MOUSE_BUTTON_5_DOWN      = INTERCEPTION_MOUSE_BUTTON_5_DOWN,
        INTERCEPTION_FILTER_MOUSE_BUTTON_5_UP        = INTERCEPTION_MOUSE_BUTTON_5_UP,
        INTERCEPTION_FILTER_MOUSE_WHEEL              = INTERCEPTION_MOUSE_WHEEL,
        INTERCEPTION_FILTER_MOUSE_HWHEEL             = INTERCEPTION_MOUSE_HWHEEL,
        INTERCEPTION_FILTER_MOUSE_MOVE               = 0x1000
    };
    */
    [Flags]
    public enum FilterMouseState : ushort
    {
        None = 0x0000,
        All = 0xFFFF,

        LeftButtonDown = MouseState.LeftButtonDown,
        LeftButtonUp = MouseState.LeftButtonUp,
        RightButtonDown = MouseState.RightButtonDown,
        RightButtonUp = MouseState.RightButtonUp,
        MiddleButtonDown = MouseState.MiddleButtonDown,
        MiddleButtonUp = MouseState.MiddleButtonUp,

        Button1Down = MouseState.Button1Down,
        Button1Up = MouseState.Button1Up,
        Button2Down = MouseState.Button2Down,
        Button2Up = MouseState.Button2Up,
        Button3Down = MouseState.Button3Down,
        Button3Up = MouseState.Button3Up,

        Button4Down = MouseState.Button4Down,
        Button4Up = MouseState.Button4Up,
        Button5Down = MouseState.Button5Down,
        Button5Up = MouseState.Button5Up,

        Wheel = MouseState.Wheel,
        HWheel = MouseState.HWheel,

        Move = 0x1000
    }

    /*
    enum InterceptionMouseFlag
    {
        INTERCEPTION_MOUSE_MOVE_RELATIVE      = 0x000,
        INTERCEPTION_MOUSE_MOVE_ABSOLUTE      = 0x001,
        INTERCEPTION_MOUSE_VIRTUAL_DESKTOP    = 0x002,
        INTERCEPTION_MOUSE_ATTRIBUTES_CHANGED = 0x004,
        INTERCEPTION_MOUSE_MOVE_NOCOALESCE    = 0x008,
        INTERCEPTION_MOUSE_TERMSRV_SRC_SHADOW = 0x100
    };
    */
    [Flags]
    public enum MouseFlag : ushort
    {
        MoveRelative = 0x000,
        MoveAbsolute = 0x001,
        VirturalDesktop = 0x002,
        AttributesChanged = 0x004,
        MoveNoCoalesce = 0x008,
        TermSrvSrcShadow = 0x100
    }

    /*
    typedef struct
    {
        unsigned short state;
        unsigned short flags;
        short rolling;
        int x;
        int y;
        unsigned int information;
    } InterceptionMouseStroke;
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct MouseStroke
    {
        public MouseState State;
        public MouseFlag Flags;
        public short Rolling;
        public int X;
        public int Y;
        public uint Information;
    }

    /*
    typedef struct
    {
        unsigned short code;
        unsigned short state;
        unsigned int information;
    } InterceptionKeyStroke;
    */
    [StructLayout(LayoutKind.Sequential)]
    public struct KeyStroke
    {
        public ushort Code;
        public KeyState State;
        public uint Information;
    }

    //typedef char InterceptionStroke[sizeof(InterceptionMouseStroke)];
    //TODO это массив
    [StructLayout(LayoutKind.Explicit)]
    public struct Stroke
    {
        [FieldOffset(0)]
        public MouseStroke Mouse;

        [FieldOffset(0)]
        public KeyStroke Key;
    }

    //InterceptionContext INTERCEPTION_API interception_create_context(void);
    [DllImport("interception.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "interception_create_context")]
    public static extern InterceptionContext CreateContext();

    //void INTERCEPTION_API interception_destroy_context(InterceptionContext context);
    [DllImport("interception.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "interception_destroy_context")]
    public static extern void DestroyContext(InterceptionContext context);

    //InterceptionPrecedence INTERCEPTION_API interception_get_precedence(InterceptionContext context, InterceptionDevice device);

    //void INTERCEPTION_API interception_set_precedence(InterceptionContext context, InterceptionDevice device, InterceptionPrecedence precedence);

    //InterceptionFilter INTERCEPTION_API interception_get_filter(InterceptionContext context, InterceptionDevice device);
    [DllImport("interception.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "interception_get_filter")]
    public static extern Filter GetFilter(InterceptionContext context, InterceptionDevice deviceID);

    //void INTERCEPTION_API interception_set_filter(InterceptionContext context, InterceptionPredicate predicate, InterceptionFilter filter);
    [DllImport("interception.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "interception_set_filter")]
    public static extern void SetFilter(InterceptionContext context, Predicate predicate, Filter filter);

    //InterceptionDevice INTERCEPTION_API interception_wait(InterceptionContext context);
    [DllImport("interception.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "interception_wait")]
    public static extern InterceptionDevice Wait(InterceptionContext context);

    //InterceptionDevice INTERCEPTION_API interception_wait_with_timeout(InterceptionContext context, unsigned long milliseconds);
    [DllImport("interception.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "interception_wait_with_timeout")]
    public static extern InterceptionDevice WaitWithTimeout(InterceptionContext context, ulong milliseconds);

    //int INTERCEPTION_API interception_send(InterceptionContext context, InterceptionDevice device, const InterceptionStroke *stroke, unsigned int nstroke);
    [DllImport("interception.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "interception_send")]
    public static extern int Send(InterceptionContext context, InterceptionDevice deviceID, ref Stroke stroke, uint nstroke);
    // TODO переделать на маршалинг массива

    //int INTERCEPTION_API interception_receive(InterceptionContext context, InterceptionDevice device, InterceptionStroke *stroke, unsigned int nstroke);
    [DllImport("interception.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "interception_receive")]
    public static extern int Receive(InterceptionContext context, InterceptionDevice deviceID, ref Stroke stroke, uint nstroke);
    // TODO переделать на маршалинг массива

    //unsigned int INTERCEPTION_API interception_get_hardware_id(InterceptionContext context, InterceptionDevice device, void *hardware_id_buffer, unsigned int buffer_size);

    //int INTERCEPTION_API interception_is_invalid(InterceptionDevice device);

    //int INTERCEPTION_API interception_is_keyboard(InterceptionDevice device);
    [DllImport("interception.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "interception_is_keyboard")]
    public static extern int IsKeyboard(InterceptionDevice deviceID);
    //int INTERCEPTION_API interception_is_mouse(InterceptionDevice device);
    [DllImport("interception.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "interception_is_mouse")]
    public static extern int IsMouse(InterceptionDevice deviceID);


    static Interception()
    {
        // https://stackoverflow.com/questions/10852634/using-a-32bit-or-64bit-dll-in-c-sharp-dllimport
        string myPath = new Uri(typeof(Interception).Assembly.CodeBase).LocalPath;
        string myFolder = Path.GetDirectoryName(myPath);

        bool is64 = (IntPtr.Size == 8);
        string subfolder = is64 ? @"\Lib64\" : @"\Lib32\";

        LoadLibrary(myFolder + subfolder + "interception.dll");
    }

    [DllImport("kernel32.dll")]
    private static extern IntPtr LoadLibrary(string lpLibFileName);
}