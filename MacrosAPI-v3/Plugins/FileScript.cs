using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using static MacrosAPI_v3.MacrosLoader;

namespace MacrosAPI_v3
{
    public class FileScript : Macros
    {
        private string file = "";
        private string[] lines = new string[0];
        private string[] args = new string[0];
        private bool csharp;
        private Thread thread;
        private Dictionary<string, object> localVars = new Dictionary<string, object>();

        public FileScript(FileInfo filename)
        {
            file = filename.FullName;
        }


        public override void Initialize()
        {
            lines = System.IO.File.ReadAllLines(file, Encoding.UTF8);
            csharp = file.EndsWith(".cs");
            thread = null;
        }
        public override void Update()
        {
            if (csharp) //C# compiled script
            {
                //Initialize thread on first update
                if (thread == null)
                {
                    thread = new Thread(() =>
                    {
                        Run(this, lines, args, localVars);
                    });
                    thread.Name = "MCC Script - " + file;
                    thread.Start();
                }

                //Unload bot once the thread has finished running
                if (thread != null && !thread.IsAlive)
                {
                    UnLoadMacros();
                }
            }
        }
    }
}
