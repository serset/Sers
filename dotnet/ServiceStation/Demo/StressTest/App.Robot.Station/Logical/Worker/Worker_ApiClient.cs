using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Newtonsoft.Json;
using Sers.Core.Module.Api;
using Vit.Core.Module.Log;
using Vit.Core.Util.ComponentModel.Data;
using Vit.Core.Util.Threading;
using Vit.Extensions;

namespace App.Robot.Station.Logical.Worker
{


    public class Worker_ApiClient: IWorker
    {
        [JsonIgnore]
        protected TaskItem taskItem;

        public Worker_ApiClient(TaskItem taskItem)
        {
            this.taskItem = taskItem;
 

            tasks.threadCount = taskItem.config.threadCount;
            tasks.repeatCountPerThread = taskItem.config.loopCountPerThread;

            tasks.action = Processor;
        }


  



        [JsonIgnore]
        RepeatTaskHelp tasks = new RepeatTaskHelp();

        public int RunningThreadCount => tasks.RunningThreadCount;


        public bool IsRunning => tasks.IsRunning;
 
  
    


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void Processor()
        {
            bool success = false;
            try
            {
                
                var ret = ApiClient.CallRemoteApi<ApiReturn>(taskItem.config.apiRoute, taskItem.config.apiArg, taskItem.config.httpMethod);
                if (ret == null || ret.success)
                {
                    success = true;
                }
                else
                {
                    if(taskItem.config.logError)
                    Logger.Info("失败：ret:" + ret.Serialize());
                }
            }
            catch (Exception ex)
            {               
                Logger.Error(ex);
            }

            taskItem.StepUp(success);

            if (taskItem.config.interval > 0)
                Thread.Sleep(taskItem.config.interval);
        }

        public void Start()
        {
            tasks.threadName = "Robot-"+ taskItem.config.name;

            taskItem.curCount = 0;
            taskItem.failCount = 0;
            tasks.Start();
        }

        public void Stop()
        {
            tasks.Stop();
        }

    }
}
