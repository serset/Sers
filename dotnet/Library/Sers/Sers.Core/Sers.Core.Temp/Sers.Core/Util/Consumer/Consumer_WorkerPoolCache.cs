using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Sers.Core.Util.Consumer
{
    /// <summary>
    /// qps : 1200万   producer:32    consumer:32
    /// qps : 1000万   producer:16    consumer:16
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Consumer_WorkerPoolCache<T> : IConsumer<T>
    {

        public int workThreadCount { get; set; } = 2;

        public string name { get; set; }

        public Action<T> processor { get; set; }


        int curRootIndex;

        Consumer_WorkerPool<T>[] rootWorkerList;

        public bool IsRunning { get; private set; } = false;


        public void Start()
        {
            if (IsRunning) return;
            IsRunning = true;


            rootWorkerList = Enumerable.Range(0, workThreadCount).Select(m =>
            {
                var worker = new Consumer_WorkerPool<T>();
                worker.processor = processor;
                worker.workThreadCount = 1;
                //worker.Start();
                return worker;
            }).ToArray();

            curRootIndex = 0;

            rootWorkerList.ToList().ForEach(m => m.Start());
        }


        public void Stop()
        {
            if (!IsRunning) return;
            IsRunning = false;

            rootWorkerList?.ToList().ForEach(m => m.Stop());
            rootWorkerList = null;
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Publish(T data)
        { 
            var index = Interlocked.Increment(ref curRootIndex);

            index = Math.Abs(index) % rootWorkerList.Length;

            rootWorkerList[index]?.Publish(data);
        }






    }
}
