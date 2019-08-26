using Sers.Core.Util.ConfigurationManager;
using Sers.Core.Util.Statistics;
using System;
using System.Threading;
using System.Collections.Generic;
using Sers.Core.Module.Log;

namespace ConsoleApp1
{
    class Program
    {

        static StatisticsQpsInfo qpsInfo = new StatisticsQpsInfo();

        static void Main(string[] args)
        {

            Logger.OnLog = (level, msg) => { Console.Write("[" + level + "]" + msg); };


            // var m_socket = new Sers.Mq.Socket.Iocp.ServerMq();
            ////var m_socket = new Sers.Mq.Socket.Socket.ServerMq();
            //m_socket.port = ConfigurationManager.Instance.GetByPath<int>("Sers.Mq.Socket.port");

            var m_socket = new Sers.Mq.Zmq.ClrZmq.ThreadWait.ServerMq();
            //var m_socket = new Sers.Mq.Socket.Socket.ServerMq();
            m_socket.endpoint = "tcp://*:" + ConfigurationManager.Instance.GetByPath<int>("Sers.Mq.Socket.port");




            m_socket.Conn_OnGetFrame = (conn,msg) => 
            {
                qpsInfo.IncrementRequest();
                conn.SendFrameAsync(new List<ArraySegment<byte>> { msg }); 
            };

            m_socket.Conn_OnDisconnected = (conn) =>
            {
                Logger.Info("Conn_OnDisconnected");
            };

            m_socket.Conn_OnConnected = (conn) =>
            {
                conn.SendFrameAsync(new List<ArraySegment<byte>> { new ArraySegment<byte> (new byte[] { 1}) });

                //String msg = CommonHelp.NewGuid();
                //msg = "1";
                //byte[] buff = Encoding.UTF8.GetBytes(msg);

                //Console.WriteLine("StartThreadSendMessage");

                //Task.Run(() =>
                //{
                //    //Thread.Sleep(5000);
                //    for (int i = 0; i < 50000;)
                //    {
                //        conn.SendMessage(buff.BytesToByteData());
                //        //Thread.Sleep(10);
                //    }
                //});
            };


            qpsInfo.Start("Msg");

            m_socket.Start();

            Console.WriteLine("Hello World!");

            while (true)
            {
                //Console.WriteLine("Hello World!");
                Thread.Sleep(10000);
            }

        }
    }
}
