using Newtonsoft.Json;
using Sers.Core.Module.Api;
using Sers.Core.Module.Log;
using Sers.Core.Util.Threading;
using System;
using System.Threading;
using Sers.Core.Extensions;
using FrameWork.Net;
using Sers.Core.Module.Api.Data;

namespace App.Robot.ServiceStation.Logical
{

    public class Task
    {
        public string name => config.name;
        public int id;

        [JsonIgnore]
        RepeatTaskHelp tasks = new RepeatTaskHelp();

        public int RunningThreadCount => tasks.RunningThreadCount;


        public bool IsRunning => tasks.IsRunning;
 
        public int targetCount => config.threadCount*config.loopCountPerThread;

        public int sumCount= 0;
        public int sumFailCount = 0;

        public int curCount=0;
        public int failCount=0;
        public TaskConfig config;

        void StepUp(bool success)
        {
            Interlocked.Increment(ref curCount);
            Interlocked.Increment(ref sumCount);
            if (!success)
            {
                Interlocked.Increment(ref sumFailCount);
                Interlocked.Increment(ref failCount);
            }

        }

      
        public Task(TaskConfig config)
        {
            this.config = config;

            tasks.threadCount = config.threadCount;
            tasks.repeatCount = config.loopCountPerThread;

            if (config.apiRoute.StartsWith("http"))
            {
                http = new HttpUtil();
                http_ReqParam = new RequestParam
                {
                    url = config.apiRoute,
                    body = config.apiArg,
                    Method = "POST"
                };

                tasks.action = ActionHttp;
            }
            else
            {
                tasks.action = ActionApiClient;
            }           
        }

        HttpUtil http;
        RequestParam http_ReqParam;
        void ActionHttp()
        {
            bool success = false;
            try
            {            
                var ret = http.Ajax<ApiReturn>(http_ReqParam);
                if (ret.success)
                {
                    success = true;
                }
                else
                {
                    if (config.logError)
                        Logger.Info("失败：ret:" + ret.Serialize());
                }
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref failCount);
                Logger.Error(ex);
            }
            StepUp(success);
            Thread.Sleep(config.interval);
        }


        void ActionApiClient()
        {
            bool success = false;
            try
            {
                
                var ret = ApiClient.CallRemoteApi<ApiReturn>(config.apiRoute, config.apiArg);
                if (ret.success)
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
            Thread.Sleep(config.interval);
        }

        public void Start()
        {            
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
