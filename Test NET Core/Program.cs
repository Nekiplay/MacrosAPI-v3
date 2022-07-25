using MacrosAPI_v3;
using System;

namespace Test_NET_Core // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MacrosUpdater macrosUpdater = new MacrosUpdater();
            MacrosManager macrosManager = new MacrosManager(macrosUpdater);
            macrosManager.LoadMacros(new Test());
        }

        public class Test : Macros
        {
            public override void Initialize()
            {
                Console.WriteLine("Работает");
            }
        }

    }
}
