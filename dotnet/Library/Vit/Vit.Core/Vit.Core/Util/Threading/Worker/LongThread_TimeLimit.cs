using System;
using System.Threading;

using Vit.Core.Module.Log;
using Vit.Core.Util.Threading.Timer;
using Vit.Extensions;

namespace Vit.Core.Util.Threading.Worker
{
    /// <summary>
    /// 注意！！！慎用！！！
    /// 请勿处理ThreadInterruptedException异常，否则导致线程无法正常结束
    /// 若在超时时间内未清理状态，则强制关闭任务。拉任务的模式。
    /// </summary>
    public class LongThread_TimeLimit<T> : IDisposable
    {

        /// <summary>
        /// 不可抛异常
        /// </summary>
        public Func<T> GetWork;

        /// <summary>
        /// 不可抛异常
        /// </summary>
        public Action<T> Processor;

        /// <summary>
        /// 不可抛异常
        /// status: success/error/timeout
        /// </summary>
        public Action<ETaskFinishStatus, T> OnFinish;



        /// <summary>
        /// 线程名称
        /// </summary>
        public string threadName;

        private int _threadCount;
        /// <summary>
        /// 线程数，默认1
        /// </summary>
        public int threadCount
        {
            get => _threadCount;
            set
            {
                if (IsRunning) throw WorkerHelp.Error_CannotChangeThreadCountWhileRunning.ToException();
                _threadCount = value;
                if (_threadCount > 0)
                    semaphore = new Semaphore(_threadCount, _threadCount);
                else
                {
                    semaphore = null;
                }
            }
        }



        /// <summary>
        /// 是否在执行完成后重新执行(默认true)
        /// </summary>
        public bool loop = true;
        /// <summary>
        /// 出现异常时是否终止(默认false)
        /// </summary>
        public bool stopWhenException = false;

        Worker[] workers;

        Semaphore semaphore;

        int runningThreadCount = 0;
        public int RunningThreadCount => runningThreadCount;

        public bool IsRunning => runningThreadCount != 0;//threads != null && threads.Any(item=> item.IsAlive);

        bool NeedRunning { get; set; } = false;

        /// <summary>
        /// 超时时间。脉冲间隔。（主动关闭超过此时间的任务,实际任务强制关闭的时间会在1倍超时时间到2倍超时时间内)。单位：ms。(默认300000)
        /// </summary>
        public int timeoutMs { get => pulseMaker.intervalMs; set => pulseMaker.intervalMs = value; }


        public LongThread_TimeLimit()
        {
            pulseMaker = new VitTimer { intervalMs = 300000, timerCallback = PulseMake };

            threadCount = 1;
        }
        ~LongThread_TimeLimit()
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
        public void PulseMake(object obj)
        {
            if (!IsRunning) return;
            if (workers == null) return;

            foreach (var worker in workers)
            {
                worker.Pulse();
            }
        }
        #endregion





        public void Start()
        {
            if (IsRunning)
            {
                throw WorkerHelp.Error_CannotStartWhileRunning.ToException();
            }
            if (threadCount <= 0)
            {
                workers = null;
                return;
            }
            NeedRunning = true;
            workers = new Worker[threadCount];
            for (int i = 0; i < threadCount; i++)
            {
                var worker = workers[i] = new Worker() { GetWork = GetWork, Processor = Processor, OnFinish = OnFinish };
                worker.Start(threadName + "-" + i, this);
            }

            //(x.2)开启脉冲生产器
            pulseMaker.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void Stop()
        {
            if (!IsRunning) return;

            NeedRunning = false;

            if (null != workers)
            {
                foreach (var threadItem in workers)
                {
                    threadItem.TryStop();
                }
            }

            try
            {
                pulseMaker.Stop();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            //if (semaphore.WaitOne(1000))
            //{
            //    semaphore.Release();
            //}

        }


        class Worker
        {
            /*
                流程为：    获取任务GetWork、执行任务Processor、任务结束后回调OnFinish
            */

            public T workArg;

            /// <summary>
            /// 任务是否在执行中
            /// </summary>
            public bool IsDealing { get; private set; } = false;
            /// <summary>
            /// 阻塞获取任务
            /// </summary>
            public Func<T> GetWork;
            /// <summary>
            /// 不可抛异常
            /// </summary>
            public Action<T> Processor;

            /// <summary>
            /// 不可抛异常
            /// status: success/error/timeout
            /// </summary>
            public Action<ETaskFinishStatus, T> OnFinish;


            /// <summary>
            /// 当前任务所经历的脉冲的次数
            /// </summary>
            int pulseCount = 0;

            LongThread_TimeLimit<T> task;
            Thread thread;

            internal void Start(string threadName, LongThread_TimeLimit<T> task)
            {
                this.task = task;

                thread = new Thread(Run);
                thread.IsBackground = true;
                thread.Name = threadName;
                thread.Start();
            }

            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
            void Run()
            {
                try
                {
                    task.semaphore.WaitOne();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    return;
                    //throw;
                }

                Interlocked.Increment(ref task.runningThreadCount);
                try
                {
                    do
                    {
                        try
                        {
                            ETaskFinishStatus status = ETaskFinishStatus.error;
                            try
                            {
                                IsDealing = false;

                                //(x.x.1)
                                workArg = GetWork();

                                //(x.x.2)
                                pulseCount = 0;
                                IsDealing = true;

                                //(x.x.3)
                                Processor(workArg);
                                status = ETaskFinishStatus.success;
                                IsDealing = false;
                            }
                            catch (Exception ex) when (ex.GetBaseException() is ThreadInterruptedException)
                            {
                                IsDealing = false;
                                status = ETaskFinishStatus.timeout;

                            }
                            catch (Exception ex)
                            {
                                IsDealing = false;
                                status = ETaskFinishStatus.error;

                                Logger.Error(ex);
                                if (task.stopWhenException)
                                {
                                    return;
                                }
                            }
                            finally
                            {
                                try
                                {
                                    OnFinish?.Invoke(status, workArg);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex);
                        }
                    } while (task.loop && task.NeedRunning);
                }
                finally
                {
                    Interlocked.Decrement(ref task.runningThreadCount);
                    task.semaphore.Release();
                }
            }

            public void Pulse()
            {
                try
                {
                    Interlocked.Increment(ref pulseCount);
                    if (IsDealing && pulseCount >= 2)
                    {
                        TryStop();
                    }
                }
                catch { }
            }

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


        }

    }
}
