using System;
using System.Threading;
using Newtonsoft.Json;
using Sers.Core.Module.Api;
using Vit.Core.Module.Log;
using Vit.Core.Util.ComponentModel.Data;
using Vit.Core.Util.Net;
using Vit.Core.Util.Threading;
using Vit.Extensions;

namespace App.Robot.Station.Logical.Model
{

    /// <summary>
    /// RepeatTaskHelp
    /// </summary>
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
            tasks.repeatCountPerThread = config.loopCountPerThread;

            if (config.apiRoute.StartsWith("http"))
            {
                if (config.httpUseHttpUtil)
                {
                    httpClient = new HttpClient();
                    httpClient_ReqParam = new HttpRequest
                    {
                        url = config.apiRoute,
                        body = config.apiArg,
                        httpMethod = config.httpMethod
                    };

                    tasks.action = ActionHttpClient;
                }
                else
                {

                    httpUtil = new HttpUtil();
                    httpUtil_Request = new RequestParam
                    {
                        url = config.apiRoute,
                        body = config.apiArg,
                        Method = config.httpMethod
                    };
                    tasks.action = ActionHttpUtil;
                }
               
            }
            else
            {
                tasks.action = ActionApiClient;
            }           
        }



        #region ActionHttpClient       
        HttpClient httpClient;
        HttpRequest httpClient_ReqParam;
        void ActionHttpClient()
        {
            bool success = false;
            try
            {    

                var ret = httpClient.Send<ApiReturn>(httpClient_ReqParam)?.data;
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
        #endregion


        #region ActionHttpUtil

        HttpUtil httpUtil;
        RequestParam httpUtil_Request;
        void ActionHttpUtil()
        {
            bool success = false;
            try
            {
                var ret = httpUtil.Ajax<ApiReturn>(httpUtil_Request);
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
        #endregion


        void ActionApiClient()
        {
            bool success = false;
            try
            {
                
                var ret = ApiClient.CallRemoteApi<ApiReturn>(config.apiRoute, config.apiArg, config.httpMethod);
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
