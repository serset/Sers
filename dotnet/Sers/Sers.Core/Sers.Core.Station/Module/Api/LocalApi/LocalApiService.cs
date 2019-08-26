using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Newtonsoft.Json.Linq;
using Sers.Core.Extensions;
using Sers.Core.Module.Api.Data;
using Sers.Core.Module.Api.Message;
using Sers.Core.Module.Log;
using Sers.Core.Module.Mq.Mq;
using Sers.Core.Module.Mq.MqManager;
using Sers.Core.Module.Rpc;
using Sers.Core.Module.SersDiscovery;
using Sers.Core.Util.Common;
using Sers.Core.Util.ConfigurationManager;
using Sers.Core.Util.Pool;
using Sers.Core.Util.SsError;
using Sers.Core.Util.Threading;

namespace Sers.Core.Module.Api.LocalApi
{
    public class LocalApiService
    {

        public static readonly LocalApiService Instance = new LocalApiService();


        /// <summary>
        /// 后台服务的线程个数（单位个，默认0,代表不开启服务）(appsettings.json :: Sers.LocalApiService.workThreadCount)
        /// </summary>
        public int workThreadCount = 0;

        public LocalApiService()
        {
            workThreadCount = ConfigurationManager.Instance.GetByPath<int?>("Sers.LocalApiService.workThreadCount") ?? 0;
        }

        public void Init()
        {
            this.UseSsApiDiscovery();

            this.UseSubscriberDiscovery();

            this.UseApiTraceLog();


            #region 构建 Api Event BeforeCallApi
            var BeforeCallApi = Sers.Core.Station.Module.Api.ApiEvent.BeforeCallApi.EventBuilder.LoadEvent(ConfigurationManager.Instance.GetByPath<JArray>("Sers.LocalApiService.BeforeCallApi"));
            if (BeforeCallApi != null) this.BeforeCallApi += BeforeCallApi;
            #endregion

        }


        public readonly SersDiscoveryMng discoveryMng = new SersDiscoveryMng();


        /// <summary>
        /// 映射  route -> LocalApiNode
        /// </summary>
        public readonly SortedDictionary<string, IApiNode> apiMap = new SortedDictionary<string, IApiNode>();

        public IEnumerable<IApiNode> apiNodes => apiMap.Select((kv) => kv.Value);

        /// <summary>
        /// api的个数
        /// </summary>
        public int apiCount =>  apiMap.Count;


        #region Discovery

