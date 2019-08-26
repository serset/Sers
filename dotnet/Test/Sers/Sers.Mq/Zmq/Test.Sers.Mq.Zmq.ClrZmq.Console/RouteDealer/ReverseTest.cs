using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Sers.Core.Extensions;
using Sers.Core.Util.Common;
using Sers.Mq.Zmq.ClrZmq.RouteDealer;

namespace Test.Sers.Mq.Zmq.ClrZmq.ConsoleDemo.RouteDealer
{
    public static class ReverseTest
    {
        public static void RunTest()
        {
            Task.Run((Action)Server);


            Task.Run((Action)Client);
            Task.Run((Action)Client);
            Task.Run((Action)Client);

            while (true) Thread.Sleep(1000);
        }


        static void Server()
        {


            using (var server = new global::Sers.Mq.Zmq.ClrZmq.RouteDealer.ServerMq(new ServerMqConfig()))
            {

                server.Start();

                Thread.Sleep(1000);

                for (int i = 0; i < 5; i++)
                {
                    RunThread("s" + i);
                }
                void RunThread(string reqNode)
                {
                    Task.Run(() =>
                    {
                        Thread.Sleep(2000);

                        for (int i = 0; i < 10; i++)
                        {
                            //SendReq(0);
                            SendReq(i % 3);
                        }


                        Console.WriteLine("finished: reqNode " + reqNode);

                        void SendReq(int clientIndex)
                        {
                            var repNode = "c" + clientIndex;

                            /*                           
                            {                            
                                    reqDesc:{
                                        reqNode:"",
                                        repNode:""
                                    }

                                    repDesc:{                                   
                                        repNode:""
                                    }                            
                            }
                             */
                            var req = new JObject();
                            var reqDesc = new JObject();
                            req["reqDesc"] = reqDesc;

                            reqDesc["reqNode"] = reqNode;
                            reqDesc["repNode"] = repNode;

                            server.SendRequest(repNode, req.ToString().StringToByteData(),out var replyData);
                            var strRep = replyData.ArraySegmentByteToString();

                            var rep = JObject.Parse(strRep??"{}");

                            if (
                                (rep["reqDesc"]?["reqNode"]?.Value<string>() != reqNode)
                                ||
                                (rep["reqDesc"]?["repNode"]?.Value<string>() != rep["repDesc"]?["repNode"]?.Value<string>())
                                )
                            {
                                string shouldBe = rep["reqDesc"]?["reqNode"]?.Value<string>() + "->" + rep["reqDesc"]?["repNode"]?.Value<string>();
                                string inFact = rep["reqDesc"]?["reqNode"]?.Value<string>() + "->" + rep["repDesc"]?["repNode"]?.Value<string>() + "->" + reqNode;
                                Console.WriteLine("err  shouldBe:" + shouldBe + " inFact:" + inFact);
                            }


                        }

                    });
                }


                while (true) Thread.Sleep(1000);


            }

        }



        static int count = 0;

        static void Client()
        {

            var identity = "c" + (count++);
            using (var client = new ClientMq(new ClientMqConfig()))
            {
           
                client.OnReceiveRequest = (request) =>
                {
                    var rep = JObject.Parse(request.ArraySegmentByteToString());
                    rep["repDesc"] = new JObject
                    {
                        ["repNode"] = identity
                    };
                    return rep.ToString().StringToByteData();
                };
                client.Connect(identity);

                while (true) Thread.Sleep(1000);
            }
        }
    }
}
