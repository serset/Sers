using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;
using Vit.Core.Module.Log;
using Vit.Core.Util.Threading;
using Vit.Extensions;

namespace Sers.Core.Util.Consumer
{
    /// <summary>
    /// qps : 260万   producer:16    consumer:16
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LongTask<T> : IConsumer<T>
    {

        public string threadName { get => task.threadName; set => task.threadName = value; }


        public int threadCount { get => task.threadCount; set => task.threadCount = value; }

        public Action<T> processor { get; set; }
        public Action<T> OnFinish { get; set; }
        public Action<T> OnTimeout { get; set; }


        BlockingCollection<T> queue = new BlockingCollection<T>();
        LongTaskHelp task = new LongTaskHelp();

        public bool isRunning { get => task.IsRunning; }


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

            task.threadName = threadName;
            task.threadCount = threadCount;
            task.action = Processor;
            task.Start();
        }

        public void Stop() 
        {
            task.Stop();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Processor()
        {
            while (true)
            {
                try
                {
                    #region Process                        
                    while (true)
                    {
                        var msgFrame = queue.Take();
                        processor(msgFrame);
                        OnFinish?.Invoke(msgFrame);
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
