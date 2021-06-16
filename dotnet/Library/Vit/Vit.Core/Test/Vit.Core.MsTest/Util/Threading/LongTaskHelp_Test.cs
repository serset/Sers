using Vit.Core.Util.Threading;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Vit.Core.MsTest.Util.Threading
{
    public class LongTaskHelp_Test
    {
        public static void Test()
        {

            var task = new LongTaskHelp {action =()=>{

                while (true)
                {
                    Console.WriteLine("run in thread");
                    Thread.Sleep(1000);
                }

            } };
            Console.WriteLine("task.Start");
            task.Start();
            Task.Run(()=> {
                Thread.Sleep(3000);

                Console.WriteLine("task.Stop");
                task.Stop();

                Thread.Sleep(3000);
                Console.WriteLine("task.Start");
                task.Start();
            });


            Thread.Sleep(10000);
        }
    }
}
