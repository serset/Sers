using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Vit.Core.Module.Log;
using Vit.Core.Util.ComponentModel.Data;
using Vit.Extensions;
using Vit.Core.Util.Net;
using Newtonsoft.Json;

namespace App.Robot.Station.Logical.Worker
{


    public class Worker_HttpClient: Worker_ApiClient
    {

        HttpClient httpClient;
        HttpRequest httpClient_ReqParam;
        public Worker_HttpClient(TaskItem taskItem) :base(taskItem)
        {
            httpClient = new HttpClient();
            httpClient_ReqParam = new HttpRequest
            {
                url = taskItem.config.apiRoute,
                body = taskItem.config.apiArg,
                httpMethod = taskItem.config.httpMethod
            };

        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void Processor()
        {
            bool success = false;
            try
            {
                var ret = httpClient.Send<ApiReturn>(httpClient_ReqParam)?.data;
                if (ret == null || ret.success)
                {
                    success = true;
                }
                else
                {
                    if (taskItem.config.logError)
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



    }
}
