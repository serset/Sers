using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Vit.Core.Module.Log;
using Vit.Extensions;

namespace Vit.Core.Util.Threading
{
    /// <summary>
    /// 注意！！！慎用！！！
    /// 务必确保可以手动结束任务。
    /// 在每次调用完成后才会检测是否结束。调用过程中是不会结束的，除非手动抛出异常。
    /// </summary>
    public class RepeatTaskHelp : IDisposable
    {
        
        ~RepeatTaskHelp()
        {
            Dispose();
        }

        public virtual void Dispose()
        {
            sendStopSignal=true;
        }


        /// <summary>
        /// 线程名称
        /// </summary>
        public string threadName;


        /// <summary>
        /// 线程数,默认为1
        /// </summary>
        public int threadCount=1;
        /// <summary>
        /// 每个线程重复执行的次数,默认为1。若指定为0则重复无限次数
        /// </summary>
        public int repeatCountPerThread=1;


        /// <summary>
        /// 
        /// </summary>
        public Action action;

        /// <summary>
        /// 出现异常时是否终止,默认false
        /// </summary>
        public bool stopWhenException { get; set; } = false;


        /// <summary>
        /// 是否发送结束信号
        /// </summary>
        bool sendStopSignal { get; set; } = false;


        int runningThreadCount = 0;
        /// <summary>
        /// 执行中的线程数
        /// </summary>
        public int RunningThreadCount => runningThreadCount;

     

        public bool IsRunning => runningThreadCount != 0;//threads != null && threads.Any(item=> item.IsAlive);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Run()
        {
            Interlocked.Increment(ref runningThreadCount);
            try
            {
                if (repeatCountPerThread <= 0)
                {
                    //(x.1)重复无限次数
                    while (true)
                    {
                        try
                        {
                            action?.Invoke();
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex);
                            if (stopWhenException)
                            {
                                return;
                            }
                        }
                    }
                }
                else
                {
                    //(x.2)重复指定次数
                    for (int finishedCount = 0; !sendStopSignal && finishedCount < repeatCountPerThread; finishedCount++)
                    {
                        try
                        {
                            action?.Invoke();
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex);
                            if (stopWhenException)
                            {
                                return;
                            }
                        }
                    }
                }
            }
            finally
            {
                Interlocked.Decrement(ref runningThreadCount);                
            }
        }


        public void Start()
        {
            if (IsRunning)
            {
                throw LongTaskHelp.Error_CannotStartWhileRunning.ToException();
            }

            sendStopSignal = false;

            //for (int i = 0; i < threadCount; i++)
            //{
            //    Task.Run((Action)Run);
            //}

            var threads = new Thread[threadCount];
            for (int i = 0; i < threadCount; i++)
            {
                var thread = new Thread(Run);
                thread.IsBackground = true;
                thread.Name = threadName + "-" + i;
                thread.Start();
                threads[i] = thread;
            }

        }
        /// <summary>
        /// 只发送发送结束信号，不保证会立即结束所有任务
        /// </summary>
        /// <returns></returns>
        public void Stop()
        {
            sendStopSignal = true;
        }

    }
}
