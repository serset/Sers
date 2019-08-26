using Sers.Core.Util.Common;
using System;
using System.Threading;
using Sers.Core.Extensions;
using System.Text;
using Sers.Mq.Socket.Iocp;
using Sers.Core.Util.Statistics;
using System.Threading.Tasks;
using Sers.Core.Util.ConfigurationManager;
using System.Collections.Generic;
using Sers.Core.Module.Log;

namespace Client
{
    class ProgramQps
    {
        static int msgLen = ConfigurationManager.Instance.GetByPath<int>("mq.msgLen");
        static void Main(string[] args)
        {
            Logger.OnLog = (level, msg) => { Console.Write("[" + level + "]" + msg); };

            Console.WriteLine("Hello World!");
            qpsInfo.Start("Msg");


            for (int t = ConfigurationManager.Instance.GetByPath<int>("mq.clientCount"); t > 0; t--)
            {
                StartThreadSendMessage();
            }

            while (true)
            {
                Thread.Sleep(5000);
            }
        }



        static StatisticsQpsInfo qpsInfo = new StatisticsQpsInfo();
      
        static void StartThreadSendMessage()
        {


            ////var clientMq = new Sers.Mq.Socket.Socket.ClientMq();
            //var clientMq = new Sers.Mq.Socket.Iocp.ClientMq();

            //clientMq.host = ConfigurationManager.Instance.GetStringByPath("Sers.Mq.Socket.host");
            //clientMq.port = ConfigurationManager.Instance.GetByPath<int>("Sers.Mq.Socket.port");

            var clientMq = new Sers.Mq.Zmq.ClrZmq.ThreadWait.ClientMq();

            clientMq.endpoint = "tcp://" + ConfigurationManager.Instance.GetStringByPath("Sers.Mq.Socket.host") + ":" + ConfigurationManager.Instance.GetByPath<int>("Sers.Mq.Socket.port");


            if (clientMq.Connect())
            {
                clientMq.OnGetFrame = (conn, data) =>
                {
                    qpsInfo.IncrementRequest();
                    conn.SendFrameAsync(new List<ArraySegment<byte>> { data });
                    //data.ReturnToPool();
                    //Console.WriteLine("get :" + data?.ByteDataToString());
                };
             
                
                clientMq.Conn_OnDisconnected = (mqConn) =>
                {
                    Console.WriteLine("ServerStopEvent ServerStopEvent");
                };
            }
            else
            {
                Console.WriteLine("conn error");
                return;
            }


            String msg = "1";
            byte[] buff = Encoding.UTF8.GetBytes(msg);
            buff = new byte[msgLen];
            Console.WriteLine("StartThreadSendMessage");

            for (int t = ConfigurationManager.Instance.GetByPath<int>("mq.threadCount"); t >0; t--)
            {
                Thread.Sleep(1);
                //t++;
                clientMq.mqConn.SendFrameAsync(buff.BytesToByteData());
            }
        }
    }
}
