using Sers.Core.Module.Api.Message;
using Sers.Core.Module.App;
using Sers.Core.Module.Log;
using System;
using System.Reflection;
using Sers.Core.Extensions;
using Sers.Core.Module.PubSub.ShareEndpoint.Sys;
using System.Collections.Generic;
using Sers.Core.Module.Api.LocalApi;
using Sers.Core.Module.Serialization;
using Sers.Core.Mq.Mng;
using Sers.Core.Util.ConfigurationManager;
using Sers.Core.Module.Api;
using Sers.Core.Module.SersDiscovery;

namespace Sers.ServiceCenter
{
    public class ServiceCenter
    {
        #region static
    
        public static readonly ServiceCenter Instance =  new ServiceCenter();

        public static bool IsRunning => SersApplication.IsRunning;




        //(x.1) Init
        public static void Init()
        {
            if (true == ConfigurationManager.Instance.GetByPath<bool?>("Sers.ServiceCenter.Console_PrintLog"))
            {
                Logger.OnLog = (level, msg) => { Console.WriteLine("[" + level + "]" + msg); };
            }

            Logger.Info("初始化ServiceCenter...");

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

        public readonly ServerMqMng mqMng  = new ServerMqMng();
    
        public readonly ApiCenter.ApiCenter apiCenter =new ApiCenter.ApiCenter();

        public readonly LocalApiMng localApiMng = new LocalApiMng();


        /// <summary>
        /// 站点Start回调
        /// </summary>
        readonly List<Action> actionsOnStart = new List<Action>();

        /// <summary>
        /// 站点Stop回调
        /// </summary>
        readonly List<Action> actionsOnStop = new List<Action>();


        private const string ServiceCenterMqConnGuid = "ServiceCenter";

        #endregion



        #region (x.2) InitCenter

        public void InitCenter()
        {
            //初始化序列化配置
            Serialization.Instance.InitByConfigurationManager();

            mqMng.UseSocket();

            localApiMng.UseSsApiDiscovery();

            localApiMng.UseSubscriberDiscovery();

            localApiMng.UseApiTrace();

            UseUsageReporter();
        }
        #endregion



        #region (x.3) 站点回调管理


        /// <summary>
        /// 添加站点Start回调
        /// </summary>
        /// <param name="action"></param>
        public void AddActionOnStart(Action action)
        {
            actionsOnStart.Add(action);
        }

        /// <summary>
        /// 添加站点关闭回调
        /// </summary>
        /// <param name="action"></param>
        public void AddActionOnStop(Action action)
        {
            actionsOnStop.Add(action);
        }
        #endregion



        #region (x.4) DiscoveryApi

        /// <summary>
        /// 查找服务,可多次调用
        /// </summary> 
        /// <param name="config"></param>
        public void DiscoveryApi(DiscoveryConfig config)
        {
            localApiMng.Discovery(config);
        }


        /// <summary>
        /// 查找服务,可多次调用
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="config"></param>
        public void DiscoveryApi(Assembly assembly, DiscoveryConfig config = null)
        {
            localApiMng.Discovery(assembly, config);
        }



        /// <summary>
        /// 从配置文件(appsettings.json  Sers.ApiStation.DiscoveryConfig )获取服务发现配置并查找服务
        /// </summary>
        public void DiscoveryApi()
        {
            localApiMng.Discovery();
        }

        #endregion



        #region (x.5) StartCenter
        public bool StartCenter()
        {
            Logger.Info("启动 ServiceCenter...");


            #region (x.1)注册主程序退出回调
            AppDomain.CurrentDomain.ProcessExit += (s, e) => {
                StopCenter();
            };
            #endregion


            #region (x.2)Mq 注册 回调

            mqMng.OnReceiveRequest =
              (string connGuid, ArraySegment<byte> oriData) =>
              {
                  ServerMqMng.CurMqConnGuid = connGuid;
                  return apiCenter.CallApi(oriData);
              };

        
            mqMng.Conn_OnConnected=(string connGuid) => 
            {
                Logger.Info("[消息队列]新的连接，connGuid:"+ connGuid); 
            };

            mqMng.Conn_OnDisconnected = (string connGuid) =>
            {
                Logger.Info("[消息队列]连接断开，connGuid:" + connGuid);

                PubSubServer.Instance.Conn_OnDisconnected(connGuid);

                apiCenter.ServiceStation_Remove(connGuid);
            };
            #endregion


            #region (x.3)Local SysApi

            #region 注册本地Api 到 apiCenter        
            Logger.Info("注册本地Api..."); 


            var serviceStationData = new
            {
                serviceStationInfo = SersApplication.serviceStationInfo,
                deviceInfo = SersApplication.GetDeviceInfo(),
                localApiMng.apiNodes
            }; 

            var serviceStation = serviceStationData.ConvertBySerialize<ServiceStation>();
        

            serviceStation.mqConnGuid = ServiceCenterMqConnGuid;
            serviceStation.OnSendRequest = (string connGuid, ApiMessage apiReqMessage) => 
            {
                return localApiMng.CallLocalApi(apiReqMessage).Package();
            };

            apiCenter.ServiceStation_Regist(serviceStation);
            #endregion

            #endregion


            #region (x.4)Mq Start 
            Logger.Info("ServerMq 打开消息通道...");         

            if (!mqMng.Start())
            {
                Logger.Info("ServerMq 消息通道打开 失败");
                return false;
            }
            Logger.Info("ServerMq 消息通道打开 成功。");
            #endregion


            //(x.5) 初始化ApiClient
            ApiClient.Instance.OnSendRequest = (List<ArraySegment<byte>> apiReqMessage) => {
                
                ArraySegment<byte> oriData = new ArraySegment<byte>(apiReqMessage.ByteDataToBytes());
                var replyData= apiCenter.CallApi(oriData);

                ArraySegment<byte> apiReplyMessage = new ArraySegment<byte>(replyData.ByteDataToBytes());
                return apiReplyMessage;
            };
         




            #region PubSub 初始化消息订阅 PubSubServer
            mqMng.OnReceiveMessage = PubSubServer.Instance.OnReceiveMessage;
            PubSubServer.Instance.OnSendMessage += mqMng.SendMessage;
            #endregion

            #region PubSub 初始化消息订阅 PubSubClient
      
            PubSubServer.Instance.OnSendMessage += (string connGuid, List<ArraySegment<byte>> messageData) =>
            {
                if (connGuid != ServiceCenterMqConnGuid) return;
                PubSubClient.Instance.OnReceiveMessage(messageData.ByteDataToBytes().BytesToArraySegmentByte());
            };

            PubSubClient.Instance.OnSendMessage = (List<ArraySegment<byte>> messageData) =>
            {
                PubSubServer.Instance.OnReceiveMessage(ServiceCenterMqConnGuid, messageData.ByteDataToBytes().BytesToArraySegmentByte());
            };
            #endregion



            #region 调用站点Start回调            
            try
            {
                foreach (var action in actionsOnStart)
                {
                    try
                    {
                        action?.Invoke();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            #endregion


            Logger.Info("ServiceCenter 启动 成功。");

            SersApplication.IsRunning = true;
            SersApplication.ResistConsoleCancelKey(ServiceCenter.Stop);

            return true;
        }
        #endregion



        #region (x.6) StopCenter
        public void StopCenter()
        {
            if (!SersApplication.IsRunning) return;

            Logger.Info("ServiceCenter 关闭...");


            #region Mq Close
            Logger.Info("ServerMqMng 关闭消息通道");
            try
            {
                mqMng.Close();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            Logger.Info("ServiceCenter 已关闭");
            #endregion


            #region 调用站点Stop回调            
            try
            {
                foreach (var action in actionsOnStop)
                {
                    try
                    {
                        action?.Invoke();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            #endregion

            Logger.Info("ServiceCenter已关闭");
            SersApplication.Stop();
        }
        #endregion



        #region (x.7) UseUsageReporter
        /// <summary>
        /// 自动上报Usage任务
        /// </summary>
        public void UseUsageReporter()
        {
            if (true != ConfigurationManager.Instance.GetByPath<bool?>("Sers.ServiceStation.UsageReporter"))
            {
                return;
            }

            AddActionOnStart(Sers.Core.Module.Env.UsageReporter.StartReportTask);
            AddActionOnStop(Sers.Core.Module.Env.UsageReporter.StopReportTask);
        }

        #endregion





    }
}
