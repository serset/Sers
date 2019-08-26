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
    public class ServiceStationController : IApiController
    {
        /// <summary>
        /// 获取所有ServiceStation
        /// </summary>
        /// <returns></returns>
        [SsRoute("serviceStation/getAll")]
        [SsCallerSource(ECallerSource.Internal)]
        //[CallFromGover]
        [SsName("获取所有ServiceStation")]
        public ApiReturn<List<ServiceStationData>> GetAll()
        {
            return new ApiReturn<List<ServiceStationData>> { data = GoverManage.Instance.ServiceStation_GetAll() }; 
        }


        /// <summary>
        /// 暂停指定的服务站点
        /// </summary>
        /// <returns></returns>
        [SsRoute("serviceStation/pause")]
        [SsCallerSource(ECallerSource.Internal)]
        //[CallFromGover]
        [SsName("暂停指定的服务站点")]
        public ApiReturn Pause(string connGuid)
        {
            return new ApiReturn { success = GoverManage.Instance.ServiceStation_Pause(connGuid) };
        }

        /// <summary>
        /// 启用指定的服务站点
        /// </summary>
        /// <returns></returns>
        [SsRoute("serviceStation/start")]
        [SsCallerSource(ECallerSource.Internal)]
        //[CallFromGover]
        [SsName("启用指定的服务站点")]
        public ApiReturn Start(string connGuid)
        {
            return new ApiReturn { success = GoverManage.Instance.ServiceStation_Start(connGuid) };
        }
         


     




    }
}
