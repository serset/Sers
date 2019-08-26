using Newtonsoft.Json;
using Sers.Core.Extensions;
using Sers.Core.Module.Api.Message;
using Sers.Core.Module.Api;
using Sers.Core.Module.App;
using Sers.Core.Module.Log;
using System;
using System.Reflection;
using Sers.Core.Module.Api.LocalApi;
using Sers.Core.Module.PubSub;
using Sers.Core.Module.Serialization;
using Sers.Core.Module.SersDiscovery;
using Sers.Core.Util.ConfigurationManager;
using System.Collections.Generic;
using Sers.Core.Module.Api.Data;
using Sers.Core.Module.Mq.MqManager;
using Sers.Core.Module.Mq.Mq;
using System.Linq;
using Newtonsoft.Json.Linq;
using Sers.Core.Module.Env;

namespace Sers.ServiceStation
{

    [JsonObject(MemberSerialization.OptIn)]
    public class ServiceStation
    {

        #region static
 
        public static readonly ServiceStation Instance = new ServiceStation();

        /// <summary>
        /// 使用默认步骤启动站点
        /// </summary>
        public static void AutoRun()
        {
            ServiceStation.Init();

            //ServiceStation.Discovery(typeof(Program).Assembly);
            ServiceStation.Discovery();

            ServiceStation.Start();

            ServiceStation.RunAwait();
        }



        #region (x.1) Init

        /// <summary>
        /// 初始化ServiceStation
        /// </summary>
        public static void Init()
        {
            Instance.InitStation();
        }
        #endregion


        #region (x.2) Discovery


        /// <summary>
        /// 查找服务,可多次调用
        /// </summary> 
        /// <param name="config"></param>
        public static void Discovery(DiscoveryConfig config)
        {
            Instance.DiscoveryApi(config);
        }



        /// <summary>
        /// 查找服务,可多次调用
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="config"></param>
        public static void Discovery(Assembly assembly, DiscoveryConfig config = null)
        {
            Instance.DiscoveryApi(assembly, config);
        }



        /// <summary>
        /// 从配置文件(appsettings.json  Sers.ApiStation.DiscoveryConfig )获取服务发现配置并查找服务
        /// </summary>
        public static void Discovery()
        {
            Instance.DiscoveryApi();
        }

        #endregion


        //(x.3) Start
        public static bool Start()
        {
            return Instance.StartStation();
        }


        //(x.4) Stop
        public static void Stop()
        {
            Instance.StopStation();
        }


        //(x.5) RunAwait
        public static void RunAwait()
        {
            SersApplication.RunAwait();
        }


        #endregion


        #region (x.1) 成员对象
        readonly ClientMqManager mqMng = new ClientMqManager();
 

        public readonly LocalApiService localApiService = LocalApiService.Instance;

        

        public static bool IsRunning => SersApplication.IsRunning;

        public ServiceStation()
        {
        }
        #endregion



        #region (x.2) InitStation

        public void InitStation()
        {

            Logger.log.InitByConfigurationManager();

            //初始化序列化配置
            Serialization.Instance.InitByConfigurationManager();

            Logger.Info("[ServiceStation] init...");

            #region (x.1)mqMng add Builder for Iocp、ThreadWait
            mqMng.BeforeBuildMq = (JObject[] configs, List<IClientMq > mqList) => 
            {
                foreach (var config in configs)
                {
                    var className = config["className"].ConvertToString();
                    if (className == "Sers.Mq.Socket.Iocp.MqBuilder.ClientMqBuilder")
                    {
                        new Sers.Mq.Socket.Iocp.MqBuilder.ClientMqBuilder().BuildMq(mqList, config);
                        config["className"] = null;
                    }
                    else if (className == "Sers.Mq.Socket.ThreadWait.MqBuilder.ClientMqBuilder")
                    {
                        new Sers.Mq.Socket.ThreadWait.MqBuilder.ClientMqBuilder().BuildMq(mqList, config);
                        config["className"] = null;
                    }
                }                
            };
            #endregion

            //(x.2) localApiService
            localApiService.Init();

            //(x.3) UsageReporter
            UsageReporter.UseUsageReporter();

        }
        #endregion



        #region (x.3) DiscoveryApi

        /// <summary>
        /// 查找服务,可多次调用
        /// </summary> 
        /// <param name="config"></param>
        public void DiscoveryApi(DiscoveryConfig config)
        {
            localApiService.Discovery(config);
        }


