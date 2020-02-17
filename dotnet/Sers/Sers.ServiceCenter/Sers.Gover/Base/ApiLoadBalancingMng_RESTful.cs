using Sers.Core.Module.Api.RouteMap;
using Sers.Core.Module.Rpc;
using Sers.ServiceCenter.Entity;
using Vit.Extensions;

namespace Sers.Gover.Base
{
    /// <summary>
    /// 当前可调用的ApiService
    /// </summary>
    public class ApiLoadBalancingMng_RESTful: ApiLoadBalancingMng
    {
        /// <summary>
        /// 如 "GET"
        /// </summary>
        /// <param name="apiNode"></param>
        /// <returns></returns>
        static string GetHttpMethod(ApiNode apiNode)
        {
            var method = apiNode.apiDesc?.HttpMethodGet()?.ToUpper();
            if (string.IsNullOrEmpty(method)) method = "_";
            return method;
        }

         

        /// <summary>
        /// 通过负载均衡算法 获取可调用的ApiNode
        /// </summary>
        /// <param name="rpcData"></param>
        /// <param name="routeType"></param>
        /// <returns></returns>
        public override ApiNode GetCurApiNodeByLoadBalancing(IRpcContextData rpcData, out ERouteType routeType)
        {
            var method= rpcData.http_method_Get()?.ToUpper();
            if (string.IsNullOrEmpty(method)) method = "_";

            var oriRoute = rpcData.route;

            var route = "/"+method + oriRoute;
            var lb = routeMap.Routing(route, out routeType);
            if (lb != null)
            {
                return lb.GetCurApiNodeBalancing();
            }

            route = "/_" + oriRoute;
            lb = routeMap.Routing(route, out routeType);
            if (lb != null)
            {
                return lb.GetCurApiNodeBalancing();
            }
            return null;
        }

        protected override string GetApiRoute(ApiNode apiNode)
        {
            var route=  "/"+GetHttpMethod(apiNode) + apiNode.apiDesc.route;

            return route;
        }


    }
}
