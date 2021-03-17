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
        public override ApiNode GetCurApiNodeByLoadBalancing(RpcContextData rpcData, out ERouteType routeType)
        {
            var method= rpcData.http.method?.ToUpper();
            if (string.IsNullOrEmpty(method)) method = "_";

            var oriRoute = rpcData.route;          
            #region 去除query string(url ?后面的字符串)           
            //{
            //    // b2?a=c
            //    var index = oriRoute.IndexOf('?');
            //    if (index >= 0) 
            //    {
            //        oriRoute = oriRoute.Substring(0, index);
            //    }
            //}
            #endregion

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
