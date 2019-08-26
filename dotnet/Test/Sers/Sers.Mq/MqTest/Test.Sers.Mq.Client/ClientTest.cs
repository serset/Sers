using Sers.Core.Extensions;
using Sers.Core.Mq.Mng;
using Sers.Core.Util.ConfigurationManager;
using Sers.Core.Util.Pool;
using Sers.Core.Util.Statistics;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Test.Sers.Mq.Client
{
    public class ClientTest
    {
        public readonly ClientMqMng mqMng = new ClientMqMng();


        void WriteLine(string v)
        {
            Console.WriteLine(v);
        }


        public void Init()
        {

            mqMng.UseSocket();

            mqMng.UseZmq();


            #region 注册 Mq 回调 

            mqMng.OnReceiveRequest = (oriData) => {
                var rep = DataPool.ByteDataGet();
                rep.Add(oriData);
                return rep;
                //return new List<ArraySegment<byte>>() {oriData};
            };

           

            mqMng.Conn_OnDisconnected = () => { WriteLine("[ClientMq] 连接断开"); };         


            #endregion

        }

        public void Close()
        {
            mqMng.Close();
        }




        public void Connect()
        {
            #region ClientMq 连接服务器    

            WriteLine("[ClientMq] 准备连接服务器");

            if (!mqMng.Connect())
            {
                WriteLine("[ClientMq] 连接服务器 失败");
                return;
            }
            else
            {
                WriteLine("[ClientMq] 连接服务器 成功");
            }
            #endregion

            StartThreadSendRequest();

            StartThreadSendMessage();

            #region printMsg sendBack

            
            bool printMsg = ConfigurationManager.Instance.GetByPath<bool>("Client.Message.printMsg");
            bool sendBack = ConfigurationManager.Instance.GetByPath<bool>("Client.Message.sendBack");
            if (printMsg)
            {

                Console.WriteLine("printMsg");
                var qpsInfo = new StatisticsQpsInfo();
                qpsInfo.Start("Msg");

                if (sendBack)
                {
                    mqMng.OnReceiveMessage = (msg) =>
                    {
                        qpsInfo.IncrementRequest();
                        mqMng.SendMessage(new List<ArraySegment<byte>> { msg });
                    };
                }
                else
                {
                    mqMng.OnReceiveMessage = (msg) =>
                    {
                        qpsInfo.IncrementRequest();
                    };
                }
            }
            else
            {
                if (sendBack)
                {
                    mqMng.OnReceiveMessage = (msg) =>
                    {
                        mqMng.SendMessage(new List<ArraySegment<byte>> { msg});
                    };
                }
            }
            #endregion

        }


        #region ThreadSendMessage

        

        void StartThreadSendMessage()
        {

            int threadCount = ConfigurationManager.Instance.GetByPath<int>("Client.Message.threadCount");
            int countPerThread = ConfigurationManager.Instance.GetByPath<int>("Client.Message.countPerThread");

            if (threadCount <= 0) return;



            Console.WriteLine("StartThreadSendMessage");

            for (var t = 0; t < threadCount; t++)
            {
                Task.Run(() =>
                {
                    for (int i = 0; i < countPerThread; i++)
                    {
                        mqMng.SendMessage(new List<ArraySegment<byte>> { reqData });
                    }
                });
            }
        }


        #endregion


        #region ThreadSendRequest

        StatisticsQpsInfo qpsInfo_Request = new StatisticsQpsInfo();
        byte[] reqData = "test".StringToBytes();
        bool SendRequest()
        {
            var d1 = DateTime.Now;

            var req=DataPool.ByteDataGet();
            req.Add(reqData);
            var rep = mqMng.SendRequest(req);
            try
            {
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
            finally
            {
                //req.ReturnToPool();
                //rep.ReturnToPool();
            }
        }

     

        void StartThreadSendRequest()
        {
            int threadCount = ConfigurationManager.Instance.GetByPath<int>("Client.Request.threadCount");
            int requestPerThread = ConfigurationManager.Instance.GetByPath<int>("Client.Request.countPerThread");



            if (threadCount <= 0) return;


            Console.WriteLine("StartThreadSendRequest");

            #region method          


            int finishedThreadCount = 0;

            qpsInfo_Request.Start("Request");

            void OnFinish()
            {
                qpsInfo_Request.Stop();
            }

            void OnThreadFinish()
            {
                if (Interlocked.Increment(ref finishedThreadCount) == threadCount)
                {
                    OnFinish();
                }
            }
            
            #endregion


            #region startThread

            

            for (int i = 0; i < threadCount; i++)
            {
                Task.Run(() =>
                {                   
                    for (int count = 0; count < requestPerThread; count++)
                    {
                        SendRequest();
                    }
                    OnThreadFinish();
                });
            }
            #endregion
        }

        #endregion

    }
}
