using CLServer.Statistics;
using System;
using System.Threading;
using Vit.Core.Module.Log;

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
                    int.TryParse(args[0], out port);
                }


                qpsInfo.Start("Msg");

                StartServer();


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

        static int port = 4501;


        static void StartServer()
        {
            var server = new Sers.CL.Socket.Iocp.DeliveryServer();
            server.port = port;

            //var server = new Sers.CL.WebSocket.DeliveryServer();
            //var server = new Sers.CL.ClrZmq.ThreadWait.DeliveryServer();
            //var server = new Sers.CL.Ipc.SharedMemory.DeliveryServer();
            //var server = new Sers.CL.Zmq.FullDuplex.DeliveryServer();
            //var server = new Sers.CL.Ipc.NamedPipe.DeliveryServer();

            server.Conn_OnConnected = (conn) =>
            {
                Logger.Info("Conn_OnConnected");

                conn.OnGetFrame = (conn_, data) =>
                {
                    qpsInfo.IncrementRequest();

                    //data[0]++;
                    //data[1] = 5;

                    var byteData = new Vit.Core.Util.Pipelines.ByteData(data);
                    conn_.SendFrameAsync(byteData);
                };
            };


            server.Start();

        }




    }
}
