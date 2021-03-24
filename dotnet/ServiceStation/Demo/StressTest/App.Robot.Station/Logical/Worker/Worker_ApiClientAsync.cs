using System.Runtime.CompilerServices;
using System.Threading;
using Sers.Core.Module.Api;
using Vit.Core.Module.Log;
using Vit.Core.Util.ComponentModel.Data;
using Vit.Extensions;

namespace App.Robot.Station.Logical.Worker
{


    public class Worker_ApiClientAsync: IWorker
    {
        public Worker_ApiClientAsync(TaskConfig config)
        {
            this.config = config;

            targetCount= config.threadCount * config.loopCountPerThread;
        }


        public string name => config.name;
        public int id { get; set; }



        public long RunningThreadCount = 0;


        bool needRunning = false;

        public bool IsRunning => RunningThreadCount>0;
 
        public long targetCount;

        public long sumCount = 0;
        public long sumFailCount = 0;

        public long curCount =0;
        public long failCount =0;
        public TaskConfig config { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void StepUp(bool success)
        {          
            Interlocked.Increment(ref curCount);
            if (Interlocked.Increment(ref sumCount) >= targetCount) 
            {
                needRunning = false;
            }
            if (!success)
            {
                Interlocked.Increment(ref sumFailCount);
                Interlocked.Increment(ref failCount);
            }         
        }

      
    


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
                    if (config.logError)
                        Logger.Info("失败：ret:" + ret.Serialize());
                }

                StepUp(success);

                if (config.interval > 0)
                    Thread.Sleep(config.interval);

                if (needRunning) CallApi();
                else
                {
                    Interlocked.Decrement(ref RunningThreadCount);
                }

            },config.apiRoute, config.apiArg, config.httpMethod);      
          
        }

        public void Start()
        {
            if (needRunning) return;

            curCount = 0;
            failCount = 0;
            needRunning = true;

            for (var t = 0; t < config.threadCount; t++) 
            {
                Interlocked.Increment(ref RunningThreadCount);
                CallApi();
            }
        }

        public void Stop()
        {
            needRunning = false;
        }

    }
}
