using Vit.Core.Util.Threading;
using System;
using System.Threading;

namespace Vit.Core.MsTest.Util.Threading
{
    public class SersTimer_Test
    {
        public static void Test()
        {

            var timer = new SersTimer();


            timer.intervalMs = 1000;


            int count = 1;

     
            timer.timerCallback = (state) => {
                var curCount = count++;

 
                Console.Out.WriteLine($"[{curCount}]start ");
                Thread.Sleep(1500);

                Console.Out.WriteLine($"[{curCount}]stop ");
 
            };


            timer.Start();

            Thread.Sleep(10000);
        }
    }
}
