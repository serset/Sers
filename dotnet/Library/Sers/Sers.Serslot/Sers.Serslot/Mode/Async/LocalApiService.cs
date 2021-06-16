using System;
using System.Collections.Generic;
using System.Threading;
using Vit.Extensions;
using Vit.Core.Module.Log;
using Sers.Core.Module.Rpc;
using Sers.Core.Module.Message;
using Vit.Core.Util.ComponentModel.SsError;
using Sers.Core.Module.Api.LocalApi.Event;
using System.Runtime.CompilerServices;
using Sers.Core.Module.Api.LocalApi;
using System.Threading.Tasks;

namespace Sers.Serslot.Mode.Async
{
    public class LocalApiService: ILocalApiService
    {

        public LocalApiService(SerslotServer serslotServer) 
        {
            this.serslotServer = serslotServer;
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


        SerslotServer serslotServer;

        #region CallLocalApi
 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        async Task  CallLocalApiAsync(ApiMessage apiRequest, Object sender, Action<object, ApiMessage> callback)
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


                    // (x.3)ProcessRequestByRpcAsync
                    await serslotServer.ProcessRequestByRpcAsync(rpcContext);          

                }
                catch (Exception ex) when ((ex.GetBaseException() is ThreadInterruptedException))
                {
                    //处理超时
                    rpcContext.apiReplyMessage.rpcContextData_OriData = const_ApiReply_Err_Timeout.rpcContextData_OriData;
                    rpcContext.apiReplyMessage.value_OriData = const_ApiReply_Err_Timeout.value_OriData;
                    //throw;
                }
                catch (Exception ex) when (!(ex.GetBaseException() is ThreadInterruptedException))
                {
                    ex = ex.GetBaseException();
                    Logger.Error(ex);
                    SsError error = ex;
                    rpcContext.apiReplyMessage.InitAsApiReplyMessageByError(error);
                }

                callback(sender, rpcContext.apiReplyMessage);
            }
        }
        #endregion


        static readonly ApiMessage const_ApiReply_Err_Timeout = new ApiMessage().InitAsApiReplyMessageByError(SsError.Err_HandleTimeout);


        #region 后台服务
 



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CallApiAsync(Object sender, ApiMessage apiRequest, Action<object, ApiMessage> callback)
        {
            CallLocalApiAsync(apiRequest, sender, callback);
        }





        public bool Start()
        {
            return true;
        }


        public void Stop()
        {      
        }
     


       

        #endregion



    





    }
}
