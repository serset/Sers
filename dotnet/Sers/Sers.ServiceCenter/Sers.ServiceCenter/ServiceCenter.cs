using Sers.Core.Module.Api.Message;
using Sers.Core.Module.App;
using Sers.Core.Module.Log;
using System;
using System.Reflection;
using Sers.Core.Extensions;
using Sers.Core.Module.PubSub;
using System.Collections.Generic;
using Sers.Core.Module.Api.LocalApi;
using Sers.Core.Module.Serialization;
using Sers.Core.Module.Api;
using Sers.Core.Module.SersDiscovery;
using Sers.ServiceCenter.ApiCenter;
using Sers.Core.Module.Mq.Mq;
using Sers.Core.Module.Mq.MqManager;
using Newtonsoft.Json.Linq;
using Sers.Core.Module.Env;

namespace Sers.ServiceCenter
{
    public class ServiceCenter
    {
        #region static

        public static readonly ServiceCenter Instance = new ServiceCenter();

        public static bool IsRunning => SersApplication.IsRunning;




        //(x.1) Init
        public static void Init()
        {
            Instance.InitCenter();
        }



        #region (x.2)Discovery

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



        //(x.3)
        public static bool Start()
        {
            return Instance.StartCenter();
        }


        //(x.4)
        public static void RunAwait()
        {
            SersApplication.RunAwait();
        }


        //(x.5)
        public static void Stop()
        {
            Instance.StopCenter();
        }


        #endregion



        #region (x.1) 成员对象


        public readonly ServerMqManager mqMng = new ServerMqManager();

        public readonly ApiCenterService apiCenterService = new ApiCenterService();
 
        public readonly LocalApiService localApiService = LocalApiService.Instance; 


        #region mqConnForLocalStationService        

        class MqConn : IMqConn
        {
            public string connTag { get; set; } = "ServiceCenter";

            /// <summary>
            /// 连接状态(0:waitForCertify; 2:certified; 4:waitForClose; 8:closed;)
            /// </summary>
            public byte state { get; set; } = MqConnState.certified;

            public void SendFrameAsync(List<ArraySegment<byte>> messageData)
            {
                var msgFrame = messageData.ByteDataToBytes();
                if (msgFrame != null && msgFrame.Length > 2 && msgFrame[0] == (byte)Core.Module.Mq.MqManager.EFrameType.message)
                {
                    MessageClient.Instance.OnGetMessage(this,new ArraySegment<byte>(msgFrame, 2, msgFrame.Length - 2));
                }
            }

            public void Close()
            {
                Logger.Info("[ServiceCenter] mqConnForLocalStationService  Close");
            }
        }

        IMqConn mqConnForLocalStationService = new MqConn();
        #endregion

        #endregion



        #region (x.2) InitCenter

