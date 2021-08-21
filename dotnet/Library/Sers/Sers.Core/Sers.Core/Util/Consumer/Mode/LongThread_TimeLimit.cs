using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

using Vit.Core.Util.Threading.Worker;
using Vit.Extensions;


namespace Sers.Core.Util.Consumer.Mode
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LongThread_TimeLimit<T> : IConsumer<T>
    {
        public Action<T> Processor { get => task.Processor; set => task.Processor = value; }
        public Action<ETaskFinishStatus, T> OnFinish { get => task.OnFinish; set => task.OnFinish = value; }



        Vit.Core.Util.Threading.Worker.LongThread_TimeLimit<T> task = new Vit.Core.Util.Threading.Worker.LongThread_TimeLimit<T>();


        public string threadName { get => task.threadName; set => task.threadName = value; }


        public int threadCount { get => task.threadCount; set => task.threadCount = value; }


        /// <summary>
        ///  超时时间。脉冲间隔。（主动关闭超过此时间的任务,实际任务强制关闭的时间会在1倍超时时间到2倍超时时间内)。单位：ms。(默认300000)
        /// </summary>
        public int timeoutMs { get => task.timeoutMs; set => task.timeoutMs = value; }

        public bool isRunning => task.IsRunning;


        BlockingCollection<T> queue = new BlockingCollection<T>();

        public LongThread_TimeLimit() 
        {
            task.GetWork = GetWork;
        }


        public void Init(JObject config)
        {
            threadCount = config["threadCount"]?.Deserialize<int?>() ?? 1;
            timeoutMs = config["timeoutMs"]?.Deserialize<int?>() ?? 300000;     
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Publish(T t) 
        {
            queue.Add(t);
        }


        public void Start()
        {
            task.Start();
        }

        public void Stop() 
        {
            task.Stop();
        }

        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        T GetWork()
        {
            //堵塞获取请求
            return queue.Take();
        }
         
       

    }
}
