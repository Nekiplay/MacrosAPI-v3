﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MacrosAPI_v3.Plugins
{
    public class Script : Macros
    {
        private string file = "";
        private string[] lines = new string[0];
        private string[] args = new string[0];
        private bool csharp;
        private Thread thread;
        private Dictionary<string, object> localVars = new Dictionary<string, object>();

        public Script(FileInfo filename)
        {
            file = filename.FullName;
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
            if (LookForScript(ref file))
            {
                lines = System.IO.File.ReadAllLines(file, Encoding.UTF8);
                csharp = file.EndsWith(".cs");
                thread = null;

            }
            else
            {
                UnLoadPlugin();
            }
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
                        MacrosLoader.Run(this, lines, args, localVars);
                    });
                    thread.Name = "MCC Script - " + file;
                    thread.Start();
                }

                //Unload bot once the thread has finished running
                if (thread != null && !thread.IsAlive)
                {
                    UnLoadPlugin();
                }
            }
        }
    }
}
