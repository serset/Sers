using Newtonsoft.Json;
using Sers.Core.Extensions;
using Sers.Core.Module.Api.ApiDesc;
using Sers.Core.Module.Log;
using Sers.Core.Util.ConfigurationManager;
using Sers.Core.Util.Extensible;
using Sers.Gover.Core.Model;
using Sers.Gover.Core.Persistence;
using Sers.ServiceCenter;
using Sers.ServiceCenter.ApiCenter.Gover.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sers.Gover.Core
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ApiStationMng
    {
        [JsonProperty]
        ConcurrentDictionary<string, ApiStationData> apiStations;
        GoverManage goverManage;
        public ApiStationMng()
        {            
        }

        public ApiStationMng Init(GoverManage goverManage)
        {
            this.goverManage = goverManage;
            if (null == apiStations)
            {
                apiStations = new ConcurrentDictionary<string, ApiStationData>();
            }
            return this;
        }


        #region 查询

        public List<SsApiDesc> ApiDesc_GetAll()
        {
            var apiServices = (from apiStation in apiStations.Values
                       from apiService in apiStation.apiServices.Values
                       select apiService);

            var res = apiServices.Select((apiService)=> 
            {
                var apiDesc = apiService.apiDesc.ConvertBySerialize<SsApiDesc>();
                apiDesc.ext = new { apiService.counter};
                return apiDesc;
            });
            return res.ToList();
        }



        #endregion


        #region ApiStation

        public List<ApiStationData> ApiStation_GetAll()
        {
            return apiStations.Values.ToList();
        }

         
        public bool ApiStation_Pause(string stationName)
        {
            lock (this)
            {
                if (!apiStations.TryGetValue(stationName, out var apiStationData))
                {
                    return false;
                }               

                if (apiStationData.eStatus == EServiceStationStatus.暂停)
                {
                    return true;
                }
                apiStationData.eStatus=EServiceStationStatus.暂停;

                foreach (var apiNode in apiStationData.apiServices.Values.SelectMany((m) => m.apiNodes))
                {
                    apiNode.StopReason_Add(goverManage.apiLoadBalancingMng, "stopByApiStation");
                }
            }
            return true;
        }
   
        public bool ApiStation_Start(string stationName)
        {
            lock (this)
            {
                if (!apiStations.TryGetValue(stationName, out var apiStationData))
                {
                    return false;
                }

                if (apiStationData.eStatus == EServiceStationStatus.正常)
                {
                    return true;
                }
                apiStationData.eStatus = EServiceStationStatus.正常;



                foreach (var apiNode in apiStationData.apiServices.Values.SelectMany((m)=>m.apiNodes))
                {
                    apiNode.StopReason_Remove(goverManage.apiLoadBalancingMng, "stopByApiStation");
                }
            }
            return true;
        }
        






        ApiStationData ApiStation_Get(string route)
        {
            var stationName = route.Split('/')[1];
            if (apiStations.TryGetValue(stationName, out var apiStationData))
            {
                return apiStationData;
            }
            return null;
        }

        public ApiStationData ApiStation_GetOrAddByName(string stationName)
        {
            return apiStations.GetOrAdd(stationName, (n) => new ApiStationData() { stationName = n });
        }
        public ApiStationData ApiStation_GetOrAddByRoute(string route)
        {
            var stationName = route.Split('/')[1];
            return ApiStation_GetOrAddByName(stationName);
        }

        void ApiStation_Remove(ApiStationData apiStation)
        {
            apiStations.TryRemove(apiStation.stationName,out _);
        }

        #endregion



        #region ApiNode

        static bool Config_ApiRegistEvent_Print = (false != ConfigurationManager.Instance.GetByPath<bool?>("Sers.ServiceCenter.ApiRegistEvent_Print"));

        void ApiNode_Add(string route, ApiNode apiNode)
        {
            try
            {
                if (Config_ApiRegistEvent_Print)
                {
                    Logger.Info("[Api注册],route:" + route);
                }
                

                //ApiStation 添加ApiNode
                var apiStation = ApiStation_GetOrAddByRoute(route);
                apiStation.ApiNode_Add(apiNode);

                if (apiStation.IsActive())
                {
                    apiNode.Start(goverManage.apiLoadBalancingMng);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
        void ApiNode_Remove(ApiNode apiNode)
        {
            var route = apiNode?.apiDesc?.route;
            if (string.IsNullOrWhiteSpace(route)) return;

            if (Config_ApiRegistEvent_Print)
            {
                Logger.Info("[Api注销],route:" + route);
            }

            var apiStation = ApiStation_Get(route);
            if (apiStation == null) return;

            //ApiStation 注销ApiNode
            apiStation.ApiNode_Remove(route, apiNode);
            //if (apiStation.apiNodeCount == 0)
            //{
            //    ApiStation_Remove(apiStation);
            //}

            //ApiLoadBalancingMng 注销ApiNode
            apiNode.Stop(goverManage.apiLoadBalancingMng); 
        }


       



        #endregion


        #region ServiceStation        

        #region ServiceStation_Add
        public void ServiceStation_Add(ServiceCenter.ServiceStation serviceStation)
        {
            //注册apiNode
            lock (this)
            {
                foreach (var apiNode in serviceStation.apiNodes)
                {
                    var route = apiNode?.apiDesc?.route;
                    if (string.IsNullOrWhiteSpace(route)) continue;

                    apiNode.serviceStation = serviceStation;

                    ApiNode_Add(route, apiNode);
                }
            }

            #region 持久化对应ApiStation中的所有ApiDesc(异步执行)
            Task.Run(() =>
            {
                try
                {
                    foreach (var apiStationName in serviceStation.ApiStationNames_Get())
                    {
                        if (apiStations.TryGetValue(apiStationName, out var apiStation))
                        {
                            Persistence_ApiDesc.ApiDesc_SaveApiStationToJsonFile(apiStation);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            });
            #endregion

        }
        #endregion

        #region ServiceStation_Remove
        public void ServiceStation_Remove(ServiceCenter.ServiceStation serviceStation)
        {
            lock (this)
            {
                //注销apiNode
                foreach (var apiNode in serviceStation.apiNodes)
                {
                    ApiNode_Remove(apiNode);
                }
            }
        }
        #endregion

        #region ServiceStation_Pause
        public void ServiceStation_Pause(ServiceCenter.ServiceStation serviceStation)
        {
            lock (this)
            {
                foreach (var apiNode in serviceStation.apiNodes)
                {
                    apiNode.StopReason_Add(goverManage.apiLoadBalancingMng, "stopByServiceStation");             
                }
            }
        }
        #endregion

        #region ServiceStation_Start
        public void ServiceStation_Start(ServiceCenter.ServiceStation serviceStation)
        {
            lock (this)
            {
                foreach (var apiNode in serviceStation.apiNodes)
                {
                    apiNode.StopReason_Remove(goverManage.apiLoadBalancingMng, "stopByServiceStation");          
                }
            }
        }
        #endregion

        #endregion
    }
}
