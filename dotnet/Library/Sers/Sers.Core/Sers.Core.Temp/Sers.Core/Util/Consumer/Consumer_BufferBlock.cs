using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using Vit.Core.Module.Log;
using Vit.Core.Util.Threading;

namespace Sers.Core.Util.Consumer
{
    /// <summary>
    ///  数据流（任务并行库 TPL） 参考 https://blog.csdn.net/weixin_33697898/article/details/89700239
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Consumer_BufferBlock<T>: IConsumer<T>
    {

        public int workThreadCount { get; set; } = 16;

        public string name { get; set; }

        public Action<T> processor { get; set; }


        BufferBlock<T> queue = new BufferBlock<T>(new DataflowBlockOptions { 
        
        
        });
        LongTaskHelp task = new LongTaskHelp();

        public bool IsRunning { get => task.IsRunning; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Publish(T t) 
        {          
            queue.SendAsync(t);
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
                        //var msgFrame = queue.Receive();
                        processor(queue.Receive());
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
