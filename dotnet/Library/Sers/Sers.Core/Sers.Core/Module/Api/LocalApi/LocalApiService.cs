using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Vit.Extensions;
using Vit.Core.Module.Log;
using Sers.Core.Module.Rpc;
using Sers.Core.Module.ApiLoader;
using Vit.Core.Util.ConfigurationManager;
using Vit.Core.Util.Pool;
using Vit.Core.Util.Threading;
using System.Reflection;
using Sers.SersLoader;
using Sers.Core.CL.CommunicationManage;
using Sers.Core.CL.MessageOrganize;
using Sers.Core.Module.Message;
using Vit.Core.Util.ComponentModel.SsError;
using Sers.Core.Module.Api.LocalApi.Event;
using Sers.Core.Util.Consumer;
using System.Runtime.CompilerServices;

namespace Sers.Core.Module.Api.LocalApi
{
    public class LocalApiService
    {        

        /// <summary>
        /// 后台服务的线程个数（单位个，默认0,代表不开启服务）(appsettings.json :: Sers.LocalApiService.workThreadCount)
        /// </summary>
        public int workThreadCount { get; set; }

        public LocalApiService()
        {
            workThreadCount = ConfigurationManager.Instance.GetByPath<int?>("Sers.LocalApiService.workThreadCount") ?? 0;
        }

        public void Init()
        {
            LocalApiEventMng.Instance.UseApiTraceLog();
        }


        public readonly ApiLoaderMng apiLoaderMng = new ApiLoaderMng();




        /// <summary>
        /// 映射  route -> LocalApiNode
        /// </summary>
        public readonly ApiNodeMng apiNodeMng = new ApiNodeMng();

        public IEnumerable<IApiNode> apiNodes => apiNodeMng.apiNodes;




        #region LoadApi


        /// <summary>
        /// 从配置文件(appsettings.json  Sers.LocalApiService.ApiLoaders ) 加载api加载器并加载api
        /// </summary>
        public void LoadApi()
        {
            apiNodeMng.AddApiNode(apiLoaderMng.LoadApi());
        }


        /// <summary>
        /// 调用SersApi加载器加载api
        /// </summary>
        /// <param name="config"></param>
        public void LoadSersApi(ApiLoaderConfig config)
        {
            apiNodeMng.AddApiNode(new SersLoader.ApiLoader().LoadApi(config));
        }

        /// <summary>
        /// 调用SersApi加载器加载api
        /// </summary>
        /// <param name="assembly"></param>
        public void LoadSersApi(Assembly assembly)
        {
            apiNodeMng.AddApiNode(new SersLoader.ApiLoader().LoadApi(new ApiLoaderConfig { assembly = assembly }));
        }

        #endregion




        #region CallLocalApi
        /// <summary>
        /// 构建RpcContext并调用
        /// </summary>
        /// <param name="apiRequest"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ApiMessage CallLocalApi(ApiMessage apiRequest)
        {           
            using (var rpcContext = RpcFactory.CreateRpcContext())
            using (var localApiEvent = LocalApiEventMng.Instance.CreateApiEvent())
            {
                try
                {
                    //(x.1) init rpcContext
                    rpcContext.apiRequestMessage = apiRequest;
                    rpcContext.apiReplyMessage = new ApiMessage();

                    var rpcData = RpcContextData.FromBytes(apiRequest.rpcContextData_OriData);
                    rpcContext.rpcData = rpcData;

                    //(x.2) BeforeCallApi
                    localApiEvent.BeforeCallApi(rpcData, apiRequest);

                    //(x.3)get apiNode and call
                    apiNodeMng.TryGet(rpcData, out var apiNode);
                    if (null == apiNode)
                    {
                        rpcContext.apiReplyMessage.InitAsApiReplyMessageByError(SsError.Err_ApiNotExists);
                    }
                    else
                    {
                        rpcContext.apiReplyMessage.value_OriData = apiNode.Invoke(apiRequest.value_OriData).BytesToArraySegmentByte();
                    }

                }
                catch (Exception ex) when ((ex.GetBaseException() is ThreadInterruptedException))
                {
                    //处理超时
                    rpcContext.apiReplyMessage.rpcContextData_OriData = const_ApiReply_Err_Timeout.rpcContextData_OriData;
                    rpcContext.apiReplyMessage.value_OriData = const_ApiReply_Err_Timeout.value_OriData;
                    throw;
                }
                catch (Exception ex) when (!(ex.GetBaseException() is ThreadInterruptedException))
                {
                    ex = ex.GetBaseException();
                    Logger.Error(ex);
                    SsError error = ex;
                    rpcContext.apiReplyMessage.InitAsApiReplyMessageByError(error);
                }
                return rpcContext.apiReplyMessage;
            }
        }
        #endregion


