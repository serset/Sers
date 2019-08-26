using Sers.Apm.SkyWalking.Core.Model;
using System;
using System.Collections.Generic;

namespace Sers.Apm.SkyWalking.Core.Test
{
    class Program
    {
        static void Main(string[] args)
        {


            SkyWalkingHost.config["SkyWalking:Transport:gRPC:Servers"] = "192.168.56.101:11800";

            SkyWalkingHost.Init();



            Console.WriteLine("Hello World!");


            while (true)
            {
                var route = Console.ReadLine().Trim();
                Console.WriteLine("route:" + route);

                var m = new RequestModel();

                m.startTime_dt = DateTime.Now.AddSeconds(-1);

                m.endTime_dt = DateTime.Now;
                m.route = "/test/SkyWalking/"+ route;
                m.requestId = "4567890";
                m.parentRequestId = null;               

                SkyWalkingHost.Publish(new List<RequestModel> { m});

            }
        }
    }
}
