using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Vit.Extensions;

namespace Sers.Core.Util.Consumer
{
    /// <summary> 
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="Consumer"></typeparam>
    public class ConsumerCascade<T,Consumer> : IConsumer<T>
        where Consumer:IConsumer<T>,new()
    {

        public int threadCount { get; set; } = 16;

        public string threadName { get; set; }


        public Action<T> processor { get; set; }
        public Action<T> OnFinish { get; set; }
        public Action<T> OnTimeout { get; set; }


        int curRootIndex;

        Consumer [] rootWorkerList;



        public bool isRunning { get; private set; } = false;

        JObject config;
        public void Init(JObject config)
        {
            this.config = config;
            threadCount = config["threadCount"]?.Deserialize<int?>() ?? 16;
        }

        public void Start()
        {
            if (isRunning) return;
            isRunning = true;


            rootWorkerList = Enumerable.Range(0, threadCount).Select(m =>
            {
                var worker = new Consumer();
                worker.Init(config);
                worker.processor = processor;
                worker.OnFinish = OnFinish;
                worker.OnTimeout = OnTimeout;
                worker.threadCount = 1;
                worker.threadName = threadName;
                //worker.Start();
                return worker;
            }).ToArray();

            curRootIndex = 0;

            rootWorkerList.ToList().ForEach(m => m.Start());
        }


        public void Stop()
        {
            if (!isRunning) return;
            isRunning = false;

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
