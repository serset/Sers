using System.Collections.Generic;
using Sers.Core.Module.Api.Rpc;
using Sers.ApiLoader.Sers.Attribute;
using Sers.ApiLoader.Sers;
using Sers.Gover.Base;
using Sers.Gover.Base.Model;
using Vit.Core.Util.ComponentModel.Api;
using Vit.Core.Util.ComponentModel.Data;
using Vit.Core.Util.ComponentModel.Model;

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
        [SsName("启用指定的ApiStation")]
        public ApiReturn Start(string stationName)
        {
            return new ApiReturn { success = GoverManage.Instance.ApiStation_Start(stationName) };
        }
         



    }
}
