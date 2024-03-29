﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using static MacrosAPI_v3.MacrosLoader;

namespace MacrosAPI_v3.Plugins
{
    public class WebScript : Macros
    {
        private string url = "";
        private string[] lines = new string[0];
        private string[] args = new string[0];
        private bool csharp;
        private Thread thread;
        private Dictionary<string, object> localVars = new Dictionary<string, object>();

        public WebScript(string url)
        {
            this.url = url;
        }

        public override void Initialize()
        {
            using (System.Net.WebClient wc = new System.Net.WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                string res = wc.DownloadString(url);

                lines = res.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            };
            csharp = true;
            thread = null;

        }
        public override void Update()
        {
            if (csharp) //C# compiled script
            {
                //Initialize thread on first update
                if (thread == null && lines.Length != 0)
                {
                    thread = new Thread(() =>
                    {
                        Run(this, lines, args, localVars);
                    });
                    thread.Name = "MCC Script - " + url;
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
