using Sers.Core.Util.Common;
using System;
using System.Threading;
using Sers.Core.Extensions;
using System.Text;
using Sers.Mq.Socket.Iocp;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main11(string[] args)
        {
            Console.WriteLine("Hello World!");


            //创建连接对象, 连接到服务器
            var clientMq = new ClientMq();

            if (clientMq.Connect())
            {
                clientMq.OnGetFrame = (conn,data) => 
                {
                    Console.WriteLine("get :" + data.ArraySegmentByteToString());
                };
                //Task.Run(()=> {
                //    while (true)
                //    {
                //        var msg = smanager.mqConn.AppendData.GetMessage();
                      
                //    }

                //});
                
                clientMq.Conn_OnDisconnected = (mqConn) =>
                {
                    Console.WriteLine("ServerStopEvent ServerStopEvent");
                };
            }
            else
            {
                Console.WriteLine("无法连接");
                Console.ReadLine();
                return;
            }
            

           

            while (true)
            {
                String msg = CommonHelp.NewGuid();
                byte[] buff = Encoding.UTF8.GetBytes(msg);

                Console.WriteLine("Send:" + msg);
                clientMq.mqConn.SendFrameAsync(buff.BytesToByteData());

                Thread.Sleep(1000);
            }




        }
    }
}
