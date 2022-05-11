using Vit.Core.Module.Log;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Vit.Core.Util.Threading.Worker
{
    /// <summary>
    /// 单工作者的任务队列。入队操作为线程安全的。
    /// </summary>
    public class TaskQueue
    {

        LongThread taskToInvokeTask;

        BlockingCollection<Action> taskQueue = new BlockingCollection<Action>();


        /// <summary>
        /// 线程名称
        /// </summary>
        public string threadName { get => taskToInvokeTask.threadName; set => taskToInvokeTask.threadName = value; }
        public bool IsRunning => taskToInvokeTask.IsRunning;
        public int threadCount { get => taskToInvokeTask.threadCount; set => taskToInvokeTask.threadCount = value; }

        public TaskQueue(int threadCount = 1)
        {
            taskToInvokeTask = new LongThread() { threadName = nameof(TaskQueue), threadCount = threadCount };
        }


        #region Start Stop

        public bool Start()
        {
            if (IsRunning) return false;
            try
            {
                taskToInvokeTask.Processor = InvokeTaskInQueue;
                taskToInvokeTask.Start();
                Logger.Info("[" + nameof(TaskQueue) + "] Thread Started", taskToInvokeTask.threadName);

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return false;
        }

        public bool Stop()
        {
            if (!IsRunning) return false;
            try
            {
                taskToInvokeTask.Stop();
                Logger.Info("[" + nameof(TaskQueue) + "] Thread Stoped", taskToInvokeTask.threadName);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return false;
        }
        #endregion


        public void AddTask(Action task)
        {
            taskQueue.Add(task);
        }

        #region InvokeTaskInQueue

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        void InvokeTaskInQueue()
        {
            while (true)
            {
                try
                {
                    #region ThreadToDealMsg                        
                    while (true)
                    {
                        //堵塞获取请求           
                        taskQueue.Take()?.Invoke();
                    }
                    #endregion
                }
                catch (Exception ex) when (!(ex.GetBaseException() is ThreadInterruptedException))
                {
                    Logger.Error(ex);
                }
            }
        }
        #endregion



    }
}
