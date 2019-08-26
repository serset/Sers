using Sers.Core.Util.Hardware;
using System;
using System.Threading;

namespace CPUInfoDemo
{


    class Program
    {

        static void Main(string[] args)
        {


            while (true)
            {
                var info = DeviceManage.GetUsageInfo();

                Console.Write("Cpu使用情况:" + info.cpuUsage + "%   ");
                //Console.Write("内存使用情况:" + infos[1] + "%   ");

                //Console.Write("network:" + infos[2] + "/" + infos[3] + " Mbps   ");



                Console.WriteLine("");
                Thread.Sleep(1000);
            }

 


            Console.ReadKey();
        }
    }
}
