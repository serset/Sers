using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;
using Vit.Core.Module.Log;
using Vit.Core.Util.Threading;

namespace Sers.Core.Util.Consumer
{
    /// <summary>
    /// qps : 50万   producer:16    consumer:16
    /// qps : 70万   producer:2    consumer:2
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Consumer_BlockingCollection<T>: IConsumer<T>
    {

        public int workThreadCount { get; set; } = 16;

        public string name { get; set; }

        public Action<T> processor { get; set; }


        BlockingCollection<T> queue = new BlockingCollection<T>();
        LongTaskHelp task = new LongTaskHelp();



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Publish(T t) 
        {
            queue.Add(t);
        }


        public void Start() 
        { 
            task.Stop();

            task.threadName = name;
            task.threadCount = workThreadCount;
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
