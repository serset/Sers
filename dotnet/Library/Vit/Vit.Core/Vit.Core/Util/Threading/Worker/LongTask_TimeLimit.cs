using System;
using System.Threading;
using Vit.Extensions;
using Vit.Core.Module.Log;
using Vit.Core.Util.Threading.Timer;
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
    public class LongTask_TimeLimit<T> : IDisposable
    {

        /// <summary>
        /// 不可抛异常
        /// </summary>
        public Action<T> Processor;

        /// <summary>
        /// 不可抛异常
        /// status: success/error/timeout/overload
        /// </summary>
        public Action<ETaskFinishStatus, T> OnFinish;

        /// <summary>
        /// 线程名称
        /// </summary>
        public string threadName;

        /// <summary>
        /// 线程数，默认100
        /// </summary>
        public int threadCount = 100;

        /// <summary>
        /// 等待队列的最大长度（默认：100000）
        /// </summary>
        public int pendingQueueLength = 100000;

      
        public int RunningThreadCount => runInfo.runningThreadCount;


        #region RunInfo
     
        class RunInfo 
        {
          
            /// <summary>
            /// 运行中或准备启动的线程个数
            /// </summary>
            public int curThreadCount = 0;

            /// <summary>
            /// 在运行中的线程个数
            /// </summary>
            public int runningThreadCount = 0;

            public readonly ConcurrentQueue<T> pendingQueue = new ConcurrentQueue<T>();

            public readonly ConcurrentDictionary<int, WorkInfo> workInfoCache = new ConcurrentDictionary<int, WorkInfo>();

        }
        RunInfo runInfo = new RunInfo();

        #endregion


        public bool IsRunning => NeedRunning;

        bool NeedRunning { get; set; } = false; 

        /// <summary>
        /// 超时时间。脉冲间隔。（主动关闭超过此时间的任务,实际任务强制关闭的时间会在1倍超时时间到2倍超时时间内)。单位：ms。(默认300000)
        /// </summary>
        public int timeoutMs { get => pulseMaker.intervalMs; set => pulseMaker.intervalMs = value; }


        public LongTask_TimeLimit()
        {
            pulseMaker = new SersTimer { intervalMs = 300000, timerCallback = PulseMake };
        }


        ~LongTask_TimeLimit()
        {
            Dispose();
        }

        public virtual void Dispose()
        {
            Stop();
        }


        #region 电子脉冲
        /// <summary>
        /// 脉冲生产器
        /// </summary>
        readonly SersTimer pulseMaker;

        /// <summary>
        /// 电子脉冲，在固定的时间间隔发送脉冲
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void PulseMake(object obj)
        {
            if (!IsRunning) return;

            foreach (var workInfo in runInfo.workInfoCache.Values)
            {
                workInfo.Pulse();
            }
        }
        #endregion


        public void Start()
        {
            lock (this)
            {
                if (IsRunning)
                {
                    throw WorkerHelp.Error_CannotStartWhileRunning.ToException();
                }
                NeedRunning = true;              

                //(x.2)开启脉冲生产器
                pulseMaker.Start();
            }
        }

        public void Stop()
        {
            lock (this)
            {
                if (!IsRunning) return;
                NeedRunning = false;

                try
                {
                    pulseMaker.Stop();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }

                var info = runInfo;
                runInfo = new RunInfo();
                Task.Run(() =>
                { 
                    foreach (var workInfo in info.workInfoCache.Values)
                    {
                        workInfo.TryStop();
                    }

                    if (OnFinish != null) 
                    {
                        T arg;
                        while (info.pendingQueue.TryDequeue(out arg))
                        {
                            OnFinish?.Invoke(ETaskFinishStatus.overload, arg);
                        }
                    }
                });
            }
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Publish(T arg)
        {
            if (IsRunning)
            {
                //(x.1)启动新线程
                if (runInfo.runningThreadCount < threadCount)
                {
                    if(WorkInfo.StartNewTask(this, arg))
                        return;
                }

                //(x.2)推入等待队列
                if (runInfo.pendingQueue.Count < pendingQueueLength)
                {
                    runInfo.pendingQueue.Enqueue(arg);
                    return;
                }
            }

            //(x.3)返回服务过载回应
            OnFinish?.Invoke(ETaskFinishStatus.overload, arg);
        }

       

        class WorkInfo
        {
            int pulseCount = 0;

            public T arg;
            LongTask_TimeLimit<T> task;
            public Thread thread;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryStop()
            {
                try
                {
                    if (thread.IsAlive)
                        thread.Interrupt();
                }
                catch { }
                return thread.IsAlive;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Pulse()
            {
                if (!thread.IsAlive)
                    return;
                try
                {
                    Interlocked.Increment(ref pulseCount);
                    if (pulseCount >= 2)
                        TryStop();
                }
                catch { }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool StartNewTask(LongTask_TimeLimit<T> task, T arg)
            {
                var info = task.runInfo;
                var curThreadCount = Interlocked.Increment(ref info.curThreadCount);

                try
                {
                    if (curThreadCount <= task.threadCount)
                    {
                        var workInfo = new WorkInfo { arg = arg, task = task };
                        var thread = new Thread(workInfo.Run);
                        workInfo.thread = thread;
                        thread.IsBackground = true;
                        thread.Name = task.threadName + "-" + thread.ManagedThreadId;
                        thread.Start();
                        return true;
                    }                    
                }
                catch
                {
                    Interlocked.Decrement(ref info.curThreadCount);
                    throw;
                }

                Interlocked.Decrement(ref info.curThreadCount);
                return false;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void Run()
            {
                var info = task.runInfo;
                Interlocked.Increment(ref info.runningThreadCount);
                try
                {
                    info.workInfoCache.TryAdd(this.GetHashCode(), this);

                    T arg_;
                    while (task.NeedRunning)
                    {
                        try
                        {
                            pulseCount = 0;
                            if (arg == null)
                            {
                                if (!info.pendingQueue.TryDequeue(out arg_))
                                    return;
                            }
                            else
                            {
                                arg_ = arg;
                                arg = default;
                            }

                            #region do work
                            ETaskFinishStatus status = ETaskFinishStatus.success;
                            try
                            {
                                task.Processor(arg_);
                            }
                            catch (Exception ex) when (ex.GetBaseException() is ThreadInterruptedException)
                            {
                                status = ETaskFinishStatus.timeout;
                            }
                            catch (Exception ex)
                            {
                                status = ETaskFinishStatus.error;
                                Logger.Error(ex);
                            }
                            finally
                            {
                                task.OnFinish?.Invoke(status, arg_);
                            }
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex);
                        }
                    }
                }
                finally
                {
                    Interlocked.Decrement(ref info.runningThreadCount);
                    Interlocked.Decrement(ref info.curThreadCount);
                    info.workInfoCache.TryRemove(this.GetHashCode(), out _);
                }

            }
        }

    }
}