        public void InitCenter()
        {

            Logger.log.InitByConfigurationManager();

            //初始化序列化配置
            Serialization.Instance.InitByConfigurationManager();

            Logger.Info("初始化ServiceCenter...");

            #region (x.1)mqMng add Builder for Iocp、ThreadWait            
            mqMng.BeforeBuildMq = (JObject[] configs, List<IServerMq> mqList) =>
            {
                foreach (var config in configs)
                {
                    var className = config["className"].ConvertToString();
                    if (className == "Sers.Mq.Socket.Iocp.MqBuilder.ServerMqBuilder")
                    {
                        new Sers.Mq.Socket.Iocp.MqBuilder.ServerMqBuilder().BuildMq(mqList, config);
                        config["className"] = null;
                    }
                    else if (className == "Sers.Mq.Socket.ThreadWait.MqBuilder.ServerMqBuilder")
                    {
                        new Sers.Mq.Socket.ThreadWait.MqBuilder.ServerMqBuilder().BuildMq(mqList, config);
                        config["className"] = null;
                    }
                }
            };
            #endregion

            //(x.2)localApiService
            localApiService.Init();

            //(x.3)UsageReporter
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


        #region (x.4) StartCenter 
        public bool StartCenter()
        {
            Logger.Info("[ServiceCenter] starting ...");


            #region (x.1)注册主程序退出回调
            AppDomain.CurrentDomain.ProcessExit += (s, e) => {
                StopCenter();
            };
            #endregion


            #region (x.2)Mq 注册 回调

            mqMng.station_OnGetRequest = apiCenterService.CallApiAsync;


            mqMng.Conn_OnConnected = (IMqConn mqConn) =>
              {
                  Logger.Info("[ServerMq] OnConnected");
              };

            mqMng.Conn_OnDisconnected = (IMqConn mqConn) =>
            {
                Logger.Info("[ServerMq] OnDisconnected,connTag:" + mqConn.connTag);

                MessageCenterService.Instance.Conn_OnDisconnected(mqConn);

                apiCenterService.ServiceStation_Remove(mqConn);
            };
            #endregion


            #region (x.3)注册 localApiService 到 apiCenterService
            {
                Logger.Info("[ServiceCenter] regist localApiService to apiCenterService...");


                var serviceStationData = new
                {
                    serviceStationInfo = SersApplication.serviceStationInfo,
                    deviceInfo = SersApplication.GetDeviceInfo(),
                    localApiService.apiNodes
                };

                var serviceStation = serviceStationData.ConvertBySerialize<ServiceStation>();


                serviceStation.mqConn = mqConnForLocalStationService;
                serviceStation.OnSendRequest = (IMqConn conn, Object sender, List<ArraySegment<byte>> requestData, Action<object, List<ArraySegment<byte>>> callback) =>
                {
                    localApiService.CallApiAsync(sender, new ApiMessage(requestData.ByteDataToArraySegment()), (sender_, apiReplyMessage) => {
                        callback(sender_, apiReplyMessage.Package());
                    });
                };

                apiCenterService.ServiceStation_Regist(serviceStation);
            }
            #endregion


            #region (x.4)Mq Start 
            Logger.Info("[ServerMq] starting...");

            if (!mqMng.Start())
            {
                Logger.Info("[ServerMq] start - failed");
                return false;
            }
            Logger.Info("[ServerMq] started");
            #endregion

            localApiService.Start();


            #region (x.5) 初始化ApiClient
            Func<List<ArraySegment<byte>>, ArraySegment<byte>> OnSendRequest = ((List<ArraySegment<byte>> apiReqMessage) =>
            {
                apiCenterService.CallApi(mqConnForLocalStationService, apiReqMessage.ByteDataToArraySegment(), out var replyData, mqMng.requestAdaptor.requestTimeoutMs);
                return replyData.ByteDataToArraySegment();
            });

            ApiClient.SetOnSendRequest(new[] { OnSendRequest });
            #endregion



            #region (x.6) 初始化消息订阅 MessageClient         
            //MessageCenterService.Instance.OnSendMessage += (IMqConn conn, List<ArraySegment<byte>> messageData) =>
            //{
            //    if (conn != mqConnForLocalStationService) return;
            //    MessageClient.Instance.OnGetMessage(conn,messageData.ByteDataToBytes().BytesToArraySegmentByte());
            //};

            MessageClient.Instance.OnSendMessage = (List<ArraySegment<byte>> messageData) =>
            {
                MessageCenterService.Instance.OnGetMessage(mqConnForLocalStationService, messageData.ByteDataToBytes().BytesToArraySegmentByte());
            };
            #endregion

            #region (x.7) 初始化消息订阅 MessageCenterService
            mqMng.station_OnGetMessage = MessageCenterService.Instance.OnGetMessage;
            MessageCenterService.Instance.OnSendMessage += mqMng.Station_SendMessageAsync;
            #endregion


            //(x.8) 调用SersApp 事件
            SersApplication.ResistConsoleCancelKey(Stop);
            SersApplication.OnStart();


            Logger.Info("[ServiceCenter] started");

            return true;
        }
        #endregion



        #region (x.5) StopCenter
        public void StopCenter()
        {
            if (!SersApplication.IsRunning) return;

            Logger.Info("[ServiceCenter] stop...");


            #region Mq Close
            Logger.Info("[ServerMq] stop...");
            try
            {
                mqMng.Stop();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            Logger.Info("[ServerMq] stoped");
            #endregion

            try
            {
                localApiService.Stop();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
 

            Logger.Info("[ServiceCenter] stoped");

            //调用SersApp 事件
            SersApplication.OnStop();
        }
        #endregion


    }
}
