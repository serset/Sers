using System;
using System.Threading;
using System.Linq;
using Sers.Core.Extensions;
using System.Threading.Tasks;
using Sers.Core.Module.Log;

namespace Sers.Core.Util.Threading
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


        public int threadCount=1;
        public int repeatCount=1;


        /// <summary>
        /// 
        /// </summary>
        public Action action;

        /// <summary>
        /// 出现异常时是否终止
        /// </summary>
        public bool stopWhenException = true;

        /// <summary>
        /// 是否发送结束信号
        /// </summary>
        public bool sendStopSignal = false;

        public int RunningThreadCount => runningThreadCount;

        int runningThreadCount = 0;

        public bool IsRunning => runningThreadCount != 0;//threads != null && threads.Any(item=> item.IsAlive);

        void Run()
        {
            Interlocked.Increment(ref runningThreadCount);
            try
            {
                
                for (int finishedCount = 0; !sendStopSignal && finishedCount < repeatCount; finishedCount++)
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
            finally
            {
                Interlocked.Decrement(ref runningThreadCount);                
            }
        }


        public void Start()
        {
            if (IsRunning)
            {
                throw new Exception().SsError_Set(LongTaskHelp.Error_CannotStartWhileRunning);
            }

            sendStopSignal = false;

            for (int i = 0; i < threadCount; i++)
            {
                Task.Run((Action)Run);
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
