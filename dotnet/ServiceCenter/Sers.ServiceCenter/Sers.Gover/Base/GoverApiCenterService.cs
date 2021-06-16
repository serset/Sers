using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Sers.Core.CL.MessageOrganize;
using Sers.Core.Module.Api.ApiDesc;
using Sers.Core.Module.Api.Data;
using Sers.Core.Module.Api.RouteMap;
using Sers.Core.Module.Env;
using Sers.Core.Module.Message;
using Sers.Core.Module.Rpc;
using Sers.Gover.Base.Model;
using Sers.Gover.Persistence;
using Sers.Gover.RateLimit;
using Sers.ServiceCenter.ApiCenter;
using Sers.ServiceCenter.Entity;
using Vit.Core.Module.Log;
using Vit.Core.Util.ComponentModel.SsError;
using Vit.Core.Util.ConfigurationManager;
using Vit.Extensions;

namespace Sers.Gover.Base
{
    [JsonObject(MemberSerialization.OptIn)]
    public class GoverApiCenterService : ApiCenterService
    {
        #region static       
        public static readonly GoverApiCenterService Instance = LoadFromFile();
        static GoverApiCenterService LoadFromFile()
        {
            var mng=new GoverApiCenterService();

            Persistence_ApiDesc.ApiDesc_LoadAllFromJsonFile(mng.apiStationMng);
            Persistence_Counter.LoadCounterFromJsonFile(mng.apiStationMng);

            return mng;
        }
        public static void SaveToFile()
        {
            Persistence_Counter.SaveCounterToJsonFile(Instance.apiStationMng); 
        }
        #endregion

 



        public GoverApiCenterService()
        {
            //init apiLoadBalancingMng
            switch (ConfigurationManager.Instance.GetStringByPath("Sers.ServiceCenter.ApiRouteType"))
            {
                case "IgnoreHttpMethod": apiLoadBalancingMng = new ApiLoadBalancingMng(); break;
                default: apiLoadBalancingMng = new ApiLoadBalancingMng_RESTful(); break;
            }


            serviceStationMng = new ServiceStationMng();
            serviceStationMng.Init(this);

            apiStationMng = new ApiStationMng();
            apiStationMng.Init(this);
        }

        

        internal readonly ApiLoadBalancingMng apiLoadBalancingMng;

        [JsonProperty]
        internal ApiStationMng apiStationMng { get; private set; }

        [JsonIgnore]
        internal ServiceStationMng serviceStationMng { get; private set; }

        [JsonIgnore]
        public RateLimitMng rateLimitMng { get; private set; } = new RateLimitMng();

        public void SaveUsageInfo(EnvUsageInfo item)
        {
            serviceStationMng.SaveUsageInfo(item);
        }




        #region ServiceStation

        public IEnumerable<SsApiDesc> ApiDesc_GetActive()
        {
            return apiLoadBalancingMng.GetAllApiDesc();
        
        }

        public IEnumerable<SsApiDesc> ApiDesc_GetAll()
        {
            return apiStationMng.ApiDesc_GetAll();
        }

        #endregion






