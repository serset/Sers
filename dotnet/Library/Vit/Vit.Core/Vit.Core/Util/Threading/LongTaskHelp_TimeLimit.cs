using System;
using System.Threading;
using Vit.Extensions;
using Vit.Core.Module.Log;

namespace Vit.Core.Util.Threading
{
    /// <summary>
    /// 注意！！！慎用！！！
    /// 请勿处理ThreadInterruptedException异常，否则导致线程无法正常结束
    /// 若在超时时间内未清理状态，则强制关闭任务
    /// </summary>
    public class LongTaskHelp_TimeLimit:IDisposable
    {
        public LongTaskHelp_TimeLimit()
        {
            threadCount = 1;
        }
        ~LongTaskHelp_TimeLimit()
        {
            Dispose();
        }

        public virtual void Dispose()
        {
            Stop();
        }


        private int _threadCount;
        /// <summary>
        /// 线程数，默认1
        /// </summary>
        public int threadCount
        {
            get => _threadCount;
            set
            {
                if (IsRunning) throw LongTaskHelp.Error_CannotChangeThreadCountWhileRunning.ToException();
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

        Worker[]threads;

        Semaphore semaphore;
        int runningThreadCount = 0;
        public int RunningThreadCount => runningThreadCount;

        public bool IsRunning => runningThreadCount!=0;//threads != null && threads.Any(item=> item.IsAlive);



        /// <summary>
        /// 超时时间。脉冲间隔。（主动关闭超过此时间的任务,实际任务强制关闭的时间会在1倍超时时间到2倍超时时间内)。单位：ms。(默认300000)
        /// </summary>
        public int timeout_ms = 300000;

        #region 电子脉冲
        /// <summary>
        /// 脉冲生产器
        /// </summary>
        SersTimer pulseMaker;

        /// <summary>
        /// 电子脉冲，在固定的时间间隔发送脉冲
        /// </summary>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void PulseMake(object obj)
        {
            if (!IsRunning) return;
            if (threads == null) return;

            foreach (var worker in threads)
            {    
                Interlocked.Increment(ref worker.pulseCount);             

                if (worker.isDealing && worker.pulseCount >= 2)
                {
                    worker.TryStop();
                }
            }
        }
        #endregion



 
    
         

        /// <summary>
        /// 线程名称
        /// </summary>
        public string threadName;
     
        public void Start(Action<Worker> GetWork, Action<Worker> DealWork, Action<Worker> OnFinish, Action<Worker> OnTimeout)
        {
            if (IsRunning)
            {
                throw LongTaskHelp.Error_CannotStartWhileRunning.ToException();
            }
            if (threadCount <=0)
            {
                threads = null;
                return;
            }

            threads = new Worker[threadCount];
            for (int i = 0; i < threadCount; i++)
            {
                var worker = threads[i] = new Worker() { GetWork = GetWork , DealWork = DealWork, OnFinish = OnFinish, OnTimeout = OnTimeout };              
                worker.Start(threadName + "-" + i,this);               
            }

            //(x.2)开启脉冲生产器
            pulseMaker = new SersTimer { intervalMs= timeout_ms,timerCallback=PulseMake };
            pulseMaker.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void Stop()
        {
            if (!IsRunning) return;

            if (pulseMaker != null)
            {
                try
                {
                    pulseMaker.Stop();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }               
                pulseMaker = null;
            }

            if (null != threads)
            {                
                foreach (var threadItem in threads)
                {
                    threadItem.TryStop();
                }               
            }

            //if (semaphore.WaitOne(1000))
            //{
            //    semaphore.Release();
            //}
           
        }


        public class Worker
        {
            /*
             流程为：    获取任务GetWork、执行任务DealWork、任务结束后回调OnFinish、任务超时回调（若超时）OnTimeout
                 
                 
                 */

            public object workArg;
            public object workArg2;
            public object workArg3;
            public object workArg4;


            /// <summary>
            /// 任务是否在执行中
            /// </summary>
            public bool isDealing { get; private set; } = false;
            /// <summary>
            /// 阻塞获取任务
            /// </summary>
            public Action<Worker> GetWork;
            /// <summary>
            /// 不可抛异常
            /// </summary>
            public Action<Worker> DealWork;
            /// <summary>
            /// 不可抛异常
            /// </summary>
            public Action<Worker> OnFinish;
            /// <summary>
            /// 不可抛异常
            /// </summary>
            public Action<Worker> OnTimeout;


            /// <summary>
            /// 当前任务所经历的脉冲的次数
            /// </summary>
            internal int pulseCount = 0;

            LongTaskHelp_TimeLimit task;
            Thread thread;
            internal void Start(string threadName, LongTaskHelp_TimeLimit task)
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
                            //(x.x.1)
                            GetWork(this);

                            //(x.x.2)
                            pulseCount = 0;
                            isDealing = true;

                            //(x.x.3)
                            DealWork(this);
                            isDealing = false;

                            //(x.x.4)
                            OnFinish(this);
                        }
                        catch (Exception ex) when (ex.GetBaseException() is ThreadInterruptedException)
                        {

                            try
                            {
                                isDealing = false;
                                OnTimeout(this);
                            }
                            catch (Exception ex1)
                            {
                                Logger.Error(ex1);
                                if (task.stopWhenException)
                                {
                                    return;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            isDealing = false;
                            Logger.Error(ex);
                            if (task.stopWhenException)
                            {
                                return;
                            }
                        }
                        finally 
                        {
                            workArg = null;                           
                            workArg2 = null;
                            workArg3 = null;
                            workArg4 = null;
                        }

                    } while (task.loop);
                }
                finally
                {
                    Interlocked.Decrement(ref task.runningThreadCount);
                    task.semaphore.Release();
                }
            }



            internal bool TryStop()
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
