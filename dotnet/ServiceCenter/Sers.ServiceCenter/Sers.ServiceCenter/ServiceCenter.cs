using Sers.Core.Module.App;
using Vit.Core.Module.Log;
using System;
using System.Reflection;
using Vit.Extensions;
using Sers.Core.Module.PubSub;
using System.Collections.Generic;
using Sers.Core.Module.Api.LocalApi;
using Sers.Core.Module.Api;
using Sers.ServiceCenter.ApiCenter;
using Newtonsoft.Json.Linq;
using Sers.Core.Module.Env;
using Sers.Core.Module.Message;
using Sers.ServiceCenter.Entity;
using System.Linq;
using Sers.Core.CL.CommunicationManage;
using Sers.Core.CL.MessageOrganize;
using System.Threading.Tasks;
using Sers.SersLoader;
using Sers.Core.Module.App.AppEvent;
using System.Runtime.CompilerServices;

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

        public ServiceCenter()
        {
            connForLocalStationService = new OrganizeConnection(localApiService);

            appEventList = AppEventLoader.LoadAppEvent(Vit.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetByPath<JArray>("Sers.AppEvent"))
                ?.ToList();
        }

        #region (x.1) 成员对象

        List<IAppEvent> appEventList { get; set; }

        public   ApiCenterService apiCenterService { get; set; }  


        private readonly CommunicationManageServer communicationManage = new CommunicationManageServer();

        private readonly ILocalApiService localApiService = LocalApiServiceFactory.CreateLocalApiService();

        private readonly IOrganizeConnection connForLocalStationService;

        #region class OrganizeConnection For LocalStationService        

        class OrganizeConnection : IOrganizeConnection
        {
            public string connTag { get; set; }

            ILocalApiService localApiService;
            public OrganizeConnection(ILocalApiService localApiService)
            {
                this.localApiService = localApiService;
            }


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SendRequestAsync(Object sender, Vit.Core.Util.Pipelines.ByteData requestData, Action<object, Vit.Core.Util.Pipelines.ByteData> callback)
            {
                localApiService.InvokeApiAsync(sender, new ApiMessage(requestData.ToArraySegment()), (sender_, apiReplyMessage) =>
                {
                    callback(sender_,apiReplyMessage.Package());
                });
            }            


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SendMessageAsync(Vit.Core.Util.Pipelines.ByteData message)
            {
                MessageClient.Instance.OnGetMessage(this, message.ToArraySegment());
            }


            public void Close() 
            {
                ServiceCenter.Instance.StopCenter();
            }

        }


        #endregion

        #endregion



        #region (x.2) InitCenter

        public void InitCenter()
        {
            Logger.Info("初始化ServiceCenter...");

            //(x.0) appEvent BeforeStart
            appEventList?.ForEach(ev=>ev.BeforeStart());


            #region (x.1)CL add builder for Iocp、ThreadWait            
            communicationManage.BeforeBuildOrganize = (JObject[] configs, List<IOrganizeServer> organizeList) =>
            {
                var builderTypeList = new[] {
                    typeof(Sers.CL.Socket.Iocp.OrganizeServerBuilder),
                    typeof(Sers.CL.Socket.ThreadWait.OrganizeServerBuilder)
                };
                foreach (var config in configs)
                {
                    var className = config["className"].ConvertToString();

                    var type = builderTypeList.FirstOrDefault(t => t.FullName == className);
                    if (type == null) continue;

                    var builder = Activator.CreateInstance(type) as IOrganizeServerBuilder;
                    if (builder == null) continue;

                    builder.Build(organizeList, config);
                    config["className"] = null;
                }
            };
            #endregion

            //(x.2)localApiService
            localApiService.Init();

            //(x.3)UsageReporter
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
            localApiService.LoadApi_StaticFiles();
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


        #region (x.4) StartCenter 
        public bool StartCenter()
        {
            Logger.Info("[ServiceCenter] starting ...");

            //(x.0) appEvent OnStart
            appEventList?.ForEach(ev => ev.OnStart());


            #region (x.1)注册主程序退出回调
            AppDomain.CurrentDomain.ProcessExit += (s, e) =>
            {
                StopCenter();
            };
            #endregion


            #region (x.2)CL 注册回调

            // 注册消息订阅 MessageCenterService
            communicationManage.conn_OnGetMessage = MessageCenterService.Instance.OnGetMessage;

            communicationManage.conn_OnGetRequest = apiCenterService.CallApiAsync;

            communicationManage.Conn_OnConnected = (IOrganizeConnection conn) =>
              {
                  Logger.Info("[CL] OnConnected", new { connTag = conn.connTag });
              };

            communicationManage.Conn_OnDisconnected = (IOrganizeConnection conn) =>
            {
                Logger.Info("[CL] OnDisconnected", new { connTag = conn.connTag });

                MessageCenterService.Instance.Conn_OnDisconnected(conn);

                apiCenterService.ServiceStation_Remove(conn);
            };         

            #endregion


            #region (x.3)注册 localApiService 到 apiCenterService
            {
                Logger.Info("[ServiceCenter] regist localApiService to apiCenterService...");


                var serviceStationData = new
                {
                    serviceStationInfo = SersApplication.serviceStationInfo,
                    deviceInfo = SersApplication.deviceInfo,
                    localApiService.apiNodes
                };

                var serviceStation = serviceStationData.ConvertBySerialize<ServiceStation>();

                serviceStation.connection = connForLocalStationService;

                apiCenterService.ServiceStation_Regist(serviceStation);


                #region (x.x.2)后台获取机器码，并向服务中心提交（获取机器码比较耗时，故后台获取）
                Task.Run(() => {
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

                        serviceStation = strServiceStationData.ConvertBySerialize<ServiceStation>();
                        serviceStation.connection = connForLocalStationService;

                        //(x.x.x.3)调用api
                        apiCenterService.ServiceStation_UpdateStationInfo(serviceStation);                        
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }

                });
                #endregion


            }
            #endregion


            #region (x.4)CL Start 
            Logger.Info("[CL] starting...");

            if (!communicationManage.Start())
            {
                Logger.Info("[CL] start - failed");
                return false;
            }
            Logger.Info("[CL] started");
            #endregion

            //(x.5)
            localApiService.Start();


            #region (x.6) 初始化ApiClient
            Action<ApiMessage, Action<ArraySegment<byte>>> OnSendRequest = ((apiRequestMessage,callback) =>
            {
                apiCenterService.CallApiAsync(connForLocalStationService, null, apiRequestMessage,
                    (sender, replyData) =>
                    {
                        callback(replyData.ToArraySegment());
                    }
                 );                
            });

            ApiClient.SetOnSendRequest(new[] { OnSendRequest },communicationManage.requestTimeoutMs);
            #endregion


            #region (x.7) 桥接MessageClient 和 MessageCenterService
            MessageClient.Instance.OnSendMessage = (Vit.Core.Util.Pipelines.ByteData messageData) =>
            {
                MessageCenterService.Instance.OnGetMessage(connForLocalStationService, messageData.ToBytes().BytesToArraySegmentByte());
            };
            #endregion


            //(x.8) 调用SersApp事件
            SersApplication.ResistConsoleCancelKey(Stop);
            SersApplication.OnStart();


            Logger.Info("[ServiceCenter] started");

            //(x.9) appEvent AfterStart
            appEventList?.ForEach(ev => ev.AfterStart());

            return true;
        }
        #endregion



        #region (x.5) StopCenter
        public void StopCenter()
        { 
            Logger.Info("[ServiceCenter] stop...");

            //(x.1) appEvent BeforeStop
            appEventList?.ForEach(ev => ev.BeforeStop());

            //(x.2)stop service
            if (SersApplication.IsRunning)
            {
                #region CommunicationManage stop
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

                Logger.Info("[LocalApiService] stop...");
                try
                {
                    localApiService.Stop();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
                Logger.Info("[LocalApiService] stoped");
            }

            Logger.Info("[ServiceCenter] stoped");

            //(x.3) appEvent AfterStop
            appEventList?.ForEach(ev => ev.AfterStop());

            //(x.4)调用SersApp 事件
            SersApplication.OnStop();

           
        }
        #endregion


    }
}
