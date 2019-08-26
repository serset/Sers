using Sers.Core.Util.Common;
using System;
using System.Threading;
using Sers.Core.Extensions;
using System.Text;
 
using Sers.Core.Util.Statistics;
using System.Threading.Tasks;
using Sers.Core.Util.ConfigurationManager;
using System.Collections.Generic;
using Sers.Mq.Ipc.SharedMemory.Stream;
using Sers.Mq.Ipc.SharedMemory.Mq;

namespace Client
{
    class Program
    {
        static int msgLen = ConfigurationManager.Instance.GetByPath<int>("mq.msgLen");
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            qpsInfo.Start("Msg");


            var stream = new ClientMq() { name = "test" };


            stream.OnGetFrame = (conn, data) =>
            {
                qpsInfo.IncrementRequest();
                //stream.SendMessageAsync(data.ArraySegmentByteToBytes());
                conn.SendFrameAsync(new List<ArraySegment<byte>> { data });
            };


            if (!stream.Connect())            
            {
                Console.WriteLine("conn error");
                return;
            }


            String msg = "1";
            byte[] buff = Encoding.UTF8.GetBytes(msg);
            buff = new byte[msgLen];
            Console.WriteLine("StartThreadSendMessage");

            for (int t = ConfigurationManager.Instance.GetByPath<int>("mq.threadCount"); t > 0; t--)
            //for (int t = 100000; t > 0;)
            {
                stream.SendMessageAsync(buff);
                //Thread.Sleep(1);
            }

            while (true)
            {
                Thread.Sleep(5000);
            }
        }



        static StatisticsQpsInfo qpsInfo = new StatisticsQpsInfo();
      
      
    }
}
