using System;
using System.Threading;
using Vit.Core.Util.Statistics;
using System.Collections.Generic;
using Vit.Core.Module.Log;
using Sers.Core.CL.CommunicationManage;
using Sers.Core.CL.MessageOrganize;

namespace ConsoleApp1
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

            Console.WriteLine("Hello World!");

            while (true)
            {                 
                Thread.Sleep(5000);
            }

        }
    }
}
