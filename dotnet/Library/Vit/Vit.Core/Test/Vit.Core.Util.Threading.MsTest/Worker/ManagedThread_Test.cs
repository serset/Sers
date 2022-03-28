using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using Vit.Core.Util.Threading.Worker;

namespace Vit.Core.Util.Threading.MsTest.Worker
{
    [TestClass]
    public class ManagedThread_Test
    {
        [TestMethod]
        public void TestMethod()
        {
            int count = 0;
            string errorMessage = "";

            var task = new ManagedThread<object>
            {                 
                Processor = (worker) =>
                {
                    var _count = Interlocked.Increment(ref count);
                    Thread.Sleep(400);
                },
                OnFinish = (status,_count) =>
                {
                    if( status == ETaskFinishStatus.timeout
                       || (status == ETaskFinishStatus.overload && (int)_count > 100)
                    ) 
                        return;

                        errorMessage += Environment.NewLine + $"[{_count}]status : "+ status;
                }
            };

            task.threadName = "TestThread";
            task.threadCount = 4;
            task.maxThreadCount = 10;
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
            Thread.Sleep(100);

            Assert.IsTrue(!task.IsRunning);
            var curCount2 = count;
            Thread.Sleep(200);
            Assert.AreEqual(count, curCount2);

            if (!string.IsNullOrEmpty(errorMessage))
                Assert.Fail(errorMessage);



        }
    }
}
