using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Vit.Core.Module.Log;
using Vit.Core.Util.ComponentModel.Data;
using Vit.Extensions;
using Vit.Core.Util.Net;

namespace App.Robot.Station.Logical.Worker
{


    public class Worker_HttpClient: Worker_ApiClient
    {

        HttpClient httpClient;
        HttpRequest httpClient_ReqParam;
        public Worker_HttpClient(TaskConfig config):base(config)
        {
            httpClient = new HttpClient();
            httpClient_ReqParam = new HttpRequest
            {
                url = config.apiRoute,
                body = config.apiArg,
                httpMethod = config.httpMethod
            };

        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void Processor()
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

        

    }
}
