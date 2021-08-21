using System;
using System.Threading;
using Vit.Extensions;
using Vit.Core.Module.Log;
using System.Runtime.CompilerServices;

namespace Vit.Core.Util.Threading.Worker
{
    /// <summary>
    /// 注意！！！慎用！！！
    /// 请勿处理ThreadInterruptedException异常，否则导致线程无法正常结束
    /// catch (Exception ex) when (!(ex.GetBaseException() is ThreadInterruptedException))
    /// </summary>
    public class LongThread : IDisposable
    {

        /// <summary>
        /// 请勿处理ThreadInterruptedException异常，否则导致线程无法正常结束
        /// 不可抛异常
        /// </summary>
        public Action Processor;


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
        /// 是否在执行完成后重新执行。
        /// </summary>
        public bool loop = false;
        /// <summary>
        /// 出现异常时是否终止
        /// </summary>
        public bool stopWhenException = false;

        Thread[] threads;

        Semaphore semaphore;

        int runningThreadCount = 0;
        public int RunningThreadCount => runningThreadCount;

        public bool IsRunning => runningThreadCount != 0;//threads != null && threads.Any(item=> item.IsAlive);


        public LongThread()
        {
            threadCount = 1;
        }
        ~LongThread()
        {
            Dispose();
        }

        public virtual void Dispose()
        {
            Stop();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Run()
        {
            try
            {
                semaphore.WaitOne();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
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
                        Processor?.Invoke();
                    }
                    catch (Exception ex) when (ex.GetBaseException() is ThreadInterruptedException)
                    {
                        return;
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
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
      


        public void Start()
        {
            if (IsRunning)
            {
                throw WorkerHelp.Error_CannotStartWhileRunning.ToException();
            }
            if (threadCount <= 0)
            {
                threads = null;
                return;
            }

            threads = new Thread[threadCount];
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
                        if (thread.IsAlive)
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
