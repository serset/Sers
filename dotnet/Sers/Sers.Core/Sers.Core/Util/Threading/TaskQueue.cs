using Sers.Core.Module.Log;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Sers.Core.Util.Threading
{
    public class TaskQueue
    {

        #region 后台服务       

        public void AddTask(Action task)
        {
            taskQueue.Add(task);
        }

        /// <summary>
        /// 线程名称
        /// </summary>
        public string threadName{ set { taskToInvokeTask.threadName = value;  } }


        #region Start Stop

        public bool Start()
        {
            try
            {
                taskToInvokeTask.action = InvokeTaskInQueue;
                taskToInvokeTask.Start();
                Logger.Info("[TaskQueue]"+ taskToInvokeTask.threadName + " Started");

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
        }

        public void Stop()
        {
            try
            {
                taskToInvokeTask.Stop();
                Logger.Info("[TaskQueue]" + taskToInvokeTask.threadName + " Stoped");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
        #endregion


        #region  后台调用Api线程 taskToCallApi

        LongTaskHelp taskToInvokeTask = new LongTaskHelp() { threadName="TaskQueue",threadCount=1};
        BlockingCollection<Action> taskQueue = new BlockingCollection<Action>();




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
                catch (Exception ex) when (!(ex is ThreadInterruptedException))
                {
                    Logger.Error(ex);
                }
            }
        }
        #endregion

        #endregion

    }
}
