using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

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

            var task = new LongThread
            {
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

            task.threadCount = 2;

            task.Start();

            Thread.Sleep(500);

            task.Stop();

            Assert.AreEqual(6,count);

            Thread.Sleep(200);
            Assert.AreEqual(6,count);

       
             
        }
    }
}
