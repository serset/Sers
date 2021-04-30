using Sers.SersLoader;
using Sers.SersLoader.ApiDesc.Attribute.Valid;
using Sers.Core.Module.Rpc;
using Vit.Core.Util.ComponentModel.Api;
using Vit.Core.Util.ComponentModel.Data;
using Sers.Core.Module.Api;
using System;

namespace App.Robot.Station.Controllers
{
    public class StatisticsController : IApiController
    {
              

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [SsRoute("statistics/LogQps")]
        [SsCallerSource(ECallerSource.Internal)]
        public ApiReturn<float> LogQps()
        {             
            var apiRet = ApiClient.CallRemoteApi<ApiReturn<StatisticsInfo>>("/_gover_/serviceCenter/statistics");

            if (apiRet?.success!=true) return apiRet?.error;

            var qps = apiRet.data.qps;
            Console.WriteLine("服务中心当前总qps: "+ qps);
            return qps;
        }

        public class StatisticsInfo
        {
            /// <summary>
            /// 总qps
            /// </summary>
            public float qps;
        }

    }
}
