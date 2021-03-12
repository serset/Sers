﻿using CLServer.Statistics;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Vit.Core.Module.Log;
using Vit.Core.Util.Threading;

namespace DeliveryTest
{
    class Program
    {


        static StatisticsQpsInfo qpsInfo = new StatisticsQpsInfo();
        static void Main(string[] args)
        {
            Logger.OnLog = (level, msg) => { Console.Write("[" + level + "]" + msg); };

            try
            {

                if (args != null && args.Length > 0)
                {
                    int t = 0;

                    if (args.Length > t)
                    {
                        host = args[t];
                    }

                    t++;
                    if (args.Length > t)
                    {
                        int.TryParse(args[t], out port);
                    }

                    t++;
                    if (args.Length > t)
                    {
                        int.TryParse(args[t], out thread);
                    }


                    t++;
                    if (args.Length > t)
                    {
                        int.TryParse(args[t], out msgLen);
                    }

                }

                qpsInfo.Start("Msg");

                StartClient();

                while (true)
                {
                    Thread.Sleep(5000);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

        }

        static string host = "127.0.0.1";
        static int port = 4501;

        static int thread = 1;
        static int msgLen = 10;





        static void StartClient()
        {
            var client = new Sers.CL.Socket.Iocp.DeliveryClient();
            //var client = new Sers.CL.Socket.ThreadWait.DeliveryClient();
            client.host = host;
            client.port = port;

            //var client = new Sers.CL.WebSocket.DeliveryClient();
            //var client = new Sers.CL.ClrZmq.ThreadWait.DeliveryClient();
            //var client = new Sers.CL.Ipc.SharedMemory.DeliveryClient();
            //var client = new Sers.CL.Zmq.FullDuplex.DeliveryClient();
            //var client = new Sers.CL.Ipc.NamedPipe.DeliveryClient();

            client.Conn_OnGetFrame = (conn, data) =>
           {
               Task.Run(() =>
               {
                   qpsInfo.IncrementRequest();

                   //Thread.Sleep(1);

                   //data[0]++;
                   //data[1] = 10;                 

                   var byteData = new Vit.Core.Util.Pipelines.ByteData(data);
                   conn.SendFrameAsync(byteData);
               });

           };

            var connected = client.Connect();

            Console.WriteLine(connected?"连接成功": "连接失败");


            byte[] buff = new byte[msgLen];

            for (var t = 0; t < thread; t++)
            {
                client.conn.SendFrameAsync(new Vit.Core.Util.Pipelines.ByteData(new ArraySegment<byte>(buff))  );
                //Task.Run(()=> {

                //    while (true)
                //    {
                //        for (int t1 = 0; t1 < 100; t1++)
                //        {
                //            client.conn.SendFrameAsync(new Vit.Core.Util.Pipelines.ByteData( new ArraySegment<byte>(buff)));
                //        }
                //        Thread.Sleep(1);
                //    }
                
                //});
            }

        }

    }
}