using Sers.Core.Module.App;
using Vit.Core.Module.Log;
using System;
using System.Reflection;
using Vit.Extensions;
using Sers.Core.Module.PubSub;
using System.Collections.Generic;
using Sers.Core.Module.Api.LocalApi;
using Sers.Core.Module.Api;
using Sers.Core.Module.ApiLoader;
using Sers.ServiceCenter.ApiCenter;
using Newtonsoft.Json.Linq;
using Sers.Core.Module.Env;
using Sers.Core.Module.Message;
using Sers.ServiceCenter.Entity;
using System.Linq;
using Sers.Core.CL.CommunicationManage;
using Sers.Core.CL.MessageOrganize;
using System.Threading.Tasks;

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
        }

        #region (x.1) 成员对象

        public   ApiCenterService apiCenterService { get; set; }  


        private readonly CommunicationManageServer communicationManage = new CommunicationManageServer();

        private readonly LocalApiService localApiService = new LocalApiService();

        private readonly IOrganizeConnection connForLocalStationService;

        #region class OrganizeConnection For LocalStationService        

        class OrganizeConnection : IOrganizeConnection
        {
            public string connTag { get; set; }

            LocalApiService localApiService;
            public OrganizeConnection(LocalApiService localApiService)
            {
                this.localApiService = localApiService;
            }


            public void SendRequestAsync(Object sender, List<ArraySegment<byte>> requestData, Action<object, List<ArraySegment<byte>>> callback)
            {
                localApiService.CallApiAsync(sender, new ApiMessage(requestData.ByteDataToArraySegment()), (sender_, apiReplyMessage) =>
                {
                    callback(sender_, apiReplyMessage.Package());
                });
            }
            public bool SendRequest(List<ArraySegment<byte>> requestData, out List<ArraySegment<byte>> replyData)
            {
                Logger.Error(new NotImplementedException());
                throw new NotImplementedException();
            }

       

            public void SendMessageAsync(List<ArraySegment<byte>> message)
            {
                MessageClient.Instance.OnGetMessage(this, message.ByteDataToArraySegment());
            }
        }


        #endregion

        #endregion



        #region (x.2) InitCenter

        public void InitCenter()
        {
            Logger.Info("初始化ServiceCenter...");

            #region (x.1)CL add Builder for Iocp、ThreadWait            
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
            localApiService.LoadApi_StaticFileMap();
            localApiService.LoadApi();
        }


        /// <summary>
        /// 调用SsApi加载器加载api
        /// </summary> 
        /// <param name="config"></param>
        public void LoadSsApi(ApiLoaderConfig config)
        {
            localApiService.LoadSsApi(config);
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
                  Logger.Info("[CL] OnConnected,connTag:" + conn.connTag);
              };

            communicationManage.Conn_OnDisconnected = (IOrganizeConnection conn) =>
            {
                Logger.Info("[CL] OnDisconnected,connTag:" + conn.connTag);

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
            Func<List<ArraySegment<byte>>, ArraySegment<byte>> OnSendRequest = ((List<ArraySegment<byte>> apiReqMessage) =>
            {
                apiCenterService.CallApi(connForLocalStationService, apiReqMessage.ByteDataToArraySegment(),
                    out var replyData, communicationManage.requestTimeoutMs);
                return replyData.ByteDataToArraySegment();
            });

            ApiClient.SetOnSendRequest(new[] { OnSendRequest });
            #endregion


            #region (x.7) 桥接MessageClient 和 MessageCenterService
            MessageClient.Instance.OnSendMessage = (List<ArraySegment<byte>> messageData) =>
            {
                MessageCenterService.Instance.OnGetMessage(connForLocalStationService, messageData.ByteDataToBytes().BytesToArraySegmentByte());
            };
            #endregion


            //(x.8) 调用SersApp事件
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


            Logger.Info("[ServiceCenter] stoped");

            //调用SersApp 事件
            SersApplication.OnStop();
        }
        #endregion


    }
}
