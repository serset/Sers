using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Vit.Core.Util.Threading.Cache;

namespace Vit.Core.Util.Threading.MsTest.Cache
{

    public abstract class AsyncCache_BaseTest
    {
        public int testCountPerThread = 100;
        public abstract void RunTest(string name);
        public abstract Task RunTestAsync(string name);

        #region Task
        [TestMethod]
        public void Test_Task()
        {
            int threadCount = 0;
            int testCount = 0;

            var tasks = Enumerable.Range(0, 10).Select(number => Task.Run(() =>
                {
                    Interlocked.Increment(ref threadCount);
                    for (var i = 0; i < testCountPerThread; i++)
                    {
                        Interlocked.Increment(ref testCount);
                        var name = $"{number}_{i}_{System.Guid.NewGuid()}";
                        RunTest(name);
                    }
                })
            ).ToArray();

            Task.WaitAll(tasks);
        }

        [TestMethod]
        public void Test_TaskAsync()
        {
            int threadCount = 0;
            int testCount = 0;

            var tasks = Enumerable.Range(0, 10).Select(number => Task.Run(async () =>
                {
                    Interlocked.Increment(ref threadCount);
                    for (var i = 0; i < testCountPerThread; i++)
                    {
                        Interlocked.Increment(ref testCount);
                        var name = $"{number}_{i}_{System.Guid.NewGuid()}";
                        await RunTestAsync(name);
                    }
                })
            ).ToArray();

            Task.WaitAll(tasks);
        }
        #endregion



        #region Thread
        [TestMethod]
        public void Test_Thread()
        {
            int threadCount = 0;
            int testCount = 0;

            Thread_ForEach(Enumerable.Range(0, 100), number =>
            {
                Interlocked.Increment(ref threadCount);
                for (var i = 0; i < testCountPerThread; i++)
                {
                    Interlocked.Increment(ref testCount);
                    var name = $"{number}_{i}_{System.Guid.NewGuid()}";
                    RunTest(name);
                }
            });
        }

        [TestMethod]
        public void Test_ThreadAsync()
        {
            int threadCount = 0;
            int testCount = 0;

            Thread_ForEach(Enumerable.Range(0, 100), number =>
            {
                Interlocked.Increment(ref threadCount);
                for (var i = 0; i < testCountPerThread; i++)
                {
                    Interlocked.Increment(ref testCount);
                    var name = $"{number}_{i}_{System.Guid.NewGuid()}";
                    RunTestAsync(name).Wait();
                }
            });
        }

        void Thread_ForEach<T>(IEnumerable<T> source, Action<T> action, int msSleep = 10)
        {
            var threads = source.Select(i =>
            {
                var thread = new Thread(new ThreadStart(() => action(i)));
                thread.Start();
                return thread;
            }).ToList();

            while (true)
            {
                threads = threads.Where(t => t.IsAlive).ToList();
                if (!threads.Any()) return;
                Thread.Sleep(msSleep);
            }
        }
        #endregion


        #region Parallel
        [TestMethod]
        public void Test_Parallel()
        {
            int threadCount = 0;
            int testCount = 0;

            Parallel.ForEach(Enumerable.Range(0, 10)
                //, new ParallelOptions { MaxDegreeOfParallelism = 1 }
                , number =>
                {
                    Interlocked.Increment(ref threadCount);
                    for (var i = 0; i < testCountPerThread; i++)
                    {
                        Interlocked.Increment(ref testCount);
                        var name = $"{number}_{i}_{System.Guid.NewGuid()}";
                        RunTest(name);
                    }
                });
        }

        [TestMethod]
        public void Test_ParallelAsync()
        {
            int threadCount = 0;
            int testCount = 0;

            Parallel.ForEach(Enumerable.Range(0, 10)
                //, new ParallelOptions { MaxDegreeOfParallelism = 1 }
                , async (number, token) =>
                {
                    Interlocked.Increment(ref threadCount);
                    for (var i = 0; i < testCountPerThread; i++)
                    {
                        Interlocked.Increment(ref testCount);
                        var name = $"{number}_{i}_{System.Guid.NewGuid()}";
                        await RunTestAsync(name);
                    }
                });
        }

        #endregion
    }
}
