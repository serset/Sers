using System;
using System.Threading;
using Vit.Core.Module.Log;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Vit.Core.Util.Threading.Worker
{
    /// <summary>
    /// 注意！！！慎用！！！
    /// 线程数量限制不一定准确。
    /// 请勿处理ThreadInterruptedException异常，否则导致线程无法正常结束
    /// 若超时则强制关闭任务。
    /// 通过Task.Run创建新线程。推任务的模式。
    /// </summary>
    public class LongTask<T> : IDisposable
    {

        /// <summary>
        /// 不可抛异常
        /// </summary>
        public Action<T> Processor;

        /// <summary>
        /// 不可抛异常
        /// status: success/error/overload
        /// </summary>
        public Action<ETaskFinishStatus, T> OnFinish;

        /// <summary>
        /// 线程数，默认16
        /// </summary>
        public int threadCount = 16;

        /// <summary>
        /// 等待队列的最大长度（默认：100000）
        /// </summary>
        public int pendingQueueLength = 100000;

        int runningThreadCount = 0;

        public int RunningThreadCount => runningThreadCount;


        ConcurrentQueue<T> pendingQueue = new ConcurrentQueue<T>();

        public bool IsRunning => NeedRunning;

        bool NeedRunning { get; set; } = false;

        ~LongTask()
        {
            Dispose();
        }

        public virtual void Dispose()
        {
            Stop();
        }


        public void Start()
        {
            NeedRunning = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void Stop()
        {
            NeedRunning = false;
            pendingQueue = new ConcurrentQueue<T>();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Publish(T arg)
        {
            if (!IsRunning)
            {
                //(x.1)返回服务过载回应
                OnFinish?.Invoke(ETaskFinishStatus.overload, arg);
            }

            //(x.2)启动新线程
            if (runningThreadCount < threadCount)
            {
                StartNewTask(arg);
                return;
            }

            //(x.3)推入等待队列
            if (pendingQueue.Count < pendingQueueLength)
            {
                pendingQueue.Enqueue(arg);
                return;
            }
        }





        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void StartNewTask(T arg_)
        {
            Task.Run(() =>
            {
                Interlocked.Increment(ref runningThreadCount);
                T arg = arg_;
                try
                {
                    while (NeedRunning)
                    {
                        #region (x.1) do work

                        ETaskFinishStatus status = ETaskFinishStatus.success;
                        try
                        {
                            Processor(arg);
                        }
                        catch (Exception ex)
                        {
                            status = ETaskFinishStatus.error;
                            Logger.Error(ex);
                        }
                        finally
                        {
                            OnFinish?.Invoke(status, arg);
                        }
                        #endregion

                        if (!pendingQueue.TryDequeue(out arg)) return;
                    }
                }
                finally
                {
                    Interlocked.Decrement(ref runningThreadCount);
                }
            });
        }
    }
}
