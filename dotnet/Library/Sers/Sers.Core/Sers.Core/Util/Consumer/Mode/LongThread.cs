using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;

using Newtonsoft.Json.Linq;

using Vit.Core.Module.Log;
using Vit.Core.Util.Threading.Worker;
using Vit.Extensions.Newtonsoft_Extensions;

using WorkTask = Vit.Core.Util.Threading.Worker.LongThread;

namespace Sers.Core.Util.Consumer
{
    /// <summary>
    /// qps : 260万   producer:16    consumer:16
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LongThread<T> : IConsumer<T>
    {

        public Action<T> Processor { get; set; }
        public Action<ETaskFinishStatus, T> OnFinish { get; set; }


        WorkTask task = new WorkTask();

        public string threadName { get => task.threadName; set => task.threadName = value; }

        public int threadCount { get => task.threadCount; set => task.threadCount = value; }


        public bool isRunning { get => task.IsRunning; }

        BlockingCollection<T> queue = new BlockingCollection<T>();


        public LongThread()
        {
            task.Processor = _Processor;
        }


        public void Init(JObject config)
        {
            threadCount = config["threadCount"]?.Deserialize<int?>() ?? 16;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Publish(T t)
        {
            queue.Add(t);
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


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void _Processor()
        {
            while (true)
            {
                try
                {
                    #region Process
                    while (true)
                    {
                        var msgFrame = queue.Take();
                        Processor(msgFrame);
                        OnFinish?.Invoke(ETaskFinishStatus.success, msgFrame);
                    }
                    #endregion
                }
                catch (Exception ex) when (!(ex.GetBaseException() is ThreadInterruptedException))
                {
                    Logger.Error(ex);
                }
            }
        }

    }
}
