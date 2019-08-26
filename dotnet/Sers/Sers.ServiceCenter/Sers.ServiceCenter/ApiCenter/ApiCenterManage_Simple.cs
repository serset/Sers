using Sers.Core.Extensions;
using Sers.Core.Module.Api;
using Sers.Core.Module.Api.ApiDesc;
using Sers.Core.Module.Api.Message;
using Sers.Core.Module.Api.RouteMap;
using Sers.Core.Module.Rpc;
using Sers.Core.Module.SsApiDiscovery.SersValid;
using Sers.Core.Util.SsError;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using Sers.Core.Module.Log;
using Newtonsoft.Json;

namespace Sers.ServiceCenter.ApiCenter
{
    /// <summary>
    /// 用来管理 api站点 服务站点 api服务 api节点 等
    /// </summary> 
    public class ApiCenterManage_Simple: IApiCenterManage
    {
        RouteMap<ApiService> routeMap = new RouteMap<ApiService>();


        /// <summary>
        /// BeforeCallApi(IRpcContextData rpcData, ApiMessage requestMessage)
        /// </summary>
        [JsonIgnore]
        public Action<IRpcContextData, ApiMessage> BeforeCallApi { get; set; }


        /// <summary>
        /// connGuid 和 服务站点 的映射
        /// </summary>
        ConcurrentDictionary<string, ServiceStation> serviceStations = new ConcurrentDictionary<string, ServiceStation>();



        public List<SsApiDesc> GetAllApiDesc()
        {
            return routeMap.GetAll().Select((m) => m.apiDesc).ToList();
        }



        #region CallApi

        
 
        public List<ArraySegment<byte>> CallApi(IRpcContextData rpcData, ApiMessage requestMessage)
        {

            var route = rpcData.route;


            #region (x.1) get apiService
            var apiService = routeMap.Routing(route);

            if (null == apiService)
            {
                //返回api 不存在        
                return new ApiMessage().InitByError(SsError.Err_ApiNotExists).Package();
            }
            var apiDesc = apiService.apiDesc;

            rpcData.route = apiDesc.route;
            #endregion


            #region (x.5) BeforeCallApi
            try
            {
                BeforeCallApi?.Invoke(rpcData, requestMessage);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            #endregion


            #region (x.6)SsValid
            if (!SersValidMng.Valid(rpcData.oriJson, apiDesc.rpcValidations, out var validError))
            {
                return new ApiMessage().InitByError(validError).Package();
            }
            #endregion

            requestMessage.RpcContextData_OriData_Set(rpcData);



            #region (x.7)获取ApiNode

            ApiNode apiNode = null;
            var apiNodes = apiService.apiNodes;

            //TODO： 负载均衡的实现
            int curIndex = new Random().Next(0, apiNodes.Count);
            try
            {
                apiNode = apiNodes[curIndex];
            }
            catch (Exception)
            {
            }
           
            if (null == apiNode)
            {
                //返回api 不存在
                return new ApiMessage().InitByError(SsError.Err_ApiNotExists).Package();               
            }
            #endregion


            return apiNode.CallApi(rpcData, requestMessage);
        }
        #endregion








        public void ServiceStation_Regist(ServiceStation serviceStation)
        {

            Logger.Info("[ApiCenter]注册站点Api,mqConnGuid:" + serviceStation.mqConnGuid);
            lock (this)
            {
                serviceStations[serviceStation.mqConnGuid] = serviceStation;

                foreach (var apiNode in serviceStation.apiNodes)
                {
                    apiNode.serviceStation = serviceStation;

                    var route = apiNode.apiDesc.route;
                    if (string.IsNullOrWhiteSpace(route)) continue;


                    #region RegistApiNode
                    Logger.Info("[ApiCenter]Api注册,route:" + route);

                    var apiService = routeMap.Get(route);
                    if (null == apiService)
                    {
                        apiService = new ApiService();
                        routeMap.Set(route, apiService);
                    }
                    
                    apiService.AddApiNode(apiNode);

                    #endregion
                }
            }

        }

        public void ServiceStation_Remove(string connGuid)
        {
            Logger.Info("[ApiCenter]移除站点Api,connGuid:" + connGuid);
            lock (this)
            {
                if (serviceStations.TryRemove(connGuid, out var serviceStation))
                {
                    foreach (var apiNode in serviceStation.apiNodes)
                    {
                        var route = apiNode.apiDesc.route;

                        var apiService = apiNode.apiService;
                        apiService.RemoveApiNode(apiNode);

                        if (0 == apiService.apiNodeCount)
                        {
                            routeMap.Remove(route);
                        }
                    }
                }
            }

        }



    }
}
