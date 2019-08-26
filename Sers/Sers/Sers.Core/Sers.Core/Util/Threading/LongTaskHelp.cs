using System;
using System.Threading;
using System.Linq;
using Sers.Core.Extensions;
using Sers.Core.Module.Log;

namespace Sers.Core.Util.Threading
{
    /// <summary>
    /// 注意！！！慎用！！！
    /// 请勿处理ThreadInterruptedException异常，否则导致线程无法正常结束
    /// </summary>
    public class LongTaskHelp:IDisposable
    {
        public LongTaskHelp()
        {
            threadCount = 1;
        }
        ~LongTaskHelp()
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
                if (IsRunning) throw new Exception().SsError_Set(Error_CannotChangeThreadCountWhileRunning);
                _threadCount = value;
                semaphore = new Semaphore(_threadCount, _threadCount);
            }
        }

        /// <summary>
        /// 请勿处理ThreadInterruptedException异常，否则导致线程无法正常结束
        /// </summary>
        public Action action;
        
        /// <summary>
        /// 是否在执行完成后重新执行。
        /// </summary>
        public bool loop = false;
        /// <summary>
        /// 出现异常时是否终止
        /// </summary>
        public bool stopWhenException = false;

        Thread []threads;

        Semaphore semaphore;
        int runningThreadCount = 0;
        public int RunningThreadCount => runningThreadCount;

        public bool IsRunning => runningThreadCount!=0;//threads != null && threads.Any(item=> item.IsAlive);

        void Run()
        {
            try
            {
                semaphore.WaitOne();
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex);
                return;
                //throw;
            }
           
            Interlocked.Increment(ref runningThreadCount);
            try
            {
                do
                {
                    try
                    {
                        action?.Invoke();
                    }
                    //catch (Exception ex) when (!(ex is ThreadInterruptedException))
                    //{
                    //}
                    catch (ThreadInterruptedException ex)
                    {
                        return;
                    }
                    catch (Exception ex)
                    {
                        Logger.log.Error(ex);
                        if (stopWhenException)
                        {
                            return;
                        }
                    }

                } while (loop);
            }
            finally
            {
                Interlocked.Decrement(ref runningThreadCount);
                semaphore.Release();
            }
        }
        internal static SsError.SsError Error_CannotChangeThreadCountWhileRunning => new SsError.SsError { errorMessage="can't change threadCount while tasks is running.",errorTag="lith_190223_01"
        };
        internal static SsError.SsError Error_CannotStartWhileRunning => new SsError.SsError
        {
            errorMessage = "can't  start while task is running.",
            errorTag = "lith_190223_02"
        };

     
        public void Start()
        {
            if (IsRunning)
            {
                throw new Exception().SsError_Set(LongTaskHelp.Error_CannotStartWhileRunning);
            }

            threads = new Thread[threadCount];
            for (int i = 0; i < threadCount; i++)
            {
                var thread = new Thread(Run);
                thread.IsBackground = true;
                thread.Start();
                threads[i] = thread;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void Stop()
        {
            if (!IsRunning) return;

            if (null != threads)
            {                
                foreach (var thread in threads)
                {
                    try
                    {
                        if(thread.IsAlive)
                            thread.Interrupt();
                    }
                    catch { }                   
                }               
            }

            //if (semaphore.WaitOne(1000))
            //{
            //    semaphore.Release();
            //}
           
        }
    }
}
