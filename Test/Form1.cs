using MacrosAPI_v3;
using MacrosAPI_v3.Plugins;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        
        public static MacrosUpdater macrosUpdater = new MacrosUpdater();
        public static MacrosManager macrosManager = new MacrosManager(macrosUpdater);

        private void Form1_Load(object sender, EventArgs e)
        {
            macrosManager.LoadMacros(new FileScript(new FileInfo("Donate Events.cs")));

            macrosManager.OnMacrosLoad += (m) => 
            {
                Console.WriteLine("Макрос " + m.ToString().Split('+').LastOrDefault() + " загружен");;
            };

        }
    }
}
