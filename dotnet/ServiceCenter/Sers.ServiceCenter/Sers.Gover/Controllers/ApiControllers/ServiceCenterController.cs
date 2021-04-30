using System;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json.Linq;
using Sers.Core.Module.Rpc;
using Sers.Gover.Base;
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
        #region HealthInfo

     

        [SsRoute("serviceCenter/healthInfo")]
        [SsCallerSource(ECallerSource.Internal)]
        //[CallFromGover]
        [SsName("获取服务中心健康数据")]
        public ApiReturn<HealthInfoData> HealthInfo()
        {
            var info = new HealthInfoData();

            //(x.1) usageStatus
            info.usageStatus = UsageHelp.GetUsageInfo();

            //(x.2) Process信息
            try
            {
                var process = Process.GetCurrentProcess();


                //(x.x.1) ThreadCount
                info.Process.ThreadCount = process.Threads.Count;

                //(x.x.2) RunningThreadCount
                int n = 0;        
                foreach (ProcessThread th in process.Threads)
                {             
                    if (th.ThreadState == ThreadState.Running)
                        n++;
                }
                info.Process.RunningThreadCount = n;

                //(x.x.3) WorkingSet
                info.Process.WorkingSet = process.WorkingSet64 / 1024.0f / 1024;

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            } 
            return info;
        }


        public class HealthInfoData 
        {
            public UsageStatus usageStatus;

            public ProcessInfo Process = new ProcessInfo();


            public class ProcessInfo 
            {
                /// <summary>
                /// 总线程数
                /// </summary>
                public int ThreadCount;

                /// <summary>
                /// 活动的线程数
                /// </summary>
                public int RunningThreadCount;


                /// <summary>
                /// 占用总内存（单位：MB）
                /// </summary>
                public float WorkingSet;
            }

        }

        #endregion







        #region Statistics

        [SsRoute("serviceCenter/statistics")]
        [SsCallerSource(ECallerSource.Internal)]
        //[CallFromGover]
        [SsName("获取服务中心统计信息")]
        public ApiReturn<StatisticsInfo> Statistics()
        {
            var info = new StatisticsInfo();


            //(x.1) qps
            try
            {
                var qps = GoverApiCenterService.Instance.ApiStation_GetAll().Sum(m => m.qps);
                info.qps= qps;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return info;
        }

        public class StatisticsInfo 
        {
            /// <summary>
            /// 总qps
            /// </summary>
            public float qps;
        }

        #endregion

    }
}
