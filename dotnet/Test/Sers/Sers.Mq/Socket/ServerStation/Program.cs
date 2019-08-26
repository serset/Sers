using Sers.Core.Extensions;
using Sers.Core.Module.App;
using Sers.Mq.Socket.Channel;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Main
{
    class Program
    {
        static void Main(string[] args)
        {
            string serviceStationName = "ServiceStation"+DateTime.Now.ToString("ss.ffff");
            Console.WriteLine(serviceStationName);

            try
            {
                ClientMq clientMq = new ClientMq(new ClientMqConfig());
              
                

                clientMq.OnReceiveRequest = (oriData) => {


                    /*/
                    var strReq = oriData.ArraySegmentByteToString();
                    Console.WriteLine("收到request:" + strReq);
                    Console.WriteLine("请求server.." );
                    var rep= clientMq.mqConnect.SendRequest( new System.Collections.Generic.List<ArraySegment<byte>> { oriData });
                    var strRep = rep.ArraySegmentByteToString();
                    Console.WriteLine("收到server回应:" + strRep);

                    Console.WriteLine("返回reply:" + strRep);
                    return new System.Collections.Generic.List<ArraySegment<byte>> { rep };

                    /*/
                    var strReq = oriData.ArraySegmentByteToString();
                    var strRep = "[reply from serviceStation] serviceStationName:" + serviceStationName + " oriRequest:" + strReq;

                    //Console.WriteLine("收到request:" + strReq);
                    //Thread.Sleep(200);
                    //Console.WriteLine("发送reply:" + strRep);
                    return strRep.StringToByteData();
                

                    //*/
                };

              

                clientMq.Conn_OnDisconnected = () =>
                {
                    Console.WriteLine("conn.OnDisconnected");
                };


                Console.WriteLine("Connect(\"127.0.0.1\",18888)");

                clientMq.config.host = "127.0.0.1";
                clientMq.config.port = 18888;
                var connected=clientMq.Connect();

                if (connected)
                {
                    Console.WriteLine("conn.OnConnected");
                }
                else
                {
                    Console.WriteLine("conn.Connecte fail");
                }

                try
                {
                    
                    while (true)
                    {
                        var strReq = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(strReq))
                        {

                            Console.WriteLine("发送request:" + strReq);
                            #region 发送请求
                            Task.Run(() =>
                            {
                                var rep = clientMq.SendRequest(new System.Collections.Generic.List<ArraySegment<byte>> { strReq.StringToArraySegmentByte() });
                                var strRep = rep.ArraySegmentByteToString();
                                Console.WriteLine("收到回应:" + strRep);
                            });
                            #endregion
                        }
                    }
                }
                catch (Exception)
                {
                    
                }

                SersApplication.RunAwait();

            }
            catch (Exception e)
            {
               
            }

            

        }

        



    }
}
