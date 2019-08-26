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

namespace Client
{
    class Program
    {
 
        static void Main(string[] args)
        {

            var stream = new WriteStream();

            //stream.OnDisconnected = () =>
            //{
            //    Console.WriteLine("WriteStream  OnDisconnected！");
            //};

            if (!stream.SharedMemory_Attach())
            {
                Console.WriteLine("SharedMemory_Attach error");
                Console.ReadLine();
                return;
            }
      
            if (!stream.Start())  
            {
                Console.WriteLine("Start error");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Started ");
            Console.WriteLine("write line to send(text 'quit' to quit): ");
            int t = 1;
            while (true)
            {
                var msg = Console.ReadLine();

                if (msg == "quit")
                {
                    Console.WriteLine("send quit signal");
                    stream.Stop();
                    //Console.ReadLine();
                    return;
                }

                stream.SendMessageAsync(((t++)+"-"+msg).SerializeToBytes());

            }

           
        }



      
    }
}
