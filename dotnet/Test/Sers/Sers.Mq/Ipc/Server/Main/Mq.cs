using Sers.Core.Util.ConfigurationManager;
using Sers.Core.Util.Statistics;
using System;
using System.Threading;
using System.Collections.Generic;
using Sers.Mq.Ipc.SharedMemory.Stream;
using Sers.Core.Extensions;
using Sers.Mq.Ipc.SharedMemory.Mq;

namespace ConsoleApp1
{
    class Program
    {

        static StatisticsQpsInfo qpsInfo = new StatisticsQpsInfo();

        static void Main(string[] args)
        {
            


            //}
            var stream = new ServerMq() { name = "test" };

            stream.Conn_OnGetFrame = (conn, data) =>
            {
                qpsInfo.IncrementRequest();

                //stream.SendMessageAsync(data.ArraySegmentByteToBytes());
                conn.SendFrameAsync(new List<ArraySegment<byte>> { data }); 
            };


            //m_socket.Conn_OnConnected = (conn)=>
            //{
            //    String msg = CommonHelp.NewGuid();
            //    msg = "1";
            //    byte[] buff = Encoding.UTF8.GetBytes(msg);

            //    Console.WriteLine("StartThreadSendMessage");

            //    Task.Run(() =>
            //    {
            //        //Thread.Sleep(5000);
            //        for (int i = 0; i < 50000;)
            //        {
            //            conn.SendMessage(buff.BytesToByteData());
            //            //Thread.Sleep(10);
            //        }
            //    });
            //};

            if (!stream.Start())
            {
                Console.WriteLine("conn error");
                return;
            }


            qpsInfo.Start("Msg");

            Console.WriteLine("Hello World!");

            while (true)
            {
                //Console.WriteLine("Hello World!");
                Thread.Sleep(10000);
            }

        }
    }
}
