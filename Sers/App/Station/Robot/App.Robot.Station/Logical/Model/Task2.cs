using Newtonsoft.Json;
using Sers.Core.Module.Api;
using Sers.Core.Module.Log;
using Sers.Core.Util.Threading;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Sers.Core.Extensions;
using App.Robot.ServiceStation.Logical;
using Sers.Core.Module.Api.Data;

namespace App.Robot.ServiceStation.Logicaldd
{
 
    public class Task
    {
        public string name => config.name;
        public int id;

        [JsonIgnore]
        LongTaskHelp tasks = new LongTaskHelp();


 
        public bool IsRunning => tasks.IsRunning;
 
        public int targetCount => config.threadCount*config.loopCountPerThread;

 

        public int curCount=0;
        public int failCount=0;

        public TaskConfig config;
        public Task(TaskConfig config)
        {
            this.config = config;

            tasks.threadCount = config.threadCount;

            tasks.action = Action;
        }
 
        void Action()
        {            
            for (int i = config.loopCountPerThread; i >0; i--)
            {
                try
                {
                    Interlocked.Increment(ref curCount);
                    var ret = ApiClient.CallRemoteApi<ApiReturn>(config.apiRoute, config.apiArg);                    
                    if (!ret.success)
                    {
                        Logger.log.Error("失败：ret:"+ret.Serialize());
                        Interlocked.Increment(ref failCount);
                    }
                }
                catch (Exception ex) when (!(ex is ThreadInterruptedException))
                { 
                    Interlocked.Increment(ref failCount);
                    Logger.log.Error(ex);
                }
            }
        }

        public void Start()
        {
            tasks.Stop();

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
