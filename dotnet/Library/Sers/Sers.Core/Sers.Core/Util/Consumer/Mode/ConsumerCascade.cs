using Newtonsoft.Json.Linq;

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

using Vit.Core.Util.Threading.Worker;
using Vit.Extensions;
using Vit.Extensions.Newtonsoft_Extensions;

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


        public Action<T> Processor { get; set; }
        public Action<ETaskFinishStatus,T> OnFinish { get; set; } 


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
                worker.Processor = Processor;
                worker.OnFinish = OnFinish;              
                worker.threadCount = 1;
                worker.threadName = threadName;      
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
