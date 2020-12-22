using System;
using System.Threading;
using Sers.Hardware.Env;
using Sers.Hardware.Usage;

namespace Sers.Hardware.Demo
{


    class Program
    {

        static void Main(string[] args)
        {

            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine("MachineUnqueInfo:");
            Console.WriteLine(EnvHelp.GetMachineUnqueInfo());

            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine("AppUnqueInfo:");
            Console.WriteLine(EnvHelp.GetAppUnqueInfo());

            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine("MachineUnqueKey: " + EnvHelp.MachineUnqueKey);
            Console.WriteLine("AppUnqueKey: " + EnvHelp.AppUnqueKey);

            Console.ReadLine();
            return;

            while (true)
            {
                var info = UsageHelp.GetUsageInfo();

                Console.Write("Cpu使用情况:" + info.cpuUsage + "%   ");
                //Console.Write("内存使用情况:" + infos[1] + "%   ");

                //Console.Write("network:" + infos[2] + "/" + infos[3] + " Mbps   ");



                Console.WriteLine("");
                Thread.Sleep(1000);
            }

 
             
        }
    }
}