        /// <summary>
        /// 发现服务,可多次调用
        /// </summary>
        /// <param name="config"></param>
        public void Discovery(DiscoveryConfig config)
        {

            //(x.1) load from dll file
            if (config.assembly == null && !String.IsNullOrEmpty(config.assemblyFile))
            {
                try
                {
                    config.assembly = Assembly.LoadFile(CommonHelp.GetAbsPathByRealativePath(config.assemblyFile));
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }

            //(x.2) load by assemblyName
            if (config.assembly == null && !String.IsNullOrEmpty(config.assemblyName))
            {
                try
                {
                    config.assembly = Assembly.Load(config.assemblyName);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }


            if (config.assembly == null) return;

            Logger.Info("[LocalApiService] Discovery,assembly:[" + config.assembly?.FullName + "]");
            discoveryMng.Discovery(config);
        }

        /// <summary>
        /// 发现服务,可多次调用
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="config"></param>
        public void Discovery(Assembly assembly, DiscoveryConfig config = null)
        {
            if (null == config) config = new DiscoveryConfig();
            config.assembly = assembly;
            Discovery(config);
        }


        /// <summary>
        /// 从配置文件(appsettings.json  Sers.ApiStation.DiscoveryConfig )获取服务发现配置并查找服务
        /// </summary>
        public void Discovery()
        {
            ConfigurationManager.Instance.GetByPath<List<DiscoveryConfig>>("Sers.LocalApiService.SersApiDiscovery.DiscoveryConfig")?.ForEach(Discovery);
        }




        #endregion


        /// <summary>
        /// BeforeCallApi(IRpcContextData rpcData, ApiMessage requestMessage)
        /// </summary>
        public Action<IRpcContextData, ApiMessage> BeforeCallApi=null;


        #region CallLocalApi
        /// <summary>
        /// 构建RpcContext并调用
        /// </summary>
        /// <param name="apiRequest"></param>
        /// <returns></returns>
        private ApiMessage CallLocalApi(ApiMessage apiRequest)
        {
            using (var rpcContext = RpcFactory.Instance.CreateRpcContext())
            {
                try
                {
                    //(x.1) init rpcContext
                    rpcContext.apiRequestMessage = apiRequest;
                    rpcContext.apiReplyMessage = new ApiMessage();

                    var rpcData = RpcFactory.Instance.CreateRpcContextData();
                    rpcData.UnpackOriData(apiRequest.rpcContextData_OriData);
                    rpcContext.rpcData=rpcData;


                    //(x.2) BeforeCallApi
                    BeforeCallApi?.Invoke(rpcData, apiRequest);

                    //(x.3)get apiNode and call
                    apiMap.TryGetValue(rpcData.route, out var apiNode);
                    if (null == apiNode)
                    {
                        ApiReturn apiRet = new SsError { errorMessage = "api not found! route:" + rpcData.route, errorCode = 100 };
                        rpcContext.apiReplyMessage.value_OriData = apiRet.SerializeToArraySegmentByte();
                    }
                    else
                    {
                        rpcContext.apiReplyMessage.value_OriData = apiNode.Invoke(apiRequest.value_OriData).BytesToArraySegmentByte();
                    }

                }
                catch (Exception ex)
                {
                    ex = ex.GetBaseException();
                    Logger.Error(ex);
                    ApiReturn apiRet = ex;
                    rpcContext.apiReplyMessage.value_OriData = apiRet.SerializeToArraySegmentByte();
                }
                return rpcContext.apiReplyMessage;
            }
        }
        #endregion





        #region 后台服务
        class RequestInfo
        {
            public IMqConn mqConn;
            public ApiMessage apiRequest;
            public Object sender;
            public Action<object, ApiMessage> callback;


            public static RequestInfo Pop()
            {
                return ObjectPool<RequestInfo>.Shared.Pop();
            }

            public void Push()
            {
                mqConn = null;
                apiRequest = null;
                sender = null;
                callback = null;

                ObjectPool<RequestInfo>.Shared.Push(this);
            }

        }


        public void CallApiAsync(Object sender, ApiMessage apiRequest, Action<object, ApiMessage> callback)
        {
            var requestInfo = RequestInfo.Pop();

            requestInfo.mqConn = ServerMqManager.CurMqConn;
            requestInfo.sender = sender;
            requestInfo.apiRequest = apiRequest;
            requestInfo.callback = callback;

            requestQueue.Add(requestInfo);
        }



        #region Start Stop

      

        public bool Start()
        {
            try
            {
                taskToCallApi.threadName = "LocalApiService";

                taskToCallApi.threadCount = workThreadCount;
                taskToCallApi.action = TaskToCallApi;

                if (workThreadCount > 0)
                {
                    taskToCallApi.Start();
                    Logger.Info("[LocalApiService] Started,workThreadCount:"+ workThreadCount);

                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Stop();
                return false;
            }
        }

        public void Stop()
        {
            if (!taskToCallApi.IsRunning) return;

            try
            { 
                taskToCallApi.Stop();          
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            
            Logger.Info("[LocalApiService] Stoped");
        }
        #endregion


        #region  后台调用Api线程 taskToCallApi

        LongTaskHelp taskToCallApi = new LongTaskHelp();
        BlockingCollection<RequestInfo> requestQueue = new BlockingCollection<RequestInfo>();
 
        void TaskToCallApi()
        {
            RequestInfo requestInfo;
           
            ApiMessage apiRequest;
            Object sender;
            Action<object, ApiMessage> callback;

            ApiMessage apiReply;
            while (true)
            {
                try
                {
                    #region ThreadToDealMsg                        
                    while (true)
                    {
                        //堵塞获取请求
                        requestInfo = requestQueue.Take();

                        #region 处理 requestInfo
                        ServerMqManager.CurMqConn = requestInfo.mqConn;
                        apiRequest = requestInfo.apiRequest;
                        sender = requestInfo.sender;
                        callback = requestInfo.callback;

                        requestInfo.Push();
                        #endregion

                        //处理请求
                        apiReply = CallLocalApi(apiRequest);

                        //调用请求回调
                        callback(sender, apiReply);
                    }
                    #endregion
                }
                catch (Exception ex) when (!(ex is ThreadInterruptedException))
                {
                    Logger.Error(ex);
                }
            }
        }
        #endregion

        #endregion






    }
}
