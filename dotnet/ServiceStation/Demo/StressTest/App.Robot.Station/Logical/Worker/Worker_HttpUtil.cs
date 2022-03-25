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
        public Worker_HttpUtil(TaskItem taskItem) : base(taskItem)
        {

            httpUtil = new HttpUtil();
            httpUtil_Request = new RequestParam
            {
                url = taskItem.config.apiRoute,
                body = taskItem.config.apiArg,
                Method = taskItem.config.httpMethod
            };
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override void Processor()
        {
            bool success = false;
            try
            {
                var ret = httpUtil.Ajax<ApiReturn>(httpUtil_Request);
                if (ret == null || ret.success)
                {
                    success = true;
                }
                else
                {
                    if (taskItem.config.logError)
                        Logger.Info("失败", ret);
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
