using System;
using System.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Vit.Core.Util.Threading.Worker;

namespace Vit.Core.Util.Threading.MsTest.Worker
{
    [TestClass]
    public class LongThread_Test
    {
        [TestMethod]
        public void TestMethod()
        {
            int count = 0;

            LongThread.Run(
                () =>
                {
                    while (count < 10)
                    {
                        var curCount = Interlocked.Increment(ref count);
                        Console.Out.WriteLine($"[{curCount}]start ");
                        Thread.Sleep(10);
                    }
                }
             );

            Thread.Sleep(500);

            Assert.AreEqual(10, count);
        }


        [TestMethod]
        public void TestMethod_MultiThread()
        {
            int count = 0;

            var task = new LongThread
            {
                threadCount = 2,
                Processor = () =>
                {
                    while (true)
                    {
                        var curCount = Interlocked.Increment(ref count);
                        Console.Out.WriteLine($"[{curCount}]start ");
                        Thread.Sleep(200);
                    }
                }
            };

            task.Start();

            Thread.Sleep(500);

            task.Stop();

            Assert.AreEqual(6, count);

            Thread.Sleep(200);
            Assert.AreEqual(6, count);
        }
    }
}
