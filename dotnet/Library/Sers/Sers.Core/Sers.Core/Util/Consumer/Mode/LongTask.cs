using Newtonsoft.Json.Linq;

using System;
using System.Runtime.CompilerServices;
using Vit.Core.Util.Threading.Worker;
using Vit.Extensions;



namespace Sers.Core.Util.Consumer
{

    public class LongTask<T> : IConsumer<T>
    {

        public Action<T> Processor { get => task.Processor; set => task.Processor = value; }
        public Action<ETaskFinishStatus, T> OnFinish { get => task.OnFinish; set => task.OnFinish = value; }



        Vit.Core.Util.Threading.Worker.LongTask<T> task = new Vit.Core.Util.Threading.Worker.LongTask<T>();

        public string threadName { get; set; }

        public int threadCount { get => task.threadCount; set => task.threadCount = value; }

        /// <summary>
        /// 等待队列的最大长度（默认：100000）
        /// </summary>
        public int pendingQueueLength { get => task.pendingQueueLength; set => task.pendingQueueLength = value; }


        public bool isRunning { get => task.IsRunning; }  


        public void Init(JObject config)
        {
            threadCount = config["threadCount"]?.Deserialize<int?>() ?? 16;
            pendingQueueLength = config["pendingQueueLength"]?.Deserialize<int?>() ?? 100000;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Publish(T t)
        {
            task.Publish(t);
        }


        public void Start()
        {
            task.Stop();
            task.Start();
        }

        public void Stop()
        {
            task.Stop();
        }

         
    }
}
