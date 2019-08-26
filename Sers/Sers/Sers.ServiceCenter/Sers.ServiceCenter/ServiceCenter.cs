using Sers.Core.Module.Api.Message;
using Sers.Core.Module.App;
using Sers.Core.Util.Ioc;
using Sers.Core.Module.Log;
using System;
using System.Reflection;
using Sers.Core.Extensions;
using Sers.Core.Module.PubSub.ShareEndpoint.Sys;
using System.Collections.Generic;
using Sers.Core.Module.Api.LocalApi;
using Sers.Core.Module.Serialization;
using Sers.Core.Util.Common;
using Sers.Core.Mq.Mng;
using Sers.Core.Util.ConfigurationManager;
using Sers.Core.Module.Env;
using Sers.Core.Module.Api;

namespace Sers.ServiceCenter
{
    public class ServiceCenter
    {
        #region static

        public static bool IsRunning => SersApplication.IsRunning;
        public static void Init()
        {
            if (true == ConfigurationManager.Instance.GetByPath<bool?>("Sers.ServiceCenter.Console_PrintLog"))
            {
                Logger.OnLog = (level, msg) => { Console.WriteLine("[" + level + "]" + msg); };
            }

            Logger.Info("初始化ServiceCenter...");     

            IocHelp.Update();

            Instance.InitCenter();
        }

       

        public static void Discovery(Assembly Assembly)
        {        
            Instance.DiscoveryAssembly(Assembly);       
        }
        public static bool Start()
        {
            return Instance.StartCenter();
        }
        public static void RunAwait()
        {
            SersApplication.RunAwait();
        }
        public static void Stop()
        {
            _Instance.StopCenter();
            _Instance = null;              
        }
        


        private static ServiceCenter _Instance;
        public static ServiceCenter Instance => _Instance??( _Instance= new ServiceCenter());


        #endregion


        #region 站点Start回调
        readonly List<Action> actionsOnStart = new List<Action>();

        /// <summary>
        /// 添加站点Start回调
        /// </summary>
        /// <param name="action"></param>
        public void AddActionOnStart(Action action)
        {
            actionsOnStart.Add(action);
        }
        #endregion


        #region 站点Stop回调
        readonly List<Action> actionsOnStop = new List<Action>();

        /// <summary>
        /// 添加站点关闭回调
        /// </summary>
        /// <param name="action"></param>
        public void AddActionOnStop(Action action)
        {
            actionsOnStop.Add(action);
        }
        #endregion


        public readonly ServerMqMng mqMng  = new ServerMqMng();

        private ApiCenter.ApiCenter _apiCenter=new ApiCenter.ApiCenter();
        public ApiCenter.ApiCenter apiCenter => _apiCenter;



        public readonly LocalApiMng localApiMng = new LocalApiMng();

      


        public void InitCenter()
        {
            #region 初始化序列化字符编码
            var encoding = ConfigurationManager.Instance.GetByPath<string>("Sers.Serialization.Encoding");
            if (!string.IsNullOrWhiteSpace(encoding))
            {
                try
                {
                    Serialization.Instance.SetEncoding(encoding.StringToEnum<EEncoding>());
                }
                catch
                {
                }
            }
            #endregion


            mqMng.UseSocket();

            localApiMng.UseSsApiDiscovery();
            localApiMng.UseSubscriberDiscovery();

            localApiMng.UseApiTrace();

            UseUsageReporter();
        }

        public void DiscoveryAssembly(Assembly Assembly)
        {
            Logger.Info("查找本地Api,程序集:[" + Assembly.FullName + "]");
            localApiMng.Discovery(Assembly);
        }

        private const string ServiceCenterMqConnGuid = "ServiceCenter";
        public bool StartCenter()
        {
            Logger.Info("启动 ServiceCenter...");


            #region (x.1)注册主程序退出回调
            AppDomain.CurrentDomain.ProcessExit += (s, e) => {
                StopCenter();
            };
            #endregion


            #region (x.2)Mq 注册 回调

            mqMng.OnException = Logger.log.Error;

            mqMng.OnReceiveRequest =
              (string connGuid, ArraySegment<byte> oriData) =>
              {
                  ServerMqMng.CurMqConnGuid = connGuid;
                  return _apiCenter.CallApi(oriData);
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
                serviceStationInfo = SersApplication.GetServiceStationInfo(),
                deviceInfo = SersApplication.GetDeviceInfo(),
                localApiMng.apiNodes
            }; 

            var serviceStation = serviceStationData.ConvertBySerialize<ServiceStation>();
        

            serviceStation.mqConnGuid = ServiceCenterMqConnGuid;
            serviceStation.OnSendRequest = (string connGuid, ApiMessage apiReqMessage) => 
            {
                return localApiMng.CallLocalApi(apiReqMessage).Package();                
            };

            _apiCenter.ServiceStation_Regist(serviceStation);
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
            ApiClient.Static_SendRequest = (List<ArraySegment<byte>> apiReqMessage) => {
                
                ArraySegment<byte> oriData = new ArraySegment<byte>(apiReqMessage.ByteDataToBytes());
                var replyData=_apiCenter.CallApi(oriData);

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
                        Logger.log.Error(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex);
            }
            #endregion


            Logger.Info("ServiceCenter 启动 成功。");

            SersApplication.IsRunning = true;
            SersApplication.ResistConsoleCancelKey(ServiceCenter.Stop);

            return true;
        }

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
                Logger.log.Error(ex);
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
                        Logger.log.Error(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex);
            }
            #endregion

            Logger.Info("ServiceCenter已关闭");
            SersApplication.Stop();
        }




    }
}
