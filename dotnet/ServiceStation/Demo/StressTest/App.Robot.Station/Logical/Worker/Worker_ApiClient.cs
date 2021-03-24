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
        public Worker_ApiClient(TaskConfig config)
        {
            this.config = config;

            tasks.threadCount = config.threadCount;
            tasks.repeatCountPerThread = config.loopCountPerThread;

            tasks.action = Processor;
        }


        public string name => config.name;
        public int id { get; set; }

        [JsonIgnore]
        RepeatTaskHelp tasks = new RepeatTaskHelp();

        public int RunningThreadCount => tasks.RunningThreadCount;


        public bool IsRunning => tasks.IsRunning;
 
        public long targetCount => config.threadCount * config.loopCountPerThread;

        public long sumCount = 0;
        public long sumFailCount = 0;

        public long curCount =0;
        public long failCount =0;
        public TaskConfig config { get; set; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void StepUp(bool success)
        {
            Interlocked.Increment(ref curCount);
            Interlocked.Increment(ref sumCount);
            if (!success)
            {
                Interlocked.Increment(ref sumFailCount);
                Interlocked.Increment(ref failCount);
            }

        }

      
    


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual void Processor()
        {
            bool success = false;
            try
            {
                
                var ret = ApiClient.CallRemoteApi<ApiReturn>(config.apiRoute, config.apiArg, config.httpMethod);
                if (ret == null || ret.success)
                {
                    success = true;
                }
                else
                {
                    if(config.logError)
                    Logger.Info("失败：ret:" + ret.Serialize());
                }
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref failCount);
                Logger.Error(ex);
            }
            StepUp(success);
            if(config.interval>0)
                Thread.Sleep(config.interval);
        }

        public void Start()
        {
            tasks.threadName = "Robot-"+ config.name;

            curCount = 0;
            failCount = 0;
            tasks.Start();
        }

        public void Stop()
        {
            tasks.Stop();
        }

    }
}
