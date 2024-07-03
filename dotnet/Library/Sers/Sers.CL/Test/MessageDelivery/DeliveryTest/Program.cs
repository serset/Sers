using System;
using System.Threading;

using Vit.Core.Module.Log;

namespace DeliveryTest
{
    class Program
    {


        static void Main(string[] args)
        {
            Logger.PrintToConsole = true;

            StartServer();
            StartClient();


            while (true)
            {
                Thread.Sleep(5000);
            }

        }




        static void StartServer()
        {

            //var server = new Sers.CL.WebSocket.DeliveryServer();
            //var server = new Sers.CL.ClrZmq.ThreadWait.DeliveryServer();
            //var server = new Sers.CL.Ipc.SharedMemory.DeliveryServer();
            //var server = new Sers.CL.Zmq.FullDuplex.DeliveryServer();
            var server = new Sers.CL.Ipc.NamedPipe.DeliveryServer();

            server.Conn_OnConnected = (conn) =>
            {
                conn.OnGetFrame = (conn_, data) =>
                {
                    //data[0]++;
                    //data[1] = 5;
                    var byteData = new Vit.Core.Util.Pipelines.ByteData(data);

                    conn_.SendFrameAsync(byteData);
                };
            };


            server.Start();

        }

        static Vit.Core.Util.Pipelines.ByteData staticByteData => new Vit.Core.Util.Pipelines.ByteData(new ArraySegment<byte>(new byte[] { 0, 1, 2, 3 }));


        static void StartClient()
        {
            //var client = new Sers.CL.WebSocket.DeliveryClient();
            //var client = new Sers.CL.ClrZmq.ThreadWait.DeliveryClient();
            //var client = new Sers.CL.Ipc.SharedMemory.DeliveryClient();
            //var client = new Sers.CL.Zmq.FullDuplex.DeliveryClient();
            var client = new Sers.CL.Ipc.NamedPipe.DeliveryClient();

            client.Conn_OnGetFrame = (conn, data) =>
            {
                //data[0]++;

                //data[1]=10;
                var byteData = new Vit.Core.Util.Pipelines.ByteData(data);
                conn.SendFrameAsync(byteData);
            };

            var connected = client.Connect();

            Thread.Sleep(1000);

            client.conn.SendFrameAsync(staticByteData);


        }

    }
}
