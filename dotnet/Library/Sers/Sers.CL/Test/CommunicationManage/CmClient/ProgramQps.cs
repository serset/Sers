﻿using System;
using System.Threading;

using CLClient.Statistics;

using Sers.Core.CL.CommunicationManage;

using Vit.Core.Module.Log;
using Vit.Core.Util.Pipelines;

namespace CLClient
{
    class ProgramQps
    {

        static StatisticsQpsInfo qpsInfo = new StatisticsQpsInfo();
        static void Main(string[] args)
        {
            Logger.PrintToConsole = true;


            qpsInfo.Start("Msg");


            for (int i = Vit.Core.Util.ConfigurationManager.Appsettings.json.GetByPath<int>("PressureTest.clientCount"); i > 0; i--)
            {
                StartThreadSendMessage();
            }

            Sers.Core.Module.App.SersApplication.OnStart();

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



            cm.conn_OnGetMessage = (conn, msg) =>
            {
                //Task.Run(() =>
                //{
                qpsInfo.IncrementRequest();
                cm.SendMessageAsync(new ByteData(msg));
                //});
            };


            cm.conn_OnGetRequest = (conn, sender, request, callback) =>
            {
                qpsInfo.IncrementRequest();
                callback(sender, new ByteData(request));
            };

            if (!cm.Start())
            {
                return;
            }


            byte[] buff = new byte[Vit.Core.Util.ConfigurationManager.Appsettings.json.GetByPath<int>("PressureTest.msgLen")];


            for (int i = Vit.Core.Util.ConfigurationManager.Appsettings.json.GetByPath<int>("PressureTest.messageThreadCount"); i > 0; i--)
            {
                cm.SendMessageAsync(new ByteData(buff));
            }


            int theadCount = Vit.Core.Util.ConfigurationManager.Appsettings.json.GetByPath<int>("PressureTest.requestThreadCount");

            if (theadCount >= 0)
            {
                ///*/
                var conn = cm.organizeList[0].conn;
                Action<object, ByteData> callback = null;

                callback = (sender, data) =>
                {
                    qpsInfo.IncrementRequest();
                    conn.SendRequestAsync(null, data, callback);
                };


                for (int t = theadCount; t > 0; t--)
                {
                    callback(null, new ByteData(buff));
                }

                /*/

                LongTaskHelp task = new LongTaskHelp();
                task.threadName = "PressureTest.SendRequest";
                task.threadCount = theadCount;
                task.action = () =>
                {

                    var conn = cm.organizeList[0].conn;
                    for (; ; )
                    {
                        if (conn.SendRequest(new ByteData(buff), out _))
                            qpsInfo.IncrementRequest();
                    }
                };
                task.Start();
                //*/


            }






        }
    }
}
