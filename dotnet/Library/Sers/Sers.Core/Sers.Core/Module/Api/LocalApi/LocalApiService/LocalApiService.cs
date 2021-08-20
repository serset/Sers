using System;
using System.Collections.Generic;
using System.Threading;
using Vit.Extensions;
using Vit.Core.Module.Log;
using Sers.Core.Module.Rpc;
using Vit.Core.Util.ConfigurationManager;
using Sers.Core.CL.CommunicationManage;
using Sers.Core.CL.MessageOrganize;
using Sers.Core.Module.Message;
using Vit.Core.Util.ComponentModel.SsError;
using Sers.Core.Module.Api.LocalApi.Event;
using Sers.Core.Util.Consumer;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;

namespace Sers.Core.Module.Api.LocalApi
{
    public class LocalApiService: ILocalApiService
    {

        /// <summary>
        /// 后台服务的线程个数（单位个，默认0,代表不开启服务）(appsettings.json :: Sers.LocalApiService.workThreadCount)
        /// </summary>
        public int workThreadCount { get => workThread.workThreadCount; set => workThread.workThreadCount = value; }

        public LocalApiService()
        {
            workThread = ConsumerFactory.CreateConsumer<RequestInfo>(ConfigurationManager.Instance.GetByPath<JObject>("Sers.LocalApiService.workThread"));

            workThread.name = "LocalApiService";
            workThread.processor = Consumer_Processor;
            workThread.OnFinish = Consumer_OnFinish;
            workThread.OnTimeout = Consumer_OnTimeout;
        }

        public void Init()
        {
            LocalApiEventMng.Instance.UseApiTraceLog();
        }

        /// <summary>
        /// 映射  route -> LocalApiNode
        /// </summary>
        protected readonly ApiNodeMng apiNodeMng = new ApiNodeMng();

        public IEnumerable<IApiNode> apiNodes => apiNodeMng.apiNodes;


        public ApiNodeMng ApiNodeMng => apiNodeMng;


        static readonly ApiMessage const_ApiReply_Err_Timeout = new ApiMessage().InitAsApiReplyMessageByError(SsError.Err_HandleTimeout);



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
            public IOrganizeConnection  conn;
            public ApiMessage apiRequest;
            public ApiMessage apiReply;
            public Object sender;
            public Action<object, ApiMessage> callback;
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CallApiAsync(Object sender, ApiMessage apiRequest, Action<object, ApiMessage> callback)
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
            if (workThread.IsRunning) return false;
            try
            {
                if (workThreadCount > 0)
                {
                    workThread.Start();
                    Logger.Info("[LocalApiService] Started,workThreadCount:" + workThreadCount);

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
            if (!workThread.IsRunning) return;

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
        void Consumer_OnFinish(RequestInfo requestInfo)
        {
            //调用请求回调
            requestInfo.callback(requestInfo.sender, requestInfo.apiReply);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void Consumer_OnTimeout(RequestInfo requestInfo)
        {
            ApiMessage apiReply = new ApiMessage();
            apiReply.rpcContextData_OriData = const_ApiReply_Err_Timeout.rpcContextData_OriData;
            apiReply.value_OriData = const_ApiReply_Err_Timeout.value_OriData;

            //调用请求回调
            requestInfo.callback(requestInfo.sender, apiReply);
        }

        #endregion

        #endregion









    }
}
