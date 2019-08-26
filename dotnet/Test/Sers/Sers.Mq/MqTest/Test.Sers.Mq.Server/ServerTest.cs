using Sers.Core.Extensions;
using Sers.Core.Mq.Mng;
using Sers.Core.Util.ConfigurationManager;
using Sers.Core.Util.Statistics;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Test.Sers.Mq.Client
{
    public class ServerTest
    {
        public readonly ServerMqMng mqMng = new ServerMqMng();


        void WriteLine(string v)
        {
            Console.WriteLine(v);
        }


        public void Init()
        {

            mqMng.UseSocket();

            mqMng.UseZmq();


            #region 注册 Mq 回调

            mqMng.OnReceiveRequest = (connGuid,oriData) => {
                return new List<ArraySegment<byte>>() {oriData};
            };

            mqMng.Conn_OnDisconnected = (connGuid) => { WriteLine("[ServerMq] 连接断开"); };

            mqMng.Conn_OnConnected = (connGuid) => {

                StartThreadSendRequest(connGuid);
                StartThreadSendMessage(connGuid);
            };

            #endregion

            #region printMsg sendBack

            bool printMsg = ConfigurationManager.Instance.GetByPath<bool>("Server.Message.printMsg");
            bool sendBack = ConfigurationManager.Instance.GetByPath<bool>("Server.Message.sendBack");
            if (printMsg)
            {

                Console.WriteLine("printMsg");

                var qpsInfo = new StatisticsQpsInfo();
                qpsInfo.Start("Msg");

                if (sendBack)
                {
                    mqMng.OnReceiveMessage = (connGuid, msg) =>
                    {
                        qpsInfo.IncrementRequest();
                        mqMng.SendMessage(connGuid, new List<ArraySegment<byte>> { msg });
                    };
                }
                else
                {
                    mqMng.OnReceiveMessage = (connGuid, msg) =>
                    {
                        qpsInfo.IncrementRequest();
                    };
                }
            }
            else
            {
                if (sendBack)
                {
                    mqMng.OnReceiveMessage = (connGuid, msg) =>
                    {
                        mqMng.SendMessage(connGuid, new List<ArraySegment<byte>> { msg });
                    };
                }
            }
            #endregion

        }





        public void Connect()
        {
            #region ServerMq 连接服务器    

            WriteLine("[ServerMq] 准备开始监听");

            if (!mqMng.Start())
            {
                WriteLine("[ServerMq] 开始监听 失败");

            }
            else
            {
                WriteLine("[ServerMq] 开始监听 成功");
            }
            #endregion
        }


        #region ThreadSendMessage


        void StartThreadSendMessage(string connGuid)
        {

            int threadCount = ConfigurationManager.Instance.GetByPath<int>("Server.Message.threadCount");
            int countPerThread = ConfigurationManager.Instance.GetByPath<int>("Server.Message.countPerThread");

            if (threadCount <= 0) return;



            Console.WriteLine("StartThreadSendMessage");

            for (var t = 0; t < threadCount; t++)
            {
                Task.Run(() =>
                {
                    for (int i = 0; i < countPerThread; i++)
                    {
                        mqMng.SendMessage(connGuid, new List<ArraySegment<byte>> { reqData });
                    }
                });
            }
        }
        #endregion



        #region ThreadSendRequest
        StatisticsQpsInfo qpsInfo_Request = new StatisticsQpsInfo();
        byte[] reqData = "test".StringToBytes();
        bool SendRequest(string connGuid)
        {
            var d1 = DateTime.Now;
            mqMng.SendRequest(connGuid, new List<ArraySegment<byte>> { reqData }, out var rep);

            qpsInfo_Request.IncrementRequestTicks((DateTime.Now - d1).Ticks);
            qpsInfo_Request.IncrementRequest();

            if (rep != null && rep.Count != 0)
            {
                return true;
            }
            else
            {
                qpsInfo_Request.IncrementError();
                return false;
            }
        }
        void StartThreadSendRequest(string connGuid)
        {

            int threadCount = ConfigurationManager.Instance.GetByPath<int>("Server.Request.threadCount");
            int countPerThread = ConfigurationManager.Instance.GetByPath<int>("Server.Request.countPerThread");

            if (threadCount <= 0) return;

            Console.WriteLine("StartThreadSendRequest");

            qpsInfo_Request.Start("Request");
            int finishedThreadCount = 0;

            for (var t = 0; t < threadCount; t++)
            {
                Task.Run(() =>
                {
                    for (int i = 0; i < countPerThread; i++)
                    {
                        SendRequest(connGuid);
                    }

                    if (threadCount <= Interlocked.Increment(ref finishedThreadCount))
                    {
                        qpsInfo_Request.Stop();
                        qpsInfo_Request = new StatisticsQpsInfo();
                    }
                });
            }
        }


        #endregion
    }
}
