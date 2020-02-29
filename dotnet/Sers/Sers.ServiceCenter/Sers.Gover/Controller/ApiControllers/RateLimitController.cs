using Newtonsoft.Json.Linq;
using Sers.Core.Module.Api.Rpc;
using Sers.SersLoader;
using Sers.SersLoader.ApiDesc.Attribute.Valid;
using Sers.Gover.Base;
using Sers.Gover.RateLimit;
using Vit.Core.Util.ComponentModel.Api;
using Vit.Core.Util.ComponentModel.Data;

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
