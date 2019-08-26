using Sers.Core.Util.Common;
using System;
using System.Threading;
using Sers.Core.Extensions;
using System.Text;
using Sers.Core.Util.Statistics;
using Sers.Core.Module.Mq.MqManager;
using System.Threading.Tasks;
using System.Collections.Generic;
using Sers.Core.Module.Log;

namespace Client
{
    class ProgramQps
    {

        static StatisticsQpsInfo qpsInfo = new StatisticsQpsInfo();
        static void Main(string[] args)
        {
            Logger.OnLog = (level, msg) => { Console.Write("[" + level + "]" + msg); };


            qpsInfo.Start("Msg");


            for (int i = Sers.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetByPath<int>("mq.clientCount"); i >0; i--)
            {
                StartThreadSendMessage();
            }

            while (true)
            { 

                Thread.Sleep(5000);
            }




        }



      
        static void StartThreadSendMessage()
        {

            ClientMqManager mqMng = new ClientMqManager();

            //Task.Run(() =>
            //{
            //    Thread.Sleep(2000);
            //    Console.Write("mqMng.Stop");
            //    mqMng.Stop();
               
            //});

            mqMng.Conn_OnDisconnected = (conn) =>
            {
                Logger.Info("Conn_OnDisconnected");
            };

            mqMng.station_OnGetRequest = (conn,sender,request,callback)=> 
            {
                qpsInfo.IncrementRequest();
                callback(sender,new List<ArraySegment<byte>> { request });
            };

            mqMng.station_OnGetMessage = (conn,msg) => 
            {
                qpsInfo.IncrementRequest();
                mqMng.Station_SendMessageAsync(new List<ArraySegment<byte>> { msg });
            };


            if (!mqMng.Start())
            {
                return;
            }


            byte[] buff = new byte[Sers.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetByPath<int>("mq.msgLen")];


            for (int i = Sers.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetByPath<int>("mq.messageThreadCount"); i > 0; i--)
            {
                mqMng.Station_SendMessageAsync(buff.BytesToByteData());
            }

            for (int i = Sers.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetByPath<int>("mq.requestThreadCount"); i > 0; i--)
            {       

                Task.Run(() =>
                {
                    var conn = mqMng.mqConns[0];
                    for (int j = 0; j < 100000;)
                    {
                        if (mqMng.Station_SendRequest(buff.BytesToByteData(), out _, conn))
                            qpsInfo.IncrementRequest();
                    }

                });

            }



        }
    }
}
