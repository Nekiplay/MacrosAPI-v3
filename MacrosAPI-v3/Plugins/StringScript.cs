using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static MacrosAPI_v3.MacrosLoader;

namespace MacrosAPI_v3.Plugins
{
    public class StringScript : Macros
    {
        private string file = "";
        private List<string> lines = new List<string>();
        private string[] args = new string[0];
        private bool csharp;
        private Thread thread;
        private Dictionary<string, object> localVars = new Dictionary<string, object>();
        public StringScript(string[] code)
        {
            lines.AddRange(code);
        }

        public StringScript(List<string> code)
        {
            lines = code;
        }

        public StringScript(string code)
        {
            lines = code.Split('\n').ToList();
        }

        public override void Initialize()
        {
            csharp = true;
            thread = null;
        }
        public override void Update()
        {
            //Initialize thread on first update
            if (thread == null)
            {
                thread = new Thread(() =>
                {
                    Run(this, lines.ToArray(), args, localVars);
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