        static readonly ApiMessage const_ApiReply_Err_Timeout = new ApiMessage().InitAsApiReplyMessageByError(SsError.Err_HandleTimeout);


        #region 后台服务
        class RequestInfo
        {
            public IOrganizeConnection  conn;
            public ApiMessage apiRequest;
            public Object sender;
            public Action<object, ApiMessage> callback;


            public static RequestInfo Pop()
            {
                return ObjectPool<RequestInfo>.Shared.Pop();
            }

            public void Push()
            {
                conn = null;
                apiRequest = null;
                sender = null;
                callback = null;

                ObjectPool<RequestInfo>.Shared.Push(this);
            }

        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CallApiAsync(Object sender, ApiMessage apiRequest, Action<object, ApiMessage> callback)
        {
            var requestInfo = RequestInfo.Pop();

            requestInfo.conn = CommunicationManageServer.CurConn;
            requestInfo.sender = sender;
            requestInfo.apiRequest = apiRequest;
            requestInfo.callback = callback;

            worker.Publish(requestInfo);  
        }



        #region Start Stop

    
        /// <summary>
        /// 后台调用Api的线程 
        /// </summary>
        IWorker worker = null;

        public bool Start()
        {
            Stop();

            int timeout_ms = ConfigurationManager.Instance.GetByPath<int?>("Sers.LocalApiService.timeout_ms") ?? 0;

            if (timeout_ms > 0)
            {
                worker = new Worker_TimeLimit() { timeout_ms = timeout_ms };
              
            }
            else
            {
                worker = new Worker();
            }

          
            return worker.Start(workThreadCount, CallLocalApi);
        }

        public void Stop()
        {
            worker?.Stop();
            worker = null;
        }
        #endregion


       

        #endregion



        #region class Worker
        interface IWorker { bool Start(int workThreadCount, Func<ApiMessage, ApiMessage> callLocalApi);void Stop(); void Publish(RequestInfo requestInfo); }

        #region Worker


        class Worker : IWorker
        {

            IConsumer<RequestInfo> taskToCallApi = new Consumer_BlockingCollection<RequestInfo>();
            //IConsumer<RequestInfo> taskToCallApi = new Consumer_Disruptor<RequestInfo>();
            //IConsumer<RequestInfo> taskToCallApi = new Consumer_WorkerPool<RequestInfo>();
            //IConsumer<RequestInfo> taskToCallApi = new Consumer_WorkerPoolCache<RequestInfo>();
            //IConsumer<RequestInfo> taskToCallApi = new Consumer_WorkerPoolCascade<RequestInfo>();
            //IConsumer<RequestInfo> taskToCallApi = new ConsumerCache<RequestInfo,Consumer_BlockingCollection<RequestInfo>>();


            Func<ApiMessage, ApiMessage> callLocalApi;


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Publish(RequestInfo requestInfo)
            {
                taskToCallApi.Publish(requestInfo);
            }


