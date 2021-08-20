using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

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

        public int workThreadCount { get; set; } = 2;

        public string name { get; set; }


        public Action<T> processor { get; set; }
        public Action<T> OnFinish { get; set; }
        public Action<T> OnTimeout { get; set; }


        int curRootIndex;

        Consumer [] rootWorkerList;



        public bool IsRunning { get; private set; } = false;

        public void Init(JObject config)
        {
        }

        public void Start()
        {
            if (IsRunning) return;
            IsRunning = true;


            rootWorkerList = Enumerable.Range(0, workThreadCount).Select(m =>
            {
                var worker = new Consumer();
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
