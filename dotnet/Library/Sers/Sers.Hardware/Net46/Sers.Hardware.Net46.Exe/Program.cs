using System;
using Sers.Hardware.Net46.Exe.Ms;

namespace Sers.Hardware.Net46.Exe
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


                    //Console.WriteLine("");
                    //Thread.Sleep(100);
                }
            }



            //Console.ReadKey();
        }
    }
}