        #region CallApi

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void CallApiAsync(ApiMessage requestMessage, Object sender, Action<object, Vit.Core.Util.Pipelines.ByteData> callback)
        {
            RpcContextData rpcData = null;
            try
            {
                rpcData = RpcContextData.FromBytes(requestMessage.rpcContextData_OriData);


                #region (x.0)ApiScopeEvent
                apiScopeEventList?.ForEach(onScope =>
                {
                    try
                    {
                        var onDispose = onScope(rpcData, requestMessage);
                        if (onDispose != null)
                        {
                            callback += onDispose;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                });
                #endregion


                #region (x.1)route 判空               
                if (string.IsNullOrWhiteSpace(rpcData.route))
                {
                    //返回api 不存在
                    SendReply(SsError.Err_ApiNotExists);
                    return;
                }
                #endregion


                #region (x.2) 服务限流 BeforeLoadBalancing               
                var error = rateLimitMng.BeforeLoadBalancing(rpcData, requestMessage);
                if (null != error)
                {
                    SendReply(error);
                    return;
                }
                #endregion

                #region (x.3) 负载均衡，获取对应服务端                
                var apiNode = apiLoadBalancingMng.GetCurApiNodeByLoadBalancing(rpcData, out var routeType);

                if (null == apiNode)
                {
                    //返回api 不存在
                    SendReply(SsError.Err_ApiNotExists);
                    return;
                }
                #endregion

                #region (x.4) 服务限流 BeforeCallRemoteApi
                error = rateLimitMng.BeforeCallRemoteApi(rpcData, requestMessage, apiNode);
                if (null != error)
                {
                    SendReply(error);
                    return;
                }
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

                #region (x.6) 权限校验 SsValid
                // 权限校验不通过，调用次数也计数
                // TODO:应当有其他计数

                JObject oriJson = null;

                //(x.x.1) rpcValidations Sers1校验
                if (apiNode.apiDesc.rpcValidations != null && apiNode.apiDesc.rpcValidations.Count > 0)
                {
                    if (oriJson == null) oriJson = rpcData.Serialize().Deserialize<JObject>();

                    if (!Sers.Core.Module.Valid.Sers1.RpcVerify1.Verify(oriJson, apiNode.apiDesc.rpcValidations, out var validError))
                    {
                        SendReply(validError);
                        return;
                    }
                }

                //(x.x.2) rpcVerify2 Sers2校验
                if (apiNode.apiDesc.rpcVerify2 != null && apiNode.apiDesc.rpcVerify2.Count > 0)
                {
                    if (oriJson == null) oriJson = rpcData.Serialize().Deserialize<JObject>();

                    if (!Sers.Core.Module.Valid.Sers2.RpcVerify2.Verify(oriJson, apiNode.apiDesc.rpcVerify2, out var verifyError))
                    {
                        SendReply(verifyError);
                        return;
                    }
                }
                #endregion


                #region (x.7) RpcContextData 修正
                //(x.x.1) 修正route
                // 调用服务端 泛接口（如： "/station1/fold2/*"）时,route应修正为"/station1/fold2/*"，而不是原始 地址（如： "/station1/fold2/index.html"）
                if (routeType == ERouteType.genericRoute)
                {
                    rpcData.route = apiNode.apiDesc.route;
                    requestMessage.rpcContextData_OriData = ArraySegmentByteExtensions.Null;
                }


                //(x.x.2) 修正 requestMessage
                if (requestMessage.rpcContextData_OriData.Count <= 0) {
                    requestMessage.RpcContextData_OriData_Set(rpcData);
                }
                #endregion             


                #region (x.8)服务调用
                apiNode.CallApiAsync(rpcData, requestMessage, sender,callback);                
                #endregion

            }
            catch (Exception ex)
            {
                Logger.Error(ex);

                ApiSysError.LogSysError(rpcData, requestMessage, ex.ToSsError());
 
                SendReply(SsError.Err_SysErr);
                return;
            }


            void SendReply(SsError error)
            {
                //callback(sender, new ApiMessage().InitByError(error).SetSysErrToRpcData(error).Package().ByteDataToBytes().BytesToArraySegmentByte());
                callback(sender, new ApiMessage().InitAsApiReplyMessageByError(error).Package());
            }

          

        }
        #endregion


        #region ServiceStation

        public List<ServiceStationData> ServiceStation_GetAll()
        {
            return serviceStationMng.ServiceStation_GetAll();
        }


        public override void ServiceStation_Regist(ServiceStation serviceStation)
        {
            Logger.Info("[ApiCenterService]Regist serviceStation,stationName:" + serviceStation?.serviceStationInfo?.serviceStationName);
            serviceStationMng.ServiceStation_Add(serviceStation);
        }

        /// <summary>
        /// 更新服务站点设备硬件信息
        /// </summary>
        /// <param name="serviceStation"></param>
        public override bool ServiceStation_UpdateStationInfo(ServiceStation serviceStation)
        {
            Logger.Info("[ApiCenterService]ServiceStation_UpdateStationInfo,stationName:" + serviceStation?.serviceStationInfo?.serviceStationName);
            return serviceStationMng.ServiceStation_UpdateStationInfo(serviceStation);
        }


        public override void ServiceStation_Remove(IOrganizeConnection  conn)
        {
            string connKey = ""+ conn.GetHashCode();
            var serviceStation = serviceStationMng.ServiceStation_Remove(connKey);
            if (serviceStation != null)
            {
                Logger.Info("[ApiCenterService]Remove serviceStation,stationName:" + serviceStation?.serviceStationInfo?.serviceStationName);
            }
        }


        public bool ServiceStation_Pause(string connKey)
        {
            var serviceStation = serviceStationMng.ServiceStation_Pause(connKey);
            if (serviceStation != null)
            {
                Logger.Info("[ApiCenterService]Pause serviceStation,stationName:" + serviceStation?.serviceStationInfo?.serviceStationName);
            } 
            return serviceStation != null;
        }

        public bool ServiceStation_Start(string connKey)
        {
            var serviceStation = serviceStationMng.ServiceStation_Start(connKey);
            if (serviceStation != null)
            {
                Logger.Info("[ApiCenterService]Start serviceStation,stationName:" + serviceStation?.serviceStationInfo?.serviceStationName);
            }
            return serviceStation != null; 
        }

        public bool ServiceStation_Stop(string connKey)
        {
            var serviceStation = serviceStationMng.ServiceStation_Remove(connKey);
            if (serviceStation != null)
            {
                Logger.Info("[ApiCenterService]Stop serviceStation,stationName:" + serviceStation?.serviceStationInfo?.serviceStationName);
                serviceStation.connection.Close();
            }
            return serviceStation != null;
        }

        #endregion



        #region ApiStation

        public List<ApiStationData> ApiStation_GetAll()
        {
            return apiStationMng.ApiStation_GetAll();
        }

        public bool ApiStation_Pause(string stationName)
        {
            Logger.Info("[ApiCenterService]Pause ApiStation,stationName:" + stationName);
            return apiStationMng.ApiStation_Pause(stationName);
        }

        public bool ApiStation_Start(string stationName)
        {
            Logger.Info("[ApiCenterService]Start ApiStation,stationName:" + stationName);
            return apiStationMng.ApiStation_Start(stationName);
        }
        #endregion


        #region ApiScopeEvent

        /// <summary>
        /// 
        /// </summary>
        List<Func<RpcContextData, ApiMessage, Action<Object, Vit.Core.Util.Pipelines.ByteData>>> apiScopeEventList = null;
        /// <summary>
        /// 在调用api前调用onScope，若onScope返回的结果（onDispose）不为空，则在api调用结束前调用onDispose
        /// </summary>
        /// <param name="apiScopeEvent"></param>
        public void AddApiScopeEvent(Func<RpcContextData, ApiMessage, Action<Object, Vit.Core.Util.Pipelines.ByteData>> apiScopeEvent) 
        {
            if (apiScopeEventList == null) apiScopeEventList=new List<Func<RpcContextData, ApiMessage, Action<Object, Vit.Core.Util.Pipelines.ByteData>>>();

            apiScopeEventList.Add(apiScopeEvent);
        }
        #endregion

    }
}
