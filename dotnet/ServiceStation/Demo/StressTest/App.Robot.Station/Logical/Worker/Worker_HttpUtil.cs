using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Vit.Core.Module.Log;
using Vit.Core.Util.ComponentModel.Data;
using Vit.Extensions;
using Vit.Core.Util.Net;

namespace App.Robot.Station.Logical.Worker
{


    public class Worker_HttpUtil : Worker_ApiClient
    {

        HttpUtil httpUtil;
        RequestParam httpUtil_Request;
        public Worker_HttpUtil(TaskConfig config) : base(config)
        {
            httpUtil = new HttpUtil();
            httpUtil_Request = new RequestParam
            {
                url = config.apiRoute,
                body = config.apiArg,
                Method = config.httpMethod
            };
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void Processor()
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



    }
}
