using Newtonsoft.Json;
using Vit.Extensions;
using Sers.Core.Module.Api;
using Sers.Core.Module.App;
using Vit.Core.Module.Log;
using System;
using System.Reflection;
using Sers.Core.Module.Api.LocalApi;
using Sers.Core.Module.PubSub;
using Vit.Core.Util.ConfigurationManager;
using System.Collections.Generic;
using System.Linq;
using Sers.Core.CL.CommunicationManage;
using Sers.Core.CL.MessageOrganize;
using Sers.Core.Module.Env;
using Sers.Core.Module.Message;
using Vit.Core.Util.ComponentModel.Data;
using System.Threading.Tasks;
using Sers.SersLoader;

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
            //手动指定Station版本号
            //Sers.Core.Module.App.SersApplication.serviceStationInfo.stationVersion = "2.0.1";


            //(x.1) Init
            ServiceStation.Init();

            //(x.2) Discovery
            //ServiceStation.Instance.LoadSersApi(typeof(Program).Assembly);
            //ServiceStation.Instance.LoadSersApi(typeof(Program).Assembly, new Sers.Core.Module.ApiLoader.ApiLoaderConfig { apiStationName = "Demo" });
            ServiceStation.Instance.LoadApi();

            //(x.3) Start
            ServiceStation.Start();


            //(x.4) RunAwait
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
       


   
        #region (x.2) Start        
        /// <summary>
        /// 是否成功启动站点
        /// </summary>
        /// <returns></returns>
        public static bool Start()
        {
            return Instance.StartStation();
        }
        #endregion



        //(x.3) Stop
        public static void Stop()
        {
            Instance.StopStation();
        }


        //(x.4) RunAwait
        public static void RunAwait()
        {
            SersApplication.RunAwait();
        }


        #endregion


        #region (x.1) 成员对象
        
        public readonly LocalApiService localApiService = new LocalApiService();

        private readonly CommunicationManageClient communicationManage = new CommunicationManageClient();

        #endregion



        #region (x.2) InitStation

        public void InitStation()
        {

            Logger.Info("[ServiceStation] init...");

            #region (x.1)CL add Builder for Iocp、ThreadWait
            communicationManage.BeforeBuildOrganize = (configs,  organizeList) => 
            {
                var builderTypeList = new[] {
                    typeof(Sers.CL.Socket.Iocp.OrganizeClientBuilder),
                    typeof(Sers.CL.Socket.ThreadWait.OrganizeClientBuilder)
                };

                foreach (var config in configs)
                {
                    var className = config["className"].ConvertToString();                    

                    var type = builderTypeList.FirstOrDefault(t => t.FullName == className);
                    if (type != null)
                    {
                        var builder = Activator.CreateInstance(type) as IOrganizeClientBuilder;
                        builder.Build(organizeList, config);
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



        #region (x.3) LoadApi

        /// <summary>
        /// 从配置文件(appsettings.json::Sers.LocalApiService.ApiLoaders )  加载api加载器并加载api
        /// 从配置文件(appsettings.json::Sers.LocalApiService.StaticFileMap)加载静态文件映射器
        /// </summary>
        public void LoadApi()
        {
            localApiService.LoadApi_StaticFileMap();
            localApiService.LoadApi();
        }


        /// <summary>
        /// 调用SsApi加载器加载api
        /// </summary> 
        /// <param name="config"></param>
        public void LoadSsApi(ApiLoaderConfig config)
        {
            localApiService.LoadSersApi(config);
        }


        /// <summary>
        /// 调用SsApi加载器加载api
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="config"></param>
        public void LoadSsApi(Assembly assembly, ApiLoaderConfig config = null)
        {
            if (null == config) config = new ApiLoaderConfig();
            config.assembly = assembly;
            LoadSsApi(config);
        }

        #endregion



        #region (x.4) StartStation

        /// <summary>
        /// 是否成功启动站点
        /// </summary>
        /// <returns></returns>
        public bool StartStation()
        {
            Logger.Info("[ServiceStation] starting ...");

            #region (x.1)注册主程序退出回调
            AppDomain.CurrentDomain.ProcessExit += (s, e) =>
            {
                StopStation();
            };
            #endregion

            #region (x.2) CL 注册回调

            communicationManage.conn_OnGetMessage = MessageClient.Instance.OnGetMessage;

            communicationManage.conn_OnGetRequest = (IOrganizeConnection conn,Object sender, ArraySegment<byte> requestData, Action<object, List<ArraySegment<byte>>> callback) => 
            {
                localApiService.CallApiAsync(sender, new ApiMessage(requestData), (object sender1, ApiMessage apiReplyMessage) =>
                {
                     callback(sender1, apiReplyMessage.Package());
                });
            };

            communicationManage.Conn_OnDisconnected = (conn) =>
            {
                StopStation();
            };
            #endregion


            #region (x.3) CL 连接服务器

            Logger.Info("[CL] Connect - trying...");
            
            if (!communicationManage.Start())
            {
                Logger.Info("[CL] Connect - failed");
                return false;
            }

            Logger.Info("[CL] Connect - succeed");
            #endregion



            #region (x.4) 初始化ApiClient
            ApiClient.SetOnSendRequest(communicationManage.organizeList.Select(organize=> organize.conn).Select<IOrganizeConnection, Func<List<ArraySegment<byte>>, ArraySegment<byte>>>(
                conn =>
                {
                    return (req) => { conn.SendRequest(req, out var reply); return reply.ByteDataToArraySegment(); };
                }
                ).ToArray());
            #endregion


            //(x.5) 启动 localApiService服务
            localApiService.Start();

            #region (x.6)向服务中心注册localApiService
            //if (0 < localApiMng.apiCount)
            {
                Logger.Info("[ServiceStation] regist serviceStation to ServiceCenter...");

                var serviceStationData = new
                {                
                    serviceStationInfo = SersApplication.serviceStationInfo,
                    deviceInfo = SersApplication.deviceInfo,
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


                    #region (x.x.2)后台获取机器码，并向服务中心提交（获取机器码比较耗时，故后台获取）
                    Task.Run(()=> {
                        try
                        {
                            //(x.x.x.1)计算  DeviceUnqueKey 和 ServiceStationUnqueKey
                            SersApplication.CalculateUniquekey();

                           //(x.x.x.2)构建api参数
                            var strServiceStationData = new
                            {
                                serviceStationInfo = SersApplication.serviceStationInfo,
                                deviceInfo = SersApplication.deviceInfo                           
                            }.Serialize();

                            //(x.x.x.3)调用api
                            foreach (var apiClient in ApiClient.Instances)
                            {
                                ApiReturn ret = apiClient.CallApi<ApiReturn>("/_sys_/serviceStation/updateStationInfo", strServiceStationData);

                                if (!ret.success)
                                {
                                    Logger.Info("[ServiceStation] updateStationInfo - failed. reply: " + ret.Serialize());                                  
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
                catch (Exception ex)
                {
                    Logger.Error("[ServiceStation] regist - failed", ex);
                    return false;
                }               

                Logger.Info("[ServiceStation] regist - succeed");

            }
            #endregion


            #region (x.7)PubSub 初始化消息订阅 PubSubClient          
            MessageClient.Instance.OnSendMessage = communicationManage.SendMessageAsync;
            #endregion
                            

            //(x.8) 调用SersApp事件
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


            #region CommunicationManage Close
            Logger.Info("[CL] stop...");
            try
            {
                communicationManage.Stop();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            Logger.Info("[CL] stoped");
            #endregion
            

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
