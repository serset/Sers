using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Sers.Core.CL.CommunicationManage;
using Sers.Core.CL.MessageOrganize;
using Sers.Core.Module.Api;
using Sers.Core.Module.Api.LocalApi;
using Sers.Core.Module.App;
using Sers.Core.Module.App.AppEvent;
using Sers.Core.Module.Env;
using Sers.Core.Module.Message;
using Sers.Core.Module.PubSub;
using Sers.SersLoader;

using Vit.Core.Module.Log;
using Vit.Core.Util.ComponentModel.Data;
using Vit.Core.Util.ConfigurationManager;
using Vit.Extensions;
using Vit.Extensions.Newtonsoft_Extensions;
using Vit.Extensions.Serialize_Extensions;

namespace Sers.ServiceStation
{

    [JsonObject(MemberSerialization.OptIn)]
    public class ServiceStation
    {

        #region static

        public static readonly ServiceStation Instance = new ServiceStation();

        /// <summary>
        /// Start the station using the default steps
        /// </summary>
        public static void AutoRun()
        {
            // Manually specify the Station version number
            //Sers.Core.Module.App.SersApplication.serviceStationInfo.stationVersion = "2.0.1";


            // #1 Init
            ServiceStation.Init();

            // #2 Discovery
            //ServiceStation.Instance.LoadSersApi(typeof(Program).Assembly);
            //ServiceStation.Instance.LoadSersApi(typeof(Program).Assembly, new Sers.Core.Module.ApiLoader.ApiLoaderConfig { apiStationName = "Demo" });
            ServiceStation.Instance.LoadApi();

            // #3 Start
            ServiceStation.Start();


            // #4 RunAwait
            ServiceStation.RunAwait();

        }



        #region #1 Init

        /// <summary>
        /// init ServiceStation
        /// </summary>
        public static void Init()
        {
            Instance.InitStation();
        }
        #endregion




        #region #2 Start
        /// <summary>
        ///
        /// </summary>
        /// <returns>Whether the station started successfully</returns>
        public static bool Start()
        {
            return Instance.StartStation();
        }
        #endregion



        // #3 Stop
        public static void Stop()
        {
            Instance.StopStation();
        }


        // #4 RunAwait
        public static void RunAwait()
        {
            SersApplication.RunAwait();
        }


        #endregion

        public ServiceStation()
        {
            appEventList = AppEventLoader.LoadAppEvent(Vit.Core.Util.ConfigurationManager.Appsettings.json.GetByPath<JArray>("Sers.AppEvent"))
                ?.ToList();
        }


        #region #1 Members

        List<IAppEvent> appEventList { get; set; }

        public readonly ILocalApiService localApiService = LocalApiServiceFactory.CreateLocalApiService();

        private readonly CommunicationManageClient communicationManage = new CommunicationManageClient();

        #endregion



        #region #2 InitStation

        public void InitStation()
        {
            Logger.Info("[ServiceStation] initializing", new { AssemblyVersion = SersEnvironment.GetEntryAssemblyVersion() });

            // ##1 appEvent BeforeStart
            appEventList?.ForEach(ev => ev.BeforeStart());


            #region ##2 CL add builder for Iocp/ThreadWait
            communicationManage.BeforeBuildOrganize = (configs, organizeList) =>
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

            // ##3 localApiService
            localApiService.Init();

            // ##4 UsageReporter
            UsageReporter.UseUsageReporter();

        }
        #endregion



        #region #3 LoadApi

        /// <summary>
        /// Load ApiLoaders and load the APIs from the configuration file (appsettings.json::Sers.LocalApiService.ApiLoaders)
        /// Load API for static files from the configuration file (appsettings.json::Sers.LocalApiService.StaticFileMap)
        /// </summary>
        public void LoadApi()
        {
            localApiService.LoadApi_StaticFiles();
            localApiService.LoadApi();
        }


        /// <summary>
        /// Invoke the SsApi loader to load the APIs
        /// </summary> 
        /// <param name="config"></param>
        public void LoadSersApi(ApiLoaderConfig config)
        {
            localApiService.LoadSersApi(config);
        }


        /// <summary>
        /// Invoke the SsApi loader to load the APIs
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="config"></param>
        public void LoadSersApi(Assembly assembly, ApiLoaderConfig config = null)
        {
            if (null == config) config = new ApiLoaderConfig();
            config.assembly = assembly;
            LoadSersApi(config);
        }

        #endregion



        #region #4 StartStation

