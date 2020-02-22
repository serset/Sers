using System;

namespace Sers.Hardware.Net46
{


    class Program
    {

        static void Main(string[] args)
        {


            using (var cupInfo = new CpuInfo())
            {

                while (true)
                {
                    if ("exit" == Console.ReadLine()) return;

                    var infos = cupInfo.GetInfo();

                    Console.Write("Cpu:" + infos[0] + ",");
                    Console.Write("Memory:" + infos[1] + ",");
                    Console.Write("NetworkIn:" + infos[2] + ",");
                    Console.WriteLine("NetworkOut:" + infos[3] + "");

                    //Console.Write("Cpu使用情况:" + infos[0] + "%   ");
                    //Console.Write("内存使用情况:" + infos[1] + "%   ");
                    //Console.Write("network:" + infos[2] + "/" + infos[3] + " Mbps   ");





                    //Console.WriteLine("");
                    //Thread.Sleep(100);
                }
            }



            Console.ReadKey();
        }
    }
}
