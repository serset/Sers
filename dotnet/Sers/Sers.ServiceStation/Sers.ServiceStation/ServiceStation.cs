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
    public class ServiceStation: IDisposable
    {
        #region static 


        public static void AutoRun(Assembly Assembly)
        {
            ServiceStation.Init();

            //#region 使用扩展消息队列            
            //ServiceStation.Instance.mqMng.UseZmq();
            //#endregion

            ServiceStation.Discovery(Assembly);

            ServiceStation.Start();

            ServiceStation.RunAwait();
        }




        /// <summary>
        /// 初始化ServiceStation
        /// </summary>
        public static void Init()
        {
            if (true == ConfigurationManager.Instance.GetByPath<bool?>("Sers.ServiceStation.Console_PrintLog"))
            {
                Logger.OnLog = (level, msg) => { Console.WriteLine("[" + level + "]" + msg); };
            }

            //IocHelp.Update();

            Instance.InitStation();
        }

        /// <summary>
        /// 查找本地对象,可多次调用
        /// </summary>
        /// <param name="Assembly"></param>
        public static void Discovery(Assembly Assembly)
        {
            Logger.Info("Discovery,程序集:[" + Assembly.FullName+"]");
            Instance.localApiMng.Discovery(new DiscoveryConfig { assembly = Assembly });
        }

        /// <summary>
        /// 查找本地对象,可多次调用
        /// </summary>
        /// <param name="Assembly"></param>
        /// <param name="station"></param>
        public static void Discovery(Assembly Assembly,string station)
        {
            Logger.Info("Discovery,程序集:[" + Assembly.FullName + "]");
            Instance.localApiMng.Discovery(new DiscoveryConfig {apiStationName= station,assembly= Assembly });
        }


        /// <summary>
        /// 查找本地对象,可多次调用
        /// </summary> 
        /// <param name="config"></param>
        public static void Discovery(DiscoveryConfig config)
        {
            Logger.Info("Discovery,程序集:[" + config.assembly?.FullName + "]");
            Instance.localApiMng.Discovery(config);
        }



        public static bool Start()
        {
            return Instance.StartStation();
        }

        public static void RunAwait()
        {
            SersApplication.RunAwait();
        }
        public static void Stop()
        {           
            _Instance.Dispose();
            _Instance = null;           
        }


        static ServiceStation _Instance;
        public static ServiceStation Instance => _Instance ?? (_Instance = new ServiceStation());

        #endregion


        public ServiceStation()
        {
        }

        public readonly ClientMqMng mqMng = new ClientMqMng();

        public readonly LocalApiMng localApiMng = new LocalApiMng();

        public static bool IsRunning => SersApplication.IsRunning;

        public void InitStation()
        {
            Logger.Info("初始化ServiceStation...");

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
        


        public bool StartStation()
        {
            SersApplication.IsRunning = false;


            #region (x.1) 注册 Mq 回调           

            mqMng.OnReceiveRequest = (oriData) =>
            {
                return localApiMng.CallLocalApi(new ApiMessage(oriData)).Package();
            };

            mqMng.Conn_OnDisconnected = () => {
                ServiceStation.Stop();
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
            ApiClient.Static_SendRequest = mqMng.SendRequest;



            #region (x.4)向服务中心注册ServiceStation
            //if (0 < localApiMng.apiCount)
            {
                Logger.Info("向服务中心注册ServiceStation...");

                var serviceStationData = new
                {                
                    serviceStationInfo = SersApplication.GetServiceStationInfo(),
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


            Logger.Info("ServiceStation启动成功。StationName:"+ ConfigurationManager.Instance.GetByPath<string>("Sers.ServiceStation.StationInfo.StationName"));
            SersApplication.IsRunning = true;
            SersApplication.ResistConsoleCancelKey(Stop);
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



        public void Dispose()
        {
            try
            {
                mqMng.Dispose();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }


            try
            {
                StopStation();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            } 
        }

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
    }
}