            public bool Start(int workThreadCount, Func<ApiMessage, ApiMessage> callLocalApi)
            {
                if (taskToCallApi.IsRunning) return false;

                try
                {
                    this.callLocalApi = callLocalApi; 

                    taskToCallApi.name = "LocalApiService";
                    taskToCallApi.workThreadCount = workThreadCount;
                    taskToCallApi.processor = Processor;

                    if (workThreadCount > 0)
                    {
                        taskToCallApi.Start();
                        Logger.Info("[LocalApiService] Started,workThreadCount:" + workThreadCount);

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


            #region TaskToCallApi

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void Processor(RequestInfo requestInfo) 
            {
                ApiMessage apiRequest;
                Object sender;
                Action<object, ApiMessage> callback;

                ApiMessage apiReply;

                try
                {
                    #region 处理 requestInfo
                    CommunicationManageServer.CurConn = requestInfo.conn;
                    apiRequest = requestInfo.apiRequest;
                    sender = requestInfo.sender;
                    callback = requestInfo.callback;

                    requestInfo.Push();
                    #endregion

                    //处理请求
                    apiReply = callLocalApi(apiRequest);

                    //调用请求回调
                    callback(sender, apiReply);
                }
                catch (Exception ex) when (!(ex.GetBaseException() is ThreadInterruptedException))
                {
                    Logger.Error(ex);
                }
            }

 
            #endregion
        }
        #endregion



        #region Worker_TimeLimit
        class Worker_TimeLimit : IWorker
        {

            BlockingCollection<RequestInfo> requestQueue = new BlockingCollection<RequestInfo>();


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Publish(RequestInfo requestInfo)
            {
                requestQueue.Add(requestInfo);
            }


 
            LongTaskHelp_TimeLimit taskToCallApi = new LongTaskHelp_TimeLimit();
            Func<ApiMessage, ApiMessage> callLocalApi;

            /// <summary>
            /// 超时时间，（主动关闭超过此时间的任务,实际任务强制关闭的时间会在1倍超时时间到2倍超时时间内)。单位：ms。
            /// 脉冲间隔。
            /// </summary>
            public int timeout_ms { set { taskToCallApi.timeout_ms = value; } }
            public bool Start(int workThreadCount, Func<ApiMessage, ApiMessage> callLocalApi)
            {
                try
                {
                    this.callLocalApi = callLocalApi;

                    taskToCallApi.threadName = "LocalApiService";
                    taskToCallApi.threadCount = workThreadCount;                   

                    if (workThreadCount > 0)
                    {
                        taskToCallApi.Start(GetWork, DealWork, OnFinish, OnTimeout);
                        Logger.Info("[LocalApiService] Started,workThreadCount:" + workThreadCount+ ",timeout_ms:" + taskToCallApi.timeout_ms);
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


            #region TaskToCallApi

            /*
             workArg        apiRequest
             workArg2       apiReply
             workArg3       sender
             workArg4       callback
                 
                 
                 */


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void GetWork(LongTaskHelp_TimeLimit.Worker w)
            {
                //堵塞获取请求
                var requestInfo = requestQueue.Take();
                CommunicationManageServer.CurConn = requestInfo.conn;

                w.workArg = requestInfo.apiRequest;     
                w.workArg3 = requestInfo.sender;
                w.workArg4 = requestInfo.callback;
                requestInfo.Push();
            }


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void DealWork(LongTaskHelp_TimeLimit.Worker w)
            {
                ApiMessage apiRequest = (ApiMessage)w.workArg;

                //处理请求
                w.workArg2 = callLocalApi(apiRequest);                  
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void OnFinish(LongTaskHelp_TimeLimit.Worker w)
            {
                ApiMessage apiReply = (ApiMessage)w.workArg2;
                object sender = w.workArg3;
                Action<object, ApiMessage> callback = (Action<object, ApiMessage>)w.workArg4;
                //调用请求回调
                callback(sender, apiReply);         
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void OnTimeout(LongTaskHelp_TimeLimit.Worker w)
            {
                ApiMessage apiReply =  new ApiMessage();
                apiReply.rpcContextData_OriData = const_ApiReply_Err_Timeout.rpcContextData_OriData;
                apiReply.value_OriData = const_ApiReply_Err_Timeout.value_OriData;
 
                object sender = w.workArg3;
                Action<object, ApiMessage> callback = (Action<object, ApiMessage>)w.workArg4;

                //调用请求回调
                callback(sender, apiReply);               
            }  
         
            #endregion
        }
        #endregion
        
        #endregion





    }
}
