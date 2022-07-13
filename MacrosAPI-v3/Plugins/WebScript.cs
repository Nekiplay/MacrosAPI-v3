using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
using static MacrosAPI_v3.MacrosLoader;

namespace MacrosAPI_v3.Plugins
{
    public class WebScript : Macros
    {
        private string file = "";
        private string[] lines = new string[0];
        private string[] args = new string[0];
        private bool csharp;
        private Thread thread;
        private Dictionary<string, object> localVars = new Dictionary<string, object>();

        public WebScript(string url)
        {
            this.file = url;
        }

        public static bool LookForScript(ref string filename)
        {
            //Automatically look in subfolders and try to add ".txt" file extension
            char dir_slash = MacrosManager.isUsingMono ? '/' : '\\';
            string[] files = new string[]
            {
                filename
            };

            foreach (string possible_file in files)
            {
                if (System.IO.File.Exists(possible_file))
                {
                    filename = possible_file;
                    return true;
                }
            }

            string caller = "Script";
            try
            {
                StackFrame frame = new StackFrame(1);
                MethodBase method = frame.GetMethod();
                Type type = method.DeclaringType;
                caller = type.Name;
            }
            catch { }

            return false;
        }

        public override void Initialize()
        {
            using (System.Net.WebClient wc = new System.Net.WebClient())
            {
                wc.Encoding = Encoding.Default;
                string res = wc.DownloadString(file);

                lines = res.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            };
            csharp = true;
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
