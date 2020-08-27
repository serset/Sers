using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CLClient.Statistics;
using Sers.Core.CL.CommunicationManage;
using Vit.Core.Module.Log;
using Vit.Extensions;

namespace CLClient
{
    class ProgramQps
    {

        static StatisticsQpsInfo qpsInfo = new StatisticsQpsInfo();
        static void Main(string[] args)
        {
            Logger.OnLog = (level, msg) => { Console.Write("[" + level + "]" + msg); };


            qpsInfo.Start("Msg");


            for (int i = Vit.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetByPath<int>("PressureTest.clientCount"); i >0; i--)
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

            CommunicationManageClient cm = new CommunicationManageClient();
         

            cm.Conn_OnDisconnected = (conn) =>
            {
                Logger.Info("Conn_OnDisconnected");       

                Sers.Core.Module.App.SersApplication.OnStop();
            };

            cm.conn_OnGetRequest = (conn,sender,request,callback)=> 
            {
                qpsInfo.IncrementRequest();
                callback(sender,new List<ArraySegment<byte>> { request });
            };

            cm.conn_OnGetMessage = (conn,msg) => 
            {
                qpsInfo.IncrementRequest();
                cm.SendMessageAsync(new List<ArraySegment<byte>> { msg });
            };


            if (!cm.Start())
            {
                return;
            }


            byte[] buff = new byte[Vit.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetByPath<int>("PressureTest.msgLen")];


            for (int i = Vit.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetByPath<int>("PressureTest.messageThreadCount"); i > 0; i--)
            {
                cm.SendMessageAsync(buff.BytesToByteData());
            }

            for (int i = Vit.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetByPath<int>("PressureTest.requestThreadCount"); i > 0; i--)
            {       

                Task.Run(() =>
                {
                    var conn = cm.organizeList[0].conn;
                    for (;;)
                    {
                        if (conn.SendRequest(buff.BytesToByteData(), out _))
                            qpsInfo.IncrementRequest();
                    }

                });

            }



        }
    }
}
