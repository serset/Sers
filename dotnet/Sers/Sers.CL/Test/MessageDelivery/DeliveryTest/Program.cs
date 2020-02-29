using System;
using System.Collections.Generic;
using System.Threading;
using Vit.Core.Module.Log;

namespace DeliveryTest
{
    class Program
    {

   
        static void Main(string[] args)
        {
            Logger.OnLog = (level, msg) => { Console.Write("[" + level + "]" + msg); };



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
            var server = new Sers.CL.Zmq.FullDuplex.DeliveryServer();
 
            server.Conn_OnConnected = (conn) => 
            {
                conn.OnGetFrame = (conn_, data) =>
                {
                    data[0]++;
                    data[1] = 5;
                    var byteData = new List<ArraySegment<byte>>() { data };

                    conn_.SendFrameAsync(byteData);
                };
            };

           

            server.Start();

        }

        static List<ArraySegment<byte>> staticByteData => new List<ArraySegment<byte>>() { (new ArraySegment<byte>(new byte[] { 0, 1, 2, 3 })) };

 
        static void StartClient()
        {
            //var client = new Sers.CL.WebSocket.DeliveryClient();
            //var client = new Sers.CL.ClrZmq.ThreadWait.DeliveryClient();
            //var client = new Sers.CL.Ipc.SharedMemory.DeliveryClient();
            var client = new Sers.CL.Zmq.FullDuplex.DeliveryClient();

            client.Conn_OnGetFrame = (conn, data) =>
            {
                data[0]++;

                data[1]=10;
                var byteData = new List<ArraySegment<byte>>() { data };
                conn.SendFrameAsync(byteData);
                //Console.WriteLine("ss");
            };

            var connected = client.Connect() ;

            Thread.Sleep(1000);           
         
            client.conn.SendFrameAsync(staticByteData);


        }

    }
}
