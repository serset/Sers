using Sers.Core.Module.ApiDesc.Attribute;
using Sers.Core.Module.Api;
using System.Collections.Generic;
using Sers.Core.Module.Api.Data;
using Sers.Core.Module.SsApiDiscovery;
using Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute;
using Sers.ServiceCenter.ApiCenter.Gover.Core;
using Sers.Gover.Core.Model;
using Sers.Core.Module.Api.Rpc;
using System;
using Newtonsoft.Json.Linq;
using Sers.Core.Util.Hardware;
using Sers.Core.Extensions;
using Sers.Core.Module.Log;
using System.Diagnostics;
using System.Linq;

namespace Sers.Gover.Controller.ApiControllers
{
    [SsStationName("_gover_")]
    public class ServerCenterController : IApiController
    { 

 
        [SsRoute("serviceCenter/healthInfo")]
        [SsCallerSource(ECallerSource.Internal)]
        //[CallFromGover]
        [SsName("获取服务中心健康数据")]
        public ApiReturn<Object> HealthInfo()
        {
            JObject healthInfo = new JObject();

            //(x.1) usageStatus
            healthInfo["usageStatus"] = DeviceManage.GetUsageInfo().ConvertBySerialize<JObject>();

            //(x.2) Process信息
            try
            {
                var process = Process.GetCurrentProcess();
                var processInfo = new JObject();
                healthInfo["Process"] = processInfo;
                
                //(x.x.1) threadInfo
                processInfo["ThreadCount"] = process.Threads.Count;
                int n = 0;
        
                foreach (ProcessThread th in process.Threads)
                {             
                    if (th.ThreadState == ThreadState.Running)
                        n++;
                }
                processInfo["RunningThreadCount"] = n;
                
                //(x.x.2) memory size
                processInfo["WorkingSet"] = (process.WorkingSet64/1024.0/1024).ToString("0.00")+" MB";

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            //(x.3)healthInfo
            ServiceCenter.ServiceCenter.Instance.mqMng.requestAdaptor.GetHealthInfo(healthInfo);

            return healthInfo;
        }

         


    }
}
