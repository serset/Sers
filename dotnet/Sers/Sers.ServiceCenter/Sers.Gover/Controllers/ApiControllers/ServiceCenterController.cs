using System;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using Sers.Core.Module.Api.Rpc;
using Sers.Hardware.Usage;
using Sers.SersLoader;
using Sers.SersLoader.ApiDesc.Attribute.Valid;
using Vit.Core.Module.Log;
using Vit.Core.Util.ComponentModel.Api;
using Vit.Core.Util.ComponentModel.Data;
using Vit.Core.Util.ComponentModel.Model;
using Vit.Extensions;

namespace Sers.Gover.Controllers.ApiControllers
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
            healthInfo["usageStatus"] = UsageHelp.GetUsageInfo()?.ConvertBySerialize<JObject>();

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

           
     

            return healthInfo;
        }

         


    }
}
