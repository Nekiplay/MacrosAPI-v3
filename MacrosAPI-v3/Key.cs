using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MacrosAPI_v3
{
    public enum Key : ushort
    {
        None = 0x00,

        Esc = 0x001,

        F1 = 0x03B,
        F2 = 0x03C,
        F3 = 0x03D,
        F4 = 0x03E,
        F5 = 0x03F,
        F6 = 0x040,
        F7 = 0x041,
        F8 = 0x042,
        F9 = 0x043,
        F10 = 0x044,
        F11 = 0x057,
        F12 = 0x058,

        Tilde = 0x029,
        D1 = 0x002,
        D2 = 0x003,
        D3 = 0x004,
        D4 = 0x005,
        D5 = 0x006,
        D6 = 0x007,
        D7 = 0x008,
        D8 = 0x009,
        D9 = 0x00A,
        D0 = 0x00B,
        Minus = 0x00C,
        Plus = 0x00D,
        Backspace = 0x00E,

        Tab = 0x00F,
        Q = 0x010,
        W = 0x011,
        E = 0x012,
        R = 0x013,
        T = 0x014,
        Y = 0x015,
        U = 0x016,
        I = 0x017,
        O = 0x018,
        P = 0x019,
        OpenBrack = 0x01A,
        CloseBrack = 0x01B,
        Enter = 0x01C,

        CapsLock = 0x03A,
        A = 0x01E,
        S = 0x01F,
        D = 0x020,
        F = 0x021,
        G = 0x022,
        H = 0x023,
        J = 0x024,
        K = 0x025,
        L = 0x026,
        Semicolon = 0x027,
        Quote = 0x028,
        Backslash = 0x02B,

        LShift = 0x02A,
        Z = 0x02C,
        X = 0x02D,
        C = 0x02E,
        V = 0x02F,
        B = 0x030,
        N = 0x031,
        M = 0x032,
        Comma = 0x033,
        Period = 0x034,
        Slash = 0x035,
        RShift = 0x036,

        LControl = 0x01D,
        LWin = 0x15B,
        LAlt = 0x038,
        Space = 0x039,
        RAlt = 0x138,
        RWin = 0x15C,
        ContextMenu = 0x15D,
        RControl = 0x11D,

        //PrintScreen = , // Длинная последовательность сканкодов
        ScrollLock = 0x046,
        //Pause = , // Блокирует консоль намертво. Ctrl+Z не сможет ее снять с паузы, так как ввод больше не обрабатывается

        Insert = 0x152,
        Home = 0x147,
        PageUp = 0x149,

        Delete = 0x153,
        End = 0x14F,
        PageDown = 0x151,

        Up = 0x148,
        Left = 0x14B,
        Down = 0x150,
        Right = 0x14D,

        NumLock = 0x045,
        NumDiv = 0x135,
        NumMul = 0x037,
        NumMin = 0x04A,

        Num7 = 0x047,
        Num8 = 0x048,
        Num9 = 0x049,
        NumAdd = 0x04E,

        Num4 = 0x04B,
        Num5 = 0x04C,
        Num6 = 0x04D,

        Num1 = 0x04F,
        Num2 = 0x050,
        Num3 = 0x051,
        NumEnter = 0x11C,

        Num0 = 0x052,
        NumDel = 0x053,
    }
}
