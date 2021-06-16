using Vit.Core.Util.Threading;
using System;
using System.Threading;

namespace Vit.Core.MsTest.Util.Threading
{
    public class SersTimer_SingleThread_Test
    {
        public static void Test()
        {

            var timer = new SersTimer_SingleThread();


            timer.intervalMs = 1;


            int count = 1;


            timer.timerCallback = (state) => {
                var curCount = Interlocked.Increment(ref count);


                Console.Out.WriteLine($"[{curCount}]start ");
                Thread.Sleep(500);

                Console.Out.WriteLine($"[{curCount}]stop ");

            };


            timer.Start();

            Thread.Sleep(10000);
        }
    }
}