        /// <summary>
        /// 查找服务,可多次调用
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="config"></param>
        public void DiscoveryApi(Assembly assembly, DiscoveryConfig config = null)
        {
            localApiService.Discovery(assembly, config);
        }



        /// <summary>
        /// 从配置文件(appsettings.json  Sers.ApiStation.DiscoveryConfig )获取服务发现配置并查找服务
        /// </summary>
        public void DiscoveryApi()
        {
            localApiService.Discovery();
        }

        #endregion



        #region (x.4) StartStation

       
        public bool StartStation()
        {

            #region (x.1) 注册 Mq 回调
            mqMng.station_OnGetRequest = (IMqConn conn,Object sender, ArraySegment<byte> requestData, Action<object, List<ArraySegment<byte>>> callback) => 
            {
                //ServerMqMng.CurMqConnGuid = conn.connGuid;
                localApiService.CallApiAsync(sender, new ApiMessage(requestData), (object sender1, ApiMessage apiReplyMessage) =>
                 {
                     callback(sender1, apiReplyMessage.Package());
                 });
            };

            mqMng.Conn_OnDisconnected = (conn) =>
            {
                StopStation();
            };
            #endregion


            #region (x.2) ClientMq 连接服务器

            Logger.Info("[ClientMq] Connect - trying...");
            
            if (!mqMng.Start())
            {
                Logger.Info("[ClientMq] Connect - failed");
                return false;
            }

            Logger.Info("[ClientMq] Connect - succeed");
            #endregion



            #region (x.3) 初始化ApiClient
            ApiClient.SetOnSendRequest(mqMng.mqConns.Select<IMqConn, Func<List<ArraySegment<byte>>, ArraySegment<byte>>>(
                conn =>
                {
                    return (req) => { mqMng.Station_SendRequest(req, out var reply, conn); return reply.ByteDataToArraySegment(); };
                }
                ).ToArray());
            #endregion


            //(x.4) 启动 localApiService服务
            localApiService.Start();

            #region (x.5)向服务中心注册localApiService
            //if (0 < localApiMng.apiCount)
            {
                Logger.Info("[ServiceStation] regist serviceStation to ServiceCenter...");

                var serviceStationData = new
                {                
                    serviceStationInfo = SersApplication.serviceStationInfo,
                    deviceInfo = SersApplication.GetDeviceInfo(),
                    localApiService.apiNodes
                }.Serialize();

                if (true == ConfigurationManager.Instance.GetByPath<bool?>("Sers.ServiceStation.StationRegist_PrintRegistArg"))
                {
                    Logger.Info("[ServiceStation] regist - arg:" + serviceStationData);
                }


               
                try
                {
                    foreach (var apiClient in ApiClient.Instances)
                    {
                        ApiReturn ret = apiClient.CallApi<ApiReturn>("/_sys_/serviceStation/regist", serviceStationData);

                        if (!ret.success)
                        {
                            Logger.Info("[ServiceStation] regist - failed. reply: " + ret.Serialize());
                            return false;
                        }
                    }                   
                }
                catch (Exception ex)
                {
                    Logger.Error("[ServiceStation] regist - failed", ex);
                    return false;
                }               

                Logger.Info("[ServiceStation] regist - succeed");

            }
            #endregion


            #region (x.6)PubSub 初始化消息订阅 PubSubClient
            mqMng.station_OnGetMessage = MessageClient.Instance.OnGetMessage;
           
            MessageClient.Instance.OnSendMessage = mqMng.Station_SendMessageAsync;
            #endregion
                            

            //(x.7) 调用SersApp 事件
            SersApplication.ResistConsoleCancelKey(Stop);
            SersApplication.OnStart();

            Logger.Info("[ServiceStation] started - stationName:" + SersApplication.serviceStationInfo.serviceStationName);
           
            return true;
        }
        #endregion



        #region (x.5) StopStation


        public void StopStation()
        {
            if (!SersApplication.IsRunning) return;
            Logger.Info("[ServiceStation] stoping...");

            //Mq Close
            try
            {
                mqMng.Stop();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            try
            {
                localApiService.Stop();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            Logger.Info("[ServiceStation] stoped");

            //调用SersApp 事件
            SersApplication.OnStop();
        }
        #endregion



       


    }
}
