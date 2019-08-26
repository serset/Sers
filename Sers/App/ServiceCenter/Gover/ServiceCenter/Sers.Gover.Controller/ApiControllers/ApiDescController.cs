using Sers.Core.Module.ApiDesc.Attribute;
using Sers.Core.Module.Api;
using System.Collections.Generic;
using Sers.Core.Module.Api.ApiDesc;
using Sers.Core.Module.Api.Data;
using Sers.Core.Module.SsApiDiscovery;
using Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute;
using Sers.ServiceCenter.ApiCenter.Gover.Core;
using Sers.Core.Module.Api.Rpc;

namespace Sers.Gover.Controller.ApiControllers
{
    [SsStationName("_gover_")]
    public class ApiDescController : IApiController
    { 

        /// <summary>
        /// 获取所有可调用api。返回ApiDesc列表
        /// </summary>
        /// <returns></returns>
        [SsRoute("apiDesc/getActive")]
        [SsCallerSource(ECallerSource.Internal)]
        //[CallFromGover]
        [SsName("获取所有可调用api")]
        public ApiReturn<List<SsApiDesc>> GetActive()
        {
            return new ApiReturn<List<SsApiDesc>> { data= GoverManage.Instance.ApiDesc_GetActive() };       
        }

        /// <summary>
        /// 获取所有api。返回ApiDesc列表
        /// </summary>
        /// <returns></returns>
        [SsRoute("apiDesc/getAll")]
        [SsCallerSource(ECallerSource.Internal)]
        //[CallFromGover]
        [SsName("获取所有api")]
        public ApiReturn<List<SsApiDesc>> GetAll()
        {
            return new ApiReturn<List<SsApiDesc>> { data = GoverManage.Instance.ApiDesc_GetAll() }; 
        }

 
 




    }
}
