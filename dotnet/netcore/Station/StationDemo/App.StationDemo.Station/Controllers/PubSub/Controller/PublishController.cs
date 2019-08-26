using Sers.Core.Extensions;
using Sers.Core.Module.Api.Data;
using Sers.Core.Module.PubSub;
using Sers.Core.Module.Rpc;
using Sers.Core.Module.SsApiDiscovery;
using Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute;

namespace App.StationDemo.Station.Controllers.PubSub.Controller
{
 
    //路由前缀，可不指定
    [SsRoutePrefix("api/pubsub/controller")]
    public class PublishController
        : IApiController
    {

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [SsRoute("publish/*")]
        public ApiReturn Publish(string msgBody)
        {
            var rpcData = RpcContext.RpcData;
            var http_url_search = rpcData.http_url_search_Get();
            string msgTitle = http_url_search.Split('/')[0];

            MessageClient.Publish(msgTitle, msgBody);
            return new ApiReturn();
        }

    }
}
