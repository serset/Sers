using System.Collections.Generic;

using Sers.Core.Module.Rpc;
using Sers.Gover.Base;
using Sers.Gover.Base.Model;
using Sers.SersLoader;
using Sers.SersLoader.ApiDesc.Attribute.Valid;

using Vit.Core.Util.ComponentModel.Api;
using Vit.Core.Util.ComponentModel.Data;
using Vit.Core.Util.ComponentModel.Model;

namespace Sers.Gover.Controllers.ApiControllers
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
            return new ApiReturn<List<ApiStationData>> { data = GoverApiCenterService.Instance.ApiStation_GetAll() };
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
            return new ApiReturn { success = GoverApiCenterService.Instance.ApiStation_Pause(stationName) };
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
            return new ApiReturn { success = GoverApiCenterService.Instance.ApiStation_Start(stationName) };
        }




    }
}
