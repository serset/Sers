using System;
using System.Collections.Generic;
using System.Threading;
using CLServer.Statistics;
using Sers.Core.CL.CommunicationManage;
using Sers.Core.CL.MessageOrganize;
using Vit.Core.Module.Log;

namespace CLServer
{
    class Program
    {

        static StatisticsQpsInfo qpsInfo = new StatisticsQpsInfo();
        static void Main(string[] args)
        {
           

            Logger.OnLog = (level, msg) => { Console.Write("[" + level + "]" + msg); };

            qpsInfo.Start("Msg");

            CommunicationManageServer cm = new CommunicationManageServer();
           

            cm.conn_OnGetMessage = (conn, msg) =>
            {
                qpsInfo.IncrementRequest();
                conn.SendMessageAsync(new List<ArraySegment<byte>> { msg });               
            };


            cm.conn_OnGetRequest = (IOrganizeConnection  conn, Object sender, ArraySegment<byte> requestData, Action<object, List<ArraySegment<byte>>> callback) =>
            {
                qpsInfo.IncrementRequest();
                callback(sender,new List<ArraySegment<byte>> { requestData });
            };

            cm.Conn_OnConnected = (conn) =>
            {
                Logger.Info("Conn_OnConnected");
            };

            cm.Conn_OnDisconnected = (conn) =>
            {
                Logger.Info("Conn_OnDisconnected");
            };


            cm.Start();

            while (true)
            {                 
                Thread.Sleep(5000);
            }

        }
    }
}
