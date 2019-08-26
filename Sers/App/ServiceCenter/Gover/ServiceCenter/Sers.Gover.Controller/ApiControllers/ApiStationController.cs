using Sers.Core.Module.ApiDesc.Attribute;
using Sers.Core.Module.Api;
using System.Collections.Generic;
using Sers.Core.Module.Api.Data;
using Sers.Core.Module.SsApiDiscovery;
using Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute;
using Sers.ServiceCenter.ApiCenter.Gover.Core;
using Sers.Gover.Core.Model;
using Sers.Core.Module.Api.Rpc;

namespace Sers.Gover.Controller.ApiControllers
{
    [SsStationName("_gover_")]
    public class ApiStationController : IApiController
    { 

        /// <summary>
        /// 获取所有ApiStation
        /// </summary>
        /// <returns></returns>
        [SsRoute("apiStation/getAll")]
        [SsCallerSource(ECallerSource.Internal)]
        //[CallFromGover]
        [SsName("获取所有ApiStation")]
        public ApiReturn<List<ApiStationData>> GetAll()
        {
            return new ApiReturn<List<ApiStationData>> { data = GoverManage.Instance.ApiStation_GetAll() };
        }


        /// <summary>
        /// 暂停指定的ApiStation
        /// </summary>
        /// <returns></returns>
        [SsRoute("apiStation/pause")]
        [SsCallerSource(ECallerSource.Internal)]
        //[CallFromGover]
        [SsName("暂停指定的ApiStation")]
        public ApiReturn Pause(string stationName)
        {
            return new ApiReturn { success = GoverManage.Instance.ApiStation_Pause(stationName) };
        }

        /// <summary>
        /// 启用指定的ApiStation
        /// </summary>
        /// <returns></returns>
        [SsRoute("apiStation/start")]
        [SsCallerSource(ECallerSource.Internal)]
        //[CallFromGover]
        [SsName("启用指定的ApiStation")]
        public ApiReturn Start(string stationName)
        {
            return new ApiReturn { success = GoverManage.Instance.ApiStation_Start(stationName) };
        }
         



    }
}
