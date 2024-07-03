using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;

using Vit.Core.Module.Log;
using Vit.Core.Util.Threading.Timer;
using Vit.Extensions;

namespace Vit.Core.Util.Threading.Worker
{
    /// <summary>
    /// 注意！！！慎用！！！
    /// 线程数量限制不一定准确。
    /// 请勿处理ThreadInterruptedException异常，否则导致线程无法正常结束
    /// 若超时则强制关闭任务。
    /// 托管线程。推任务的模式。
    /// </summary>
    public class ManagedThread<T> : IDisposable
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
        /// 常驻线程数，默认16。可为0
        /// </summary>
        public int threadCount = 16;

        /// <summary>
        /// 最大线程数（包含常驻线程和临时线程），默认100。
        /// </summary>
        public int maxThreadCount = 100;

        /// <summary>
        /// 等待队列的最大长度（默认：100000）
        /// </summary>
        public int pendingQueueLength = 100000;


        public bool IsRunning => runInfo != null;


        /// <summary>
        /// 超时时间。脉冲间隔。（主动关闭超过此时间的任务,实际任务强制关闭的时间会在1倍超时时间到2倍超时时间内)。单位：ms。(默认300000)
        /// </summary>
        public int timeoutMs { get => pulseMaker.intervalMs; set => pulseMaker.intervalMs = value; }


        public ManagedThread()
        {
            pulseMaker = new VitTimer { intervalMs = 300000, timerCallback = Pulse };
        }


        ~ManagedThread()
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
        readonly VitTimer pulseMaker;

