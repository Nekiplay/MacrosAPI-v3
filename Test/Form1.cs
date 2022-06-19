using MacrosAPI_v3;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private static MacrosUpdater macrosUpdater = new MacrosUpdater();
        private static MacrosManager macrosManager = new MacrosManager(macrosUpdater);
        private void Form1_Load(object sender, EventArgs e)
        {
            macrosManager.LoadMacros(new Test1());
        }

        public class Test1 : Macros
        {
            public override bool OnKeyDown(Key key, bool repeat)
            {
                if (key == Key.R)
                {
                    Console.WriteLine("Keyboard layout " + 
                        KeyBoardLayoutToString(GetKeyboardLayout())
                        );
                    SetKeyboardLayout(1033, KeyboardLayoutFlags.KLF_SETFORPROCESS);
                    //KeyDown(Key.G, Key.B, Key.L, Key.J, Key.H);
                    //KeyUp(Key.G, Key.B, Key.L, Key.J, Key.H);
                    //
                    //KeyDown(Key.Enter);
                    //KeyUp(Key.Enter);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            // 1049 Russian
            public string KeyBoardLayoutToString(ushort layout)
            {
                if (layout == 1049)
                {
                    return "Russian";
                }
                else if (layout == 1033)
                {
                    return "English";
                }
                else
                {
                    return "Unknown";
                }
            }
        }
    }
}
