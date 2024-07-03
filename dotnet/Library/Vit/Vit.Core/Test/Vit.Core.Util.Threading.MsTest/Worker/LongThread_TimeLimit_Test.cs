using System;
using System.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Vit.Core.Util.Threading.Worker;

namespace Vit.Core.Util.Threading.MsTest.Worker
{
    [TestClass]
    public class LongThread_TimeLimit_Test
    {
        [TestMethod]
        public void TestMethod()
        {
            int count = 0;
            string errorMessage = "";

            var task = new LongThread_TimeLimit<int>
            {
                GetWork = () =>
                {
                    var _count = Interlocked.Increment(ref count);

                    //Thread.Sleep(100);
                    return _count;
                },
                Processor = (_count) =>
                {
                    Thread.Sleep(400);
                },
                OnFinish = (status, _count) =>
                {
                    if (status != ETaskFinishStatus.timeout)
                        errorMessage += Environment.NewLine + $"error in times [{_count}]";
                }
            };

            task.threadCount = 8;
            task.timeoutMs = 100;

            task.Start();

            Thread.Sleep(1050);
            task.Stop();

            var curCount = count;
            Assert.IsTrue(curCount <= 80);
            Thread.Sleep(10);

            Assert.IsTrue(!task.IsRunning);
            var curCount2 = count;
            Thread.Sleep(200);
            Assert.AreEqual(count, curCount2);

            if (!string.IsNullOrEmpty(errorMessage))
                Assert.Fail(errorMessage);



        }
    }
}
