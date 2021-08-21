using Vit.Core.Util.Threading;
using System;
using System.Threading;
using Vit.Core.Util.Threading.Timer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Vit.Core.Util.Threading.MsTest.Timer
{
    [TestClass]
    public class SersTimer_Test
    {

        [TestMethod]
        public void TestMethod()
        {
            int count = 0;

            var timer = new SersTimer();
            timer.intervalMs = 100;


            string errorMessage = "";
            timer.timerCallback = (state) =>
            {
                var curCount = Interlocked.Increment(ref count);

                Console.Out.WriteLine($"[{curCount}]start ");

                if (count != curCount) 
                    errorMessage += Environment.NewLine+ $"error in times [{curCount}]" + count;

                Thread.Sleep(150);

                if (timer.isRunning && count != (curCount+1)) 
                    errorMessage += Environment.NewLine + $"error2 in times [{curCount}]"+ count;

                Console.Out.WriteLine($"[{curCount}]stop  ");
            };


            timer.Start();

            Thread.Sleep(1050);

            timer.Stop();

            Assert.AreEqual(10,count);

            Thread.Sleep(100);
            Assert.AreEqual(10,count);

            if(!string.IsNullOrEmpty(errorMessage))
                Assert.Fail(errorMessage);
        }
    }
}
