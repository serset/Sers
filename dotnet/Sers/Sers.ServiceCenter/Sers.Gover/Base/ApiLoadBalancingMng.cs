using System.Collections.Generic;
using System.Linq;
using Sers.Core.Module.Api.ApiDesc;
using Sers.Core.Module.Api.RouteMap;
using Sers.Core.Module.Rpc;
using Sers.Gover.Base.Model;
using Sers.ServiceCenter.Entity;

namespace Sers.Gover.Base
{
    /// <summary>
    /// 当前可调用的ApiService
    /// </summary>
    public class ApiLoadBalancingMng
    {
     
        public ApiLoadBalancingMng()
        {    
        }

        protected readonly RouteMap<LoadBalancingForApiNode> routeMap = new RouteMap<LoadBalancingForApiNode>();

        public IEnumerable<SsApiDesc> GetAllApiDesc()
        {
            return routeMap.GetAll().Select((m) => m.apiDesc);
        }

        /// <summary>
        /// 通过负载均衡算法 获取可调用的ApiNode
        /// </summary>
        /// <param name="rpcData"></param>
        /// <param name="routeType"></param>
        /// <returns></returns>
        public virtual ApiNode GetCurApiNodeByLoadBalancing(IRpcContextData rpcData, out ERouteType routeType)
        {
            return routeMap.Routing(rpcData.route, out routeType)?.GetCurApiNodeBalancing();
        }

        protected virtual string GetApiRoute(ApiNode apiNode)
        {
            return apiNode.apiDesc.route;
        }

        public void ApiNode_Add(ApiNode apiNode)
        {
            lock (this)
            {
                var route = GetApiRoute(apiNode);
                if (string.IsNullOrWhiteSpace(route)) return;

                var lbApiNode = routeMap.Get(route);
                if (null == lbApiNode)
                {
                    lbApiNode = new LoadBalancingForApiNode();
                    routeMap.Set(route, lbApiNode);
                }
                lbApiNode.AddApiNode(apiNode);
            }
        }

        public void ApiNode_Remove(ApiNode apiNode)
        {
            lock (this)
            {
                var route = GetApiRoute(apiNode);
                if (string.IsNullOrWhiteSpace(route)) return;


                var lbApiNode = routeMap.Get(route);
                if (null == lbApiNode)
                {
                    return;
                }

                lbApiNode.RemoveApiNode(apiNode);
                if (0 == lbApiNode.apiNodeCount)
                {
                    routeMap.Remove(route);
                }
            }
        }



    }
}