        /// <summary>
        /// 电子脉冲，在固定的时间间隔发送脉冲
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void Pulse(object obj)
        {
            runInfo?.Pulse();
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

                //(x.1)
                runInfo = new RunInfo(this);
                runInfo.Start();

                //(x.2)开启脉冲生产器
                pulseMaker.Start();
            }
        }

        public void Stop()
        {
            lock (this)
            {
                if (!IsRunning) return;

                runInfo.needRunning = false;

                var info = runInfo;
                runInfo = null;

                //关闭脉冲生产器
                try
                {
                    pulseMaker.Stop();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }

                //Task.Run(() =>
                {
                    //(x.1)终止常驻线程
                    foreach (var thread in info.longThreadManager.threads)
                    {
                        try
                        {
                            thread.Pulse();
                            thread.Pulse();
                        }
                        catch (Exception)
                        {
                        }
                    }

                    //(x.2)终止临时线程
                    foreach (var thread in info.casualThreadManager.threadMap.Values)
                    {
                        try
                        {
                            thread.Pulse();
                            thread.Pulse();
                        }
                        catch (Exception)
                        {
                        }
                    }

                    //(x.3)清空等待队列
                    if (OnFinish != null)
                    {
                        while (info.pendingQueue.TryTake(out T arg))
                        {
                            OnFinish?.Invoke(ETaskFinishStatus.overload, arg);
                        }
                    }
                }
                //);
            }
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Publish(T arg)
        {
            if (IsRunning)
            {
                //(x.1)启动临时线程
                if (runInfo.longThreadManager.waitingThreadCount == 0
                    && runInfo.casualThreadManager.TryStartNewThread(arg))
                {
                    return;
                }

                //(x.2)推入等待队列
                if (runInfo.pendingQueue.TryAdd(arg))
                {
                    return;
                }
            }

            //(x.3)返回服务过载回应
            OnFinish?.Invoke(ETaskFinishStatus.overload, arg);
        }


        #region RunInfo

        class RunInfo
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


            public bool needRunning = true;

            public readonly BlockingCollection<T> pendingQueue;

            public LongThreadManager longThreadManager;

            public CasualThreadManager casualThreadManager;


            public RunInfo(ManagedThread<T> task)
            {

                this.Processor = task.Processor;
                this.OnFinish = task.OnFinish;


                pendingQueue = new BlockingCollection<T>(task.pendingQueueLength);


                longThreadManager = new LongThreadManager(task);

                casualThreadManager = new CasualThreadManager(task);
            }


            public void Start()
            {
                longThreadManager.Start();
            }


            /// <summary>
            /// 电子脉冲，在固定的时间间隔发送脉冲
            /// </summary>
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public void Pulse()
            {
                longThreadManager.Pulse();
                casualThreadManager.Pulse();
            }

        }
        #endregion


        RunInfo runInfo;





        #region Manager 
        class LongThreadManager
        {
            /// <summary>
            /// 等待中的线程个数
            /// </summary>
            public int waitingThreadCount = 0;

            public LongThread[] threads;
            ManagedThread<T> task;
            public LongThreadManager(ManagedThread<T> task)
            {
                this.task = task;
            }

            public void Start()
            {
                threads = new LongThread[task.threadCount];
                for (int i = 0; i < threads.Length; i++)
                {
                    threads[i] = new LongThread(task, i);
                }
            }

            /// <summary>
            /// 电子脉冲，在固定的时间间隔发送脉冲
            /// </summary>
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public void Pulse()
            {
                foreach (var thread in threads)
                {
                    thread?.Pulse();
                }
            }
        }
        class CasualThreadManager
        {
            public readonly ConcurrentDictionary<int, CasualThread> threadMap = new ConcurrentDictionary<int, CasualThread>();

            ManagedThread<T> task;

            public int runningThreadCount = 0;

            public int curThreadCount = 0;

            int maxThreadCount;
            public CasualThreadManager(ManagedThread<T> task)
            {
                this.task = task;
                maxThreadCount = task.maxThreadCount - task.threadCount;
            }


            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public bool TryStartNewThread(T arg)
            {
                var curThreadCount = Interlocked.Increment(ref this.curThreadCount);
                try
                {
                    if (curThreadCount <= maxThreadCount)
                    {
                        new CasualThread(task, arg);
                        return true;
                    }
                }
                catch
                {
                    Interlocked.Decrement(ref this.curThreadCount);
                    throw;
                }

                Interlocked.Decrement(ref this.curThreadCount);
                return false;
            }


            /// <summary>
            /// 电子脉冲，在固定的时间间隔发送脉冲
            /// </summary>
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public void Pulse()
            {
                foreach (var thread in threadMap.Values)
                {
                    thread?.Pulse();
                }
            }
        }

        #endregion

        #region Thread




        class LongThread
        {
            RunInfo runInfo;

            LongThreadManager manager;

            bool isProcessing = false;

            int pulseCount = 0;

            Thread thread;


            public LongThread(ManagedThread<T> task, int index)
            {
                this.runInfo = task.runInfo;
                this.manager = runInfo.longThreadManager;

                thread = new Thread(Run);
                thread.IsBackground = true;
                thread.Name = task.threadName + "-LongThread-" + index + "-" + thread.ManagedThreadId;

                thread.Start();
            }



            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Pulse()
            {
                if (thread == null || !thread.IsAlive || !isProcessing)
                    return;
                try
                {
                    Interlocked.Increment(ref pulseCount);
                    if (pulseCount >= 2)
                        thread.Interrupt();
                }
                catch { }
            }



            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void Run()
            {
                try
                {
                    T arg_;
                    while (runInfo.needRunning)
                    {
                        try
                        {
                            isProcessing = false;
                            Interlocked.Increment(ref manager.waitingThreadCount);
                            arg_ = runInfo.pendingQueue.Take();
                            Interlocked.Decrement(ref manager.waitingThreadCount);
                            pulseCount = 0;
                            isProcessing = true;

                            #region processing 
                            ETaskFinishStatus status = ETaskFinishStatus.error;
                            try
                            {
                                runInfo.Processor(arg_);
                                status = ETaskFinishStatus.success;
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
                                runInfo.OnFinish?.Invoke(status, arg_);
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
                    thread = null;
                }
            }
        }

        class CasualThread
        {
            RunInfo runInfo;

            int pulseCount = 0;

            T arg;
            Thread thread;

            CasualThreadManager manager;


            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            public CasualThread(ManagedThread<T> task, T arg)
            {
                this.runInfo = task.runInfo;
                this.manager = runInfo.casualThreadManager;
                this.arg = arg;

                thread = new Thread(Run);
                thread.IsBackground = true;
                thread.Name = task.threadName + "-Casual-" + thread.ManagedThreadId;
                thread.Start();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Pulse()
            {
                if (!thread.IsAlive)
                {
                    manager.threadMap.TryRemove(this.GetHashCode(), out _);
                    return;
                }

                try
                {
                    Interlocked.Increment(ref pulseCount);
                    if (pulseCount >= 2)
                        thread.Interrupt();
                }
                catch { }
            }



            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void Run()
            {
                Interlocked.Increment(ref manager.runningThreadCount);
                try
                {
                    manager.threadMap.TryAdd(this.GetHashCode(), this);

                    T arg_;
                    while (runInfo.needRunning)
                    {
                        try
                        {
                            pulseCount = 0;
                            if (arg == null)
                            {
                                if (!runInfo.pendingQueue.TryTake(out arg_))
                                    return;
                            }
                            else
                            {
                                arg_ = arg;
                                arg = default;
                            }

                            #region processing
                            ETaskFinishStatus status = ETaskFinishStatus.error;
                            try
                            {
                                runInfo.Processor(arg_);
                                status = ETaskFinishStatus.success;
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
                                runInfo.OnFinish?.Invoke(status, arg_);
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
                    Interlocked.Decrement(ref manager.runningThreadCount);
                    Interlocked.Decrement(ref manager.curThreadCount);
                    manager.threadMap.TryRemove(this.GetHashCode(), out _);
                }

            }
        }


        #endregion
    }
}
