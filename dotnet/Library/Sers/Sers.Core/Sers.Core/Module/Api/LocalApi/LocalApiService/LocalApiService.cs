using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

using Newtonsoft.Json.Linq;

using Sers.Core.CL.CommunicationManage;
using Sers.Core.CL.MessageOrganize;
using Sers.Core.Module.Api.LocalApi.Event;
using Sers.Core.Module.Message;
using Sers.Core.Module.Rpc;
using Sers.Core.Util.Consumer;

using Vit.Core.Module.Log;
using Vit.Core.Util.ComponentModel.SsError;
using Vit.Core.Util.ConfigurationManager;
using Vit.Core.Util.Threading.Worker;
using Vit.Extensions;
using Vit.Extensions.Json_Extensions;

namespace Sers.Core.Module.Api.LocalApi
{
    public class LocalApiService : ILocalApiService
    {

        /// <summary>
        /// 后台服务的线程个数（单位个，0代表不开启服务）(appsettings.json :: Sers.LocalApiService.workThread.threadCount)
        /// </summary>
        public int threadCount { get => workThread.threadCount; set => workThread.threadCount = value; }

        public LocalApiService()
        {
            workThread = ConsumerFactory.CreateConsumer<RequestInfo>(Appsettings.json.GetByPath<JObject>("Sers.LocalApiService.workThread"));

            workThread.threadName = "LocalApiService";
            workThread.Processor = Consumer_Processor;
            workThread.OnFinish = Consumer_OnFinish;
        }

        public void Init()
        {
            var localApiServiceConfig = Appsettings.json.GetByPath<JToken>("Sers.LocalApiService");
            if (localApiServiceConfig != null)
                localApiEventMng.Init(localApiServiceConfig);
        }

        public LocalApiEventMng localApiEventMng { get; } = new LocalApiEventMng();

        /// <summary>
        /// 映射  route -> LocalApiNode
        /// </summary>
        protected readonly ApiNodeMng apiNodeMng = new ApiNodeMng();

        public IEnumerable<IApiNode> apiNodes => apiNodeMng.apiNodes;


        public ApiNodeMng ApiNodeMng => apiNodeMng;


        static readonly ApiMessage const_ApiReply_Err_Timeout = new ApiMessage().InitAsApiReplyMessageByError(SsError.Err_HandleTimeout);
        static readonly ApiMessage const_ApiReply_Err_Overload = new ApiMessage().InitAsApiReplyMessageByError(SsError.Err_RateLimit_Refuse);
        static readonly ApiMessage const_ApiReply_Err = new ApiMessage().InitAsApiReplyMessageByError(SsError.Err_SysErr);



        #region CallLocalApi
        /// <summary>
        /// 构建RpcContext并调用
        /// </summary>
        /// <param name="apiRequest"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ApiMessage CallLocalApi(ApiMessage apiRequest)
        {
            using (var rpcContext = new RpcContext())
            using (var localApiEvent = localApiEventMng.CreateApiEvent())
            {
                try
                {
                    //(x.1) init rpcContext
                    rpcContext.apiRequestMessage = apiRequest;
                    rpcContext.apiReplyMessage = new ApiMessage();

                    var rpcData = RpcContextData.FromBytes(apiRequest.rpcContextData_OriData);
                    rpcContext.rpcData = rpcData;

                    //(x.2) BeforeCallApi
                    localApiEvent?.BeforeCallApi(rpcData, apiRequest);

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



        #region Start Stop
        public bool Start()
        {
            Stop();
            return Consumer_Start();
        }

        public void Stop()
        {
            Consumer_Stop();
        }
        #endregion




        #region 后台服务
        class RequestInfo
        {
            public IOrganizeConnection conn;
            public ApiMessage apiRequest;
            public ApiMessage apiReply;
            public Object sender;
            public Action<object, ApiMessage> callback;
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InvokeApiAsync(Object sender, ApiMessage apiRequest, Action<object, ApiMessage> callback)
        {
            var requestInfo = new RequestInfo();

            requestInfo.conn = CommunicationManageServer.CurConn;
            requestInfo.sender = sender;
            requestInfo.apiRequest = apiRequest;
            requestInfo.callback = callback;

            Consumer_Publish(requestInfo);
        }
        #endregion



        #region Consumer


        readonly IConsumer<RequestInfo> workThread;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Consumer_Publish(RequestInfo requestInfo)
        {
            workThread.Publish(requestInfo);
        }


        bool Consumer_Start()
        {
            if (workThread.isRunning) return false;
            try
            {
                if (threadCount > 0)
                {
                    workThread.Start();
                    Logger.Info("[LocalApiService] Started", new { threadCount });

                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Consumer_Stop();
                return false;
            }
        }

        void Consumer_Stop()
        {
            if (!workThread.isRunning) return;

            try
            {
                workThread.Stop();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            Logger.Info("[LocalApiService] Stoped");
        }


        #region event

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Consumer_Processor(RequestInfo requestInfo)
        {
            try
            {
                CommunicationManageServer.CurConn = requestInfo.conn;

                //处理请求
                requestInfo.apiReply = CallLocalApi(requestInfo.apiRequest);
            }
            catch (Exception ex) when (!(ex.GetBaseException() is ThreadInterruptedException))
            {
                Logger.Error(ex);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Consumer_OnFinish(ETaskFinishStatus status, RequestInfo requestInfo)
        {
            ApiMessage apiReply;
            switch (status)
            {
                case ETaskFinishStatus.success:
                    apiReply = requestInfo.apiReply;
                    break;

                case ETaskFinishStatus.timeout:
                    apiReply = new ApiMessage();
                    apiReply.rpcContextData_OriData = const_ApiReply_Err_Timeout.rpcContextData_OriData;
                    apiReply.value_OriData = const_ApiReply_Err_Timeout.value_OriData;
                    break;

                case ETaskFinishStatus.overload:
                    apiReply = new ApiMessage();
                    apiReply.rpcContextData_OriData = const_ApiReply_Err_Overload.rpcContextData_OriData;
                    apiReply.value_OriData = const_ApiReply_Err_Overload.value_OriData;
                    break;

                default:
                    apiReply = new ApiMessage();
                    apiReply.rpcContextData_OriData = const_ApiReply_Err.rpcContextData_OriData;
                    apiReply.value_OriData = const_ApiReply_Err.value_OriData;
                    break;
            }

            //调用请求回调
            requestInfo.callback(requestInfo.sender, apiReply);
        }



        #endregion

        #endregion









    }
}
