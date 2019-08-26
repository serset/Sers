using Sers.Core.Module.ApiDesc.Attribute;
using Sers.Core.Module.Api;
using Sers.Core.Module.SsApiDiscovery;
using Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute;
using Sers.ServiceCenter.ApiCenter.Gover.Core;
using Sers.Gover.Core.RateLimit;
using Newtonsoft.Json.Linq;
using Sers.Core.Module.Api.Data;
using Sers.Core.Module.Api.Rpc;

namespace Sers.Gover.Controller.ApiControllers
{
    [SsStationName("_gover_")]
    [SsRoutePrefix("rateLimit")]
    public class RateLimitController : IApiController
    {

        static RateLimitMng rateLimitMng => GoverManage.Instance.rateLimitMng;


        /// <summary>
        /// 获取所有限流项目
        /// </summary>
        /// <returns></returns>
        [SsRoute("getAll")]
        [SsCallerSource(ECallerSource.Internal)]
        //[CallFromGover]        
        public ApiReturn<IRateLimit[]> GetAll()
        {
            return rateLimitMng.RateLimit_GetAll(); 
        }



        /// <summary>
        /// 移除指定限流项目
        /// </summary>
        /// <returns></returns>
        [SsRoute("remove")]
        [SsCallerSource(ECallerSource.Internal)]
        //[CallFromGover]
        public ApiReturn Remove(string rateLimitKey)
        {
            rateLimitMng.RateLimit_Remove(rateLimitKey);
            return true;
        }


        /// <summary>
        /// 添加限流项目
        /// </summary>
        /// <returns></returns>
        [SsRoute("add")]
        [SsCallerSource(ECallerSource.Internal)]
        //[CallFromGover]
        public ApiReturn Add(JObject rateLimit)
        {
            return rateLimitMng.RateLimit_Add(rateLimit);        
        }


    }
}
