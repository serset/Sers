using System.Runtime.CompilerServices;
using System.Threading;
using Newtonsoft.Json;
using Sers.Core.Module.Api;
using Vit.Core.Module.Log;
using Vit.Core.Util.ComponentModel.Data;
using Vit.Extensions;

namespace App.Robot.Station.Logical.Worker
{


    public class Worker_ApiClientAsync: IWorker
    {
        [JsonIgnore]
        protected TaskItem taskItem;

        public Worker_ApiClientAsync(TaskItem taskItem)
        {
            this.taskItem = taskItem;

            interval = taskItem.config.interval;
            logError = taskItem.config.logError;
            apiRoute = taskItem.config.apiRoute;
            apiArg = taskItem.config.apiArg;      
            httpMethod = taskItem.config.httpMethod;
        }


        public bool IsRunning => runningThreadCount > 0;
        public int RunningThreadCount => runningThreadCount;


        int runningThreadCount = 0;
        bool needRunning = false;


        int interval;
        bool logError;
        string apiRoute;
        string apiArg;
        string httpMethod;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected  void CallApi()
        {
            ApiClient.CallRemoteApiAsync<ApiReturn>((ret) =>
            {
                bool success = false;
                if (ret == null || ret.success)
                {
                    success = true;
                }
                else
                {
                    if (logError)
                        Logger.Info("失败：ret:" + ret.Serialize());
                }

                taskItem.StepUp(success);

                if (taskItem.sumCount >= taskItem.targetCount)
                {
                    needRunning = false;
                }


                if (interval > 0)
                    Thread.Sleep(interval);

                if (needRunning) CallApi();
                else
                {
                    Interlocked.Decrement(ref runningThreadCount);
                }

            }, apiRoute,apiArg, httpMethod);      
          
        }

        public void Start()
        {
            if (needRunning) return;

            taskItem.curCount = 0;
            taskItem.failCount = 0;

            needRunning = true;

            for (var t = 0; t < taskItem.config.threadCount; t++) 
            {
                Interlocked.Increment(ref runningThreadCount);
                CallApi();
            }
        }

        public void Stop()
        {
            needRunning = false;
        }

    }
}
