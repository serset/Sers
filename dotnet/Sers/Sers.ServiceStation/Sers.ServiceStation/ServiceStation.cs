using Newtonsoft.Json;
using Sers.Core.Extensions;
using Sers.Core.Module.Api.Message;
using Sers.Core.Module.Api;
using Sers.Core.Module.App;
using Sers.Core.Module.Log;
using System;
using System.Reflection;
using Sers.Core.Module.Api.LocalApi;
using Sers.Core.Module.PubSub.ShareEndpoint.Sys;
using Sers.Core.Module.Serialization;
using Sers.Core.Module.SersDiscovery;
using Sers.Core.Mq.Mng;
using Sers.Core.Util.ConfigurationManager;
using System.Collections.Generic;
using Sers.Core.Module.Api.Data;

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

            //#region 使用扩展消息队列            
            //ServiceStation.Instance.mqMng.UseZmq();
            //#endregion

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

        
        public readonly ClientMqMng mqMng = new ClientMqMng();

        public readonly LocalApiMng localApiMng = new LocalApiMng();

        /// <summary>
        /// 站点Start回调
        /// </summary>
        readonly List<Action> actionsOnStart = new List<Action>();

        /// <summary>
        /// 站点Stop回调
        /// </summary>
        readonly List<Action> actionsOnStop = new List<Action>();


        public static bool IsRunning => SersApplication.IsRunning;

        public ServiceStation()
        {
        }
        #endregion



        #region (x.2) InitStation

        
        public void InitStation()
        {
            Logger.Info("初始化ServiceStation...");


            if (true == ConfigurationManager.Instance.GetByPath<bool?>("Sers.ServiceStation.Console_PrintLog"))
            {
                Logger.OnLog = (level, msg) => { Console.WriteLine("[" + level + "]" + msg); };
            }


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



        #region (x.5) StartStation

        
        public bool StartStation()
        {
            SersApplication.IsRunning = false;


            #region (x.1) 注册 Mq 回调

            mqMng.OnReceiveRequest = (oriData) =>
            {
                return localApiMng.CallLocalApi(new ApiMessage(oriData)).Package();
            };

            mqMng.Conn_OnDisconnected = () =>
            {
                StopStation();
            };
            #endregion


            #region (x.2) ClientMq 连接服务器

            Logger.Info("[ClientMq] 准备连接服务器");
            
            if (!mqMng.Connect())
            {
                Logger.Info("[ClientMq] 连接服务器 失败");
                return false;
            }

            Logger.Info("[ClientMq] 连接服务器 成功");
            #endregion


            //(x.3) 初始化ApiClient
            ApiClient.Instance.OnSendRequest = mqMng.SendRequest;


            #region (x.4)向服务中心注册ServiceStation
            //if (0 < localApiMng.apiCount)
            {
                Logger.Info("向服务中心注册ServiceStation...");

                var serviceStationData = new
                {                
                    serviceStationInfo = SersApplication.serviceStationInfo,
                    deviceInfo = SersApplication.GetDeviceInfo(),
                    localApiMng.apiNodes
                }.Serialize();

                if (true == ConfigurationManager.Instance.GetByPath<bool?>("Sers.ServiceStation.StationRegist_PrintRegistArg"))
                {
                    Logger.Info("[StationRegist] arg:" + serviceStationData);
                }


                ApiReturn ret;
                try
                {
                    ret = ApiClient.Instance.CallApi<ApiReturn>("/_sys_/serviceStation/regist", serviceStationData);
                }
                catch (Exception ex)
                {
                    Logger.Error("向服务中心注册本地Api 失败", ex);
                    return false;
                }

                if (!ret.success)                
                {
                    Logger.Info("向服务中心注册本地Api 失败。返回结果：" + ret.Serialize());
                    return false;
                }

                Logger.Info("向服务中心注册本地Api 成功");

            }
            #endregion


            #region (x.5)PubSub 初始化消息订阅 PubSubClient
            mqMng.OnReceiveMessage = PubSubClient.Instance.OnReceiveMessage;
            PubSubClient.Instance.OnSendMessage = mqMng.SendMessage;
            #endregion


            #region (x.6)调用站点Start回调            
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


            Logger.Info("ServiceStation启动成功。StationName:"+ SersApplication.serviceStationInfo.serviceStationName);
            SersApplication.IsRunning = true;
            SersApplication.ResistConsoleCancelKey(Stop);
            return true;
        }
        #endregion



        #region (x.6) StopStation


        public void StopStation()
        {
            if (!SersApplication.IsRunning) return;
            Logger.Info("站点关闭...");

            //Mq Close
            try
            {
                mqMng.Close();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }


            #region 调用站点关闭回调            
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

            Logger.Info("站点已关闭");

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
