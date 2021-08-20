using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Vit.Core.Util.Threading;
using Vit.Extensions;

namespace Sers.Core.Util.Consumer.Mode
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LongTask_TimeLimit<T>: IConsumer<T>
    {
        BlockingCollection<T> queue = new BlockingCollection<T>();
        LongTaskHelp_TimeLimit task = new LongTaskHelp_TimeLimit();


        public string threadName { get => task.threadName; set => task.threadName = value; }


        public int threadCount { get => task.threadCount; set => task.threadCount = value; }


        /// <summary>
        ///  超时时间。脉冲间隔。（主动关闭超过此时间的任务,实际任务强制关闭的时间会在1倍超时时间到2倍超时时间内)。单位：ms。(默认300000)
        /// </summary>
        public int timeout_ms { get => task.timeout_ms; set => task.timeout_ms = value; }

        public Action<T> processor { get; set; }
        public Action<T> OnFinish { get; set; }
        public Action<T> OnTimeout { get; set; }



        public bool isRunning { get => task.IsRunning; }

        public void Init(JObject config)
        {
            threadCount = config["threadCount"]?.Deserialize<int?>() ?? 1;
            timeout_ms = config["timeoutMs"]?.Deserialize<int?>() ?? 300000;     
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Publish(T t) 
        {
            queue.Add(t);
        }


        public void Start()
        {
            task.Start(_GetWork, _DealWork, _OnFinish, _OnTimeout);
        }

        public void Stop() 
        {
            task.Stop();
        }


        

        #region TaskToCallApi

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void _GetWork(LongTaskHelp_TimeLimit.Worker w)
        {
            //堵塞获取请求
            w.workArg = queue.Take();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void _DealWork(LongTaskHelp_TimeLimit.Worker w)
        {
            var msg = (T)w.workArg;

            //处理请求
            processor(msg);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void _OnFinish(LongTaskHelp_TimeLimit.Worker w)
        {
            var msg = (T)w.workArg;
             
            OnFinish?.Invoke(msg);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void _OnTimeout(LongTaskHelp_TimeLimit.Worker w)
        {
            var msg = (T)w.workArg;

            OnTimeout?.Invoke(msg);
        }

        #endregion

    }
}
