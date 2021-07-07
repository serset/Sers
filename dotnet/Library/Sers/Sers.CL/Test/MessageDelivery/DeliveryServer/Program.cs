using CLServer.Statistics;
using Sers.Core.CL.MessageDelivery;
using System;
using System.Threading;
using System.Threading.Tasks;
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
            IDeliveryServer server;

            {
                var delivery = new Sers.CL.Socket.Iocp.Mode.ThreadWait.DeliveryServer();
                //var delivery = new Sers.CL.Socket.Iocp.Mode.Timer.DeliveryServer();
                //delivery.receiveBufferSize = 81920;

                delivery.port = port;

                server = delivery;
            }
          
            //server = new Sers.CL.Socket.ThreadWait.DeliveryServer();
       

            //server = new Sers.CL.WebSocket.DeliveryServer();
            //server = new Sers.CL.ClrZmq.ThreadWait.DeliveryServer();



            //server = new Sers.CL.Zmq.FullDuplex.DeliveryServer();

            //server = new Sers.CL.Ipc.SharedMemory.DeliveryServer();     
            //server = new Sers.CL.Ipc.NamedPipe.DeliveryServer();

            server.Conn_OnConnected = (conn) =>
            {
                Logger.Info("Conn_OnConnected");

                conn.OnGetFrame = (conn_, data) =>
                {
                    //Task.Run(() =>
                    //{
                    qpsInfo.IncrementRequest();

                    //Thread.Sleep(1);

                    //data[0]++;
                    //data[1] = 5;

                    var byteData = new Vit.Core.Util.Pipelines.ByteData(data);
                    conn_.SendFrameAsync(byteData);
                    //});
                };
            };


            server.Start();

        }




    }
}
