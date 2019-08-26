using Sers.Core.Module.Mq.Mq;
using Sers.Core.Module.Mq.MqManager;
using System;
using System.Threading;
using Sers.Core.Extensions;
using Sers.Core.Util.Statistics;
using System.Collections.Generic;
using Sers.Core.Module.Log;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {

        static StatisticsQpsInfo qpsInfo = new StatisticsQpsInfo();
        static void Main(string[] args)
        {
           

            Logger.OnLog = (level, msg) => { Console.Write("[" + level + "]" + msg); };

            qpsInfo.Start("Msg");

            ServerMqManager mqMng = new ServerMqManager();

            //Task.Run(()=> {
            //    Thread.Sleep(10000);
            //    mqMng.Stop();
            //    Console.Write("mqMng.Stop");
            //});

            mqMng.station_OnGetMessage = (conn, msg) =>
            {
                qpsInfo.IncrementRequest();
                mqMng.Station_SendMessageAsync(conn,new List <ArraySegment<byte>> { msg });
            };


            mqMng.station_OnGetRequest = (IMqConn conn, Object sender, ArraySegment<byte> requestData, Action<object, List<ArraySegment<byte>>> callback) =>
            {
                qpsInfo.IncrementRequest();
                callback(sender,new List<ArraySegment<byte>> { requestData });
            };

            mqMng.Conn_OnConnected = (conn) =>
            {
                Logger.Info("Conn_OnConnected");
            };

            mqMng.Conn_OnDisconnected = (conn) =>
            {
                Logger.Info("Conn_OnDisconnected");
            };


            mqMng.Start();

            Console.WriteLine("Hello World!");

            while (true)
            {
                //Console.WriteLine("GetConnList:" + mqMng.requestAdaptor.GetConnList().Count());
                //Request.Send("dddd");
                Thread.Sleep(1000);
            }

        }
    }
}
