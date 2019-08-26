using Sers.Core.Module.Api.ApiDesc;
using Sers.Core.Module.Api.RouteMap;
using Sers.Core.Module.Log;
using Sers.ServiceCenter;
using Sers.ServiceCenter.ApiCenter.Gover.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sers.Core.Extensions;

namespace Sers.Gover.Core
{
    /// <summary>
    /// 当前可调用的ApiService
    /// </summary>
    public class ApiLoadBalancingMng
    {
        //GoverManage goverManage;
        public ApiLoadBalancingMng(GoverManage goverManage)
        {
            //this.goverManage = goverManage;
        }

        readonly RouteMap<LoadBalancingForApiNode> routeMap = new RouteMap<LoadBalancingForApiNode>();

        public List<SsApiDesc> GetAllApiDesc()
        {
            return routeMap.GetAll().Select((m) => m.apiDesc).ToList();
        }

        /// <summary>
        /// 通过负载均衡算法 获取可调用的ApiNode
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        public ApiNode GetCurApiNodeByLoadBalancing(string route, out ERouteType routeType)
        {
            return routeMap.Routing(route, out routeType)?.GetCurApiNodeBalancing();
        }


        public void ApiNode_Add(ApiNode apiNode)
        {
            lock (this)
            {
                var route = apiNode.apiDesc.route;

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
                var route = apiNode.apiDesc.route;
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
