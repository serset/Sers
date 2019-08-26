using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sers.Core.Extensions;
using Sers.Core.Util.Common;
using Sers.Mq.Zmq.ClrZmq.RouteDealer;

namespace Test.Sers.Mq.Zmq.ClrZmq.ConsoleDemo.RouteDealer
{
    public class PositiveTest
    {

        public static void RunTest()
        {
            Server();
            Client();

            while (true) Thread.Sleep(1000);
        }



        static void Server()
        {
            Task.Run(() =>
            {
                using (var server = new global::Sers.Mq.Zmq.ClrZmq.RouteDealer.ServerMq(new ServerMqConfig()))
                {

                    server.OnReceiveRequest = (connGuid,req) =>
                    {
                        return new List<ArraySegment<byte>> { req };
                    };

                    server.Start();

                    while (true) Thread.Sleep(1000);
                }
            });


        }


        static void Client()
        {
            Task.Run(() =>
            {
               
                using (var client = new global::Sers.Mq.Zmq.ClrZmq.RouteDealer.ClientMq(new ClientMqConfig()))
                {

                    client.Connect();


                    for (int i = 0; i < 10; i++)
                    {
                        ClientRunThread(client, "c" + i);
                    }
                    //var strRep = ClientSendRequest(client, "hellotest");
                    while (true) Thread.Sleep(1000);
                }
            });


        }

      


        static int finishCount = 0;
        static void ClientRunThread(ClientMq client, string threadId)
        {
            Task.Run(() =>
            {
                int sucCount = 0;
                int failCount = 0;
                for (int i = 0; i < 1000; i++)
                {
                    var req = threadId + "test" + i + CommonHelp.NewGuid();
                    var rep = ClientSendRequest(client, req);
                    //var rep = ClientSendRequest(req);
                    if (req != rep)
                    {
                        failCount++;
                        Console.WriteLine("err threadId " + threadId + " req:" + req + " rep:" + rep);
                    }
                    else
                    {
                        sucCount++;
                        //Console.WriteLine("suc threadId " + threadId + " req:" + req );
                    }
                    //Console.WriteLine(rep);
                }
                finishCount++;
                Console.WriteLine("[" + finishCount + "]finished: threadId " + threadId + $" failCount[{failCount}]  sucCount[{sucCount}]");


            });
        }
        static string ClientSendRequest(string requ)
        {
            using (var client = new global::Sers.Mq.Zmq.ClrZmq.RouteDealer.ClientMq(new ClientMqConfig()))
            {
                client.Connect();

                var reply = client.SendRequest(requ.StringToByteData());

                var strRep = reply.ArraySegmentByteToString();
                return strRep;
            }
        }



        static string ClientSendRequest(ClientMq client, string requ)
        {
            var reply = client.SendRequest(requ.StringToByteData());

            var strRep = reply.ArraySegmentByteToString();
            return strRep;
        }
    }
}
