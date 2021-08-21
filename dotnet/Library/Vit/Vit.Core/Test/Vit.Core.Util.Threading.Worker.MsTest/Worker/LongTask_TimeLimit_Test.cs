using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

using Vit.Core.Util.Threading.Worker;

namespace Vit.Core.Util.Threading.MsTest.Worker
{
    [TestClass]
    public class LongTask_TimeLimit_Test
    {
        [TestMethod]
        public void TestMethod()
        {
            int count = 0;
            string errorMessage = "";

            var task = new LongTask_TimeLimit<object>
            {                 
                Processor = (worker) =>
                {
                    var _count = Interlocked.Increment(ref count);
                    Thread.Sleep(400);
                },
                OnFinish = (status,_count) =>
                {
                    if (!
                    (status == ETaskFinishStatus.timeout
                    || (status == ETaskFinishStatus.overload && (int)_count > 100)
                    ))
                        errorMessage += Environment.NewLine + $"[{_count}]status : "+ status;
                }
            };

            task.threadCount = 10;
            task.pendingQueueLength = 100;
            task.timeoutMs = 100;

            task.Start();


            for (var t = 0; t < 200; t++)
                task.Publish(t);

            Thread.Sleep(1050);
            task.OnFinish = null;
            task.Stop();

            var curCount = count;
            Assert.IsTrue(curCount <= 100);
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