        /// <summary>
        ///
        /// </summary>
        /// <returns>Whether the station started successfully</returns>
        public bool StartStation()
        {
            Logger.Info("[ServiceStation] starting ...");

            // #0 appEvent OnStart
            appEventList?.ForEach(ev => ev.OnStart());

            #region #1 Register the main program exit callback
            AppDomain.CurrentDomain.ProcessExit += (s, e) =>
            {
                StopStation();
            };
            #endregion

            #region #2 CL Register callback

            communicationManage.conn_OnGetMessage = MessageClient.Instance.OnGetMessage;

            communicationManage.conn_OnGetRequest = (IOrganizeConnection conn, Object sender, ArraySegment<byte> requestData, Action<object, Vit.Core.Util.Pipelines.ByteData> callback) =>
            {
                localApiService.InvokeApiAsync(sender, new ApiMessage(requestData), (object sender1, ApiMessage apiReplyMessage) =>
                {
                    callback(sender1, apiReplyMessage.Package());
                });
            };

            communicationManage.Conn_OnDisconnected = (conn) =>
            {
                StopStation();
            };
            #endregion


            #region #3 CL connect to server

            Logger.Info("[CL] Connect - trying...");

            if (!communicationManage.Start())
            {
                Logger.Error("[CL] Connect - failed");
                return false;
            }

            Logger.Info("[CL] Connect - succeed");
            #endregion



            #region #4  init ApiClient
            ApiClient.SetOnSendRequest(
                communicationManage.organizeList.Select(organize => organize.conn)
                .Select<IOrganizeConnection, Action<ApiMessage, Action<ArraySegment<byte>>>>(
                conn =>
                {
                    return (apiRequestMessage, callback) =>
                    {
                        conn.SendRequestAsync(null, apiRequestMessage.Package(), (sender, replyData) =>
                        {
                            callback(replyData.ToArraySegment());
                        });
                    };
                }
                ).ToArray(), communicationManage.requestTimeoutMs);
            #endregion


            // #5 start localApiService
            localApiService.Start();

            #region #6 register localApiService to ServiceCenter
            //if (0 < localApiMng.apiCount)
            {
                Logger.Info("[ServiceStation] register serviceStation to ServiceCenter...");

                var serviceStationData = new
                {
                    serviceStationInfo = SersApplication.serviceStationInfo,
                    deviceInfo = SersApplication.deviceInfo,
                    localApiService.apiNodes
                }.Serialize();

                if (true == Appsettings.json.GetByPath<bool?>("Sers.ServiceStation.StationRegist_PrintRegistArg"))
                {
                    Logger.Info("[ServiceStation] register - arg:" + serviceStationData);
                }



                try
                {
                    foreach (var apiClient in ApiClient.Instances)
                    {
                        ApiReturn ret = apiClient.CallApi<ApiReturn>("/_sys_/serviceStation/regist", serviceStationData);

                        if (ret?.success != true)
                        {
                            Logger.Error("[ServiceStation] register - failed", ret);
                            return false;
                        }
                    }


                    #region  Retrieve the machine code in the background and push it to the service center (since retrieving the machine code is time-consuming, so it's done in the background).
                    Task.Run(() =>
                    {
                        try
                        {
                            // ##1 calculate DeviceUniqueKey and ServiceStationUniqueKey
                            SersApplication.CalculateUniquekey();

                            // ##2 get arguments
                            var strServiceStationData = new
                            {
                                serviceStationInfo = SersApplication.serviceStationInfo,
                                deviceInfo = SersApplication.deviceInfo
                            };

                            // ##3 call api to update station info
                            foreach (var apiClient in ApiClient.Instances)
                            {
                                ApiReturn ret = apiClient.CallApi<ApiReturn>(route: "/_sys_/serviceStation/updateStationInfo", arg: strServiceStationData);

                                if (ret?.success != true)
                                {
                                    Logger.Error("[ServiceStation] updateStationInfo - failed", ret);
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
                    Logger.Error("[ServiceStation] register - failed", ex);
                    return false;
                }

                Logger.Info("[ServiceStation] register - succeed");

            }
            #endregion


            #region #7 PubSub init MessageSubscriber PubSubClient
            MessageClient.Instance.OnSendMessage = communicationManage.SendMessageAsync;
            #endregion


            // #8 invoke SersApp OnStart event
            SersApplication.ResistConsoleCancelKey(Stop);
            SersApplication.OnStart();

            Logger.Info("[ServiceStation] started", SersApplication.serviceStationInfo.serviceStationName);

            // #9 appEvent AfterStart
            appEventList?.ForEach(ev => ev.AfterStart());

            return true;
        }
        #endregion



        #region #5 StopStation


        public void StopStation()
        {
            Logger.Info("[ServiceStation] stoping...");

            // ##1 appEvent BeforeStop
            appEventList?.ForEach(ev => ev.BeforeStop());

            // ##2 stop service
            if (SersApplication.IsRunning)
            {
                #region CommunicationManage Stop
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

            Logger.Info("[ServiceStation] stoped");

            // ##3 appEvent AfterStop
            appEventList?.ForEach(ev => ev.AfterStop());

            // ##4 Invoke SersApp Stop events
            SersApplication.OnStop();
        }
        #endregion






    }
}
