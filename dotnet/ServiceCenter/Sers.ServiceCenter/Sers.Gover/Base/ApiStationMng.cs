﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Sers.Core.Module.Api.ApiDesc;
using Sers.Gover.Base.Model;
using Sers.Gover.Persistence;
using Sers.ServiceCenter.Entity;

using Vit.Core.Module.Log;
using Vit.Core.Util.ConfigurationManager;
using Vit.Extensions;
using Vit.Extensions.Serialize_Extensions;

namespace Sers.Gover.Base
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ApiStationMng
    {
        [JsonProperty]
        ConcurrentDictionary<string, ApiStationData> apiStations;

        GoverApiCenterService goverManage;
        public ApiStationMng()
        {
        }

        public ApiStationMng Init(GoverApiCenterService goverManage)
        {
            this.goverManage = goverManage;
            if (null == apiStations)
            {
                apiStations = new ConcurrentDictionary<string, ApiStationData>();
            }
            return this;
        }


        #region 查询

        public IEnumerable<SsApiDesc> ApiDesc_GetAll()
        {
            var apiServices = (from apiStation in apiStations.Values
                               from apiService in apiStation.apiServices.Values
                               select apiService);

            var res = apiServices.Select((apiService) =>
            {
                var apiDesc = apiService.apiDesc.ConvertBySerialize<SsApiDesc>();
                apiDesc.ext = new { apiService.counter };
                return apiDesc;
            });
            return res;
        }

        #endregion


        #region ApiStation

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public List<ApiStationData> ApiStation_GetAll()
        {
            return apiStations.Values.OrderBy(m => m.stationName).ToList();
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
                apiStationData.eStatus = EServiceStationStatus.暂停;

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



                foreach (var apiNode in apiStationData.apiServices.Values.SelectMany((m) => m.apiNodes))
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
            apiStations.TryRemove(apiStation.stationName, out _);
        }

        #endregion



        #region ApiService
        /// <summary>
        /// 移除离线的ApiService
        /// </summary>
        public void ApiService_RemoveOffline()
        {
            List<ApiStationData> changedApiStationList;

            lock (this)
            {
                var apiServiceItems = (from apiStation in apiStations.Values
                                       from apiService in apiStation.apiServices.Values
                                       where apiService.apiNodeCount == 0
                                       select (apiStation, apiService)).ToList();

                foreach (var (apiStation, apiService) in apiServiceItems)
                {
                    apiStation.ApiService_Remove(apiService.apiDesc.ServiceKeyGet());
                }


                changedApiStationList = apiServiceItems.Select(m => m.apiStation).Distinct().ToList();

                foreach (var apiStation in changedApiStationList)
                {
                    if (apiStation.apiServiceCount == 0)
                    {
                        ApiStation_Remove(apiStation);
                    }
                }
            }

            #region 持久化对应ApiStation中的所有ApiDesc(异步执行)
            Task.Run(() =>
            {
                try
                {
                    foreach (var apiStation in changedApiStationList)
                    {
                        Persistence_ApiDesc.ApiDesc_SaveApiStationToJsonFile(apiStation);
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



        #region ApiNode

        static bool Config_ApiRegistEvent_Print = (false != Appsettings.json.GetByPath<bool?>("Sers.ServiceCenter.ApiRegistEvent_Print"));

        void ApiNode_Add(string route, ApiNode apiNode)
        {
            try
            {
                if (Config_ApiRegistEvent_Print)
                {
                    Logger.Info("[ApiCenterService]Add ApiNode", apiNode.apiDesc.ServiceKeyGet());
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
                Logger.Info("[ApiCenterService]Remove ApiNode", apiNode.apiDesc.ServiceKeyGet());
            }

            var apiStation = ApiStation_Get(route);
            if (apiStation == null) return;

            //ApiStation 注销ApiNode
            apiStation.ApiNode_Remove(apiNode);
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
        public void ServiceStation_Add(ServiceStation serviceStation)
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
        public void ServiceStation_Remove(ServiceStation serviceStation)
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
        public void ServiceStation_Pause(ServiceStation serviceStation)
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
        public void ServiceStation_Start(ServiceStation serviceStation)
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
