using Sers.Core.Util.ConfigurationManager;
using Sers.Core.Util.Statistics;
using System;
using System.Threading;
using System.Collections.Generic;
using Sers.Mq.Ipc.SharedMemory.Stream;
using Sers.Core.Extensions;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {

 

        static void Main(string[] args)
        {

            StartStream();

            while (true)
            {
                //Console.WriteLine("Hello World!");
                Thread.Sleep(10000);
            }

        }


        static void StartStream()
        {
            var stream = new ReadStream();


            stream.OnDisconnected = (conn) =>
            {
                Console.WriteLine("ReadStream  OnDisconnected！");

                Task.Run(() =>
                {
                    Thread.Sleep(1000);
                    Console.WriteLine("ReadStream  restart...");
                    StartStream();
                });

            };

            stream.OnReceiveMessage = (conn, msg) =>
            {
                Console.WriteLine(msg.ArraySegmentByteToString());
            };

            if (!stream.SharedMemory_Malloc())
            {
                Console.WriteLine("SharedMemory_Malloc error");
                Console.ReadLine();
                return;
            }

            if (!stream.Start())
            {
                Console.WriteLine("Start error");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Started ");
            Console.WriteLine("wait for client to send msg... ");
        }
    }
}
