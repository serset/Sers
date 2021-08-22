using System;
using System.Linq;

using Sers.Core.Module.Rpc;
using Sers.Gover.Base;
using Sers.Hardware.Process;
using Sers.Hardware.Usage;
using Sers.SersLoader;
using Sers.SersLoader.ApiDesc.Attribute.Valid;

using Vit.Core.Module.Log;
using Vit.Core.Util.ComponentModel.Api;
using Vit.Core.Util.ComponentModel.Data;
using Vit.Core.Util.ComponentModel.Model;

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
            info.Process = ProcessInfo.GetCurrentProcessInfo();

            return info;
        }


        public class HealthInfoData
        {
            public UsageStatus usageStatus;

            public ProcessInfo Process;
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
                info.qps = qps;
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
