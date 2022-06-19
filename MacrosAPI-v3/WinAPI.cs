using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MacrosAPI_v3
{
    public enum KeyboardLayoutFlags : uint
    {
        KLF_ACTIVATE = 0x00000001,
        KLF_SETFORPROCESS = 0x00000100
    }
    public class WinAPI
    {
        [DllImport("user32.dll")]
        public static extern IntPtr GetKeyboardLayout(int idThread);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint ActivateKeyboardLayout(uint hkl, KeyboardLayoutFlags Flags);

        public static ushort GetKeyboardLayout()
        {
            Int32 o = 0;
            return (ushort)GetKeyboardLayout(GetWindowThreadProcessId(GetForegroundWindow(), ref o));
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern Int32 GetWindowThreadProcessId(IntPtr hwnd, ref Int32 pid);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll")]
        public static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left, Top, Right, Bottom;
        }
    }
}
