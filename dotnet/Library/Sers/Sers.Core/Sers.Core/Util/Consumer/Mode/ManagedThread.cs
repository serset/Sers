using System;
using System.Runtime.CompilerServices;

using Newtonsoft.Json.Linq;

using Vit.Core.Util.Threading.Worker;
using Vit.Extensions.Newtonsoft_Extensions;

namespace Sers.Core.Util.Consumer
{

    public class ManagedThread<T> : IConsumer<T>
    {

        Vit.Core.Util.Threading.Worker.ManagedThread<T> task = new Vit.Core.Util.Threading.Worker.ManagedThread<T>();

        public Action<T> Processor { get => task.Processor; set => task.Processor = value; }
        public Action<ETaskFinishStatus, T> OnFinish { get => task.OnFinish; set => task.OnFinish = value; }

        public string threadName { get => task.threadName; set => task.threadName = value; }

        /// <summary>
        /// 常驻线程数，默认16。可为0
        /// </summary>
        public int threadCount { get => task.threadCount; set => task.threadCount = value; }

        /// <summary>
        /// 最大线程数（包含常驻线程和临时线程），默认100。
        /// </summary>
        public int maxThreadCount { get => task.maxThreadCount; set => task.maxThreadCount = value; }

        /// <summary>
        /// 等待队列的最大长度（默认：100000）
        /// </summary>
        public int pendingQueueLength { get => task.pendingQueueLength; set => task.pendingQueueLength = value; }

        /// <summary>
        ///  超时时间。脉冲间隔。（主动关闭超过此时间的任务,实际任务强制关闭的时间会在1倍超时时间到2倍超时时间内)。单位：ms。(默认300000)
        /// </summary>
        public int timeoutMs { get => task.timeoutMs; set => task.timeoutMs = value; }

        public bool isRunning { get => task.IsRunning; }


        public void Init(JObject config)
        {
            threadCount = config["threadCount"]?.Deserialize<int?>() ?? 16;
            maxThreadCount = config["maxThreadCount"]?.Deserialize<int?>() ?? 100;
            pendingQueueLength = config["pendingQueueLength"]?.Deserialize<int?>() ?? 100000;
            timeoutMs = config["timeoutMs"]?.Deserialize<int?>() ?? 300000;
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
