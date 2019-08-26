using System;
using Sers.Core.ServiceCenter.ApiTrace;

namespace Sers.Apm.SkyWalking.Test
{
    /// <summary>
    /// 注意：要手动复制 dll/runtimes文件夹
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            Sers.Apm.SkyWalking.SkyWalkingManage.config["SkyWalking:Transport:gRPC:Servers"] = "192.168.56.101:11800";

            Sers.Apm.SkyWalking.SkyWalkingManage.Init();

  

            Console.WriteLine("Hello World!");


            while (true)
            {
                var route = Console.ReadLine().Trim();
                Console.WriteLine("route:"+route);

                TraceModel m = new TraceModel();

                m.startTime = DateTime.Now.AddSeconds(-2);

                m.endTime = DateTime.Now;
                m.route = "/test/SersSkyWalking/"+ route;
                m.requestId = "4567890";
                m.parentRequestId = null;
                m.rootRequestId = null;

                m.Publish();

            }
        }
    }
}
