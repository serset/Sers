using System;
using Vit.Core.Module.Log;
using Vit.Extensions;
using System.Threading;
using System.Threading.Tasks;
using Sers.Core.Module.Message;
using Vit.Core.Util.ComponentModel.SsError;
using Sers.Core.Module.Rpc;
using System.Runtime.CompilerServices;

namespace Sers.Core.Module.Api
{
    public class ApiClient
    {
        #region static Instance Instances

        public static readonly ApiClient Instance = new ApiClient();
        public static ApiClient[] Instances { get; private set; }

        /// <summary>
        /// callbacks长度必须大于1
        /// </summary>
        /// <param name="callbacks"></param>
        public static void SetOnSendRequest(Action<Vit.Core.Util.Pipelines.ByteData, Action<ArraySegment<byte>>>[] callbacks,int requestTimeoutMs)
        {
            Instances = new ApiClient[callbacks.Length];

            Instances[0] = Instance;
            Instance.OnSendRequest = callbacks[0];
            Instance.requestTimeoutMs = requestTimeoutMs;

            for (int i = 1; i < callbacks.Length; i++)
            {
                Instances[i] = new ApiClient { OnSendRequest = callbacks[i], requestTimeoutMs= requestTimeoutMs };
            }
        }

        #endregion




        #region 成员变量

        int requestTimeoutMs;

        private Action<Vit.Core.Util.Pipelines.ByteData, Action<ArraySegment<byte>>> OnSendRequest { get; set; }
        #endregion




        #region CallApi 原始

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CallApiAsync(Vit.Core.Util.Pipelines.ByteData apiRequestData, Action<ArraySegment<byte>> callback)
        {
            OnSendRequest(apiRequestData, callback);
        }

        #endregion



        #region CallApi 

        #region static curAutoResetEvent      
        public static AutoResetEvent curAutoResetEvent =>
            _curAutoResetEvent.Value ?? (_curAutoResetEvent.Value = new AutoResetEvent(false));

        static System.Threading.ThreadLocal<AutoResetEvent> _curAutoResetEvent = new System.Threading.ThreadLocal<AutoResetEvent>();
        #endregion



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ApiMessage CallApi(ApiMessage apiRequestMessage)
        {
            try
            {
                AutoResetEvent mEvent = curAutoResetEvent;
                mEvent.Reset();

                try
                {
                    ApiMessage apiReplyMessage = null;

                    CallApiAsync(apiRequestMessage.Package(), (apiReplyData) => {
                        apiReplyMessage = new ApiMessage(apiReplyData);
                        mEvent?.Set();
                    });


                    if (mEvent.WaitOne(requestTimeoutMs))
                    {
                        return apiReplyMessage;
                    }
                }
                finally
                {
                    mEvent = null;
                }


                //Logger.Error(SsError.Err_Timeout.ToException());
                //返回请求超时，无回应数据
                return new ApiMessage().InitAsApiReplyMessageByError(SsError.Err_Timeout);
            }
            catch (Exception ex) when (!(ex.GetBaseException() is ThreadInterruptedException))
            {
                ex = ex.GetBaseException();

                Logger.Error(ex);

                return new ApiMessage().InitAsApiReplyMessageByError(ex);
            }
        }
        #endregion







        #region CallApi 扩展


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="ReturnType"></typeparam>
        /// <param name="route"></param>
        /// <param name="arg"></param>
        /// <param name="httpMethod">可为 GET、POST、DELETE、PUT等,可不指定</param>
        /// <param name="InitRpc">对Rpc的额外处理,如添加header</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ArraySegment<byte> CallApiWithBytes(string route, Object arg = null, string httpMethod = null, Action<RpcContextData> InitRpc = null)
        {
            var apiRequestMessage = new ApiMessage().InitAsApiRequestMessage(route, arg, httpMethod, InitRpc);

            var apiReplyMessage = CallApi(apiRequestMessage);

            return apiReplyMessage.value_OriData;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="ReturnType"></typeparam>
        /// <param name="route"></param>
        /// <param name="arg"></param>
        /// <param name="httpMethod">可为 GET、POST、DELETE、PUT等,可不指定</param>
        /// <param name="InitRpc">对Rpc的额外处理,如添加header</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReturnType CallApi<ReturnType>(string route, Object arg = null, string httpMethod = null, Action<RpcContextData> InitRpc = null)
        {
            ArraySegment<byte> replyValue = CallApiWithBytes(route, arg, httpMethod, InitRpc);
            if (replyValue.Count == 0) return default;
            return replyValue.DeserializeFromArraySegmentByte<ReturnType>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="route"></param>
        /// <param name="arg"></param>
        /// <param name="httpMethod">可为 GET、POST、DELETE、PUT等,可不指定</param>
        /// <param name="InitRpc">对Rpc的额外处理,如添加header</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string CallApi(string route, Object arg = null, string httpMethod = null, Action<RpcContextData> InitRpc = null)
        {
            return CallApi<string>(route, arg, httpMethod, InitRpc);
        }


        #endregion


        #region CallApiAsync

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<ApiMessage> CallApiAsync(ApiMessage request)
        {
            ApiMessage apiReplyMessage = null;
            await Task.Run(() => { apiReplyMessage = CallApi(request); });
            return apiReplyMessage;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="ReturnType"></typeparam>
        /// <param name="route"></param>
        /// <param name="arg"></param>
        /// <param name="httpMethod">可为 GET、POST、DELETE、PUT等,可不指定</param>
        /// <param name="InitRpc">对Rpc的额外处理,如添加header</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<ReturnType> CallApiAsync<ReturnType>(string route, Object arg = null, string httpMethod = null, Action<RpcContextData> InitRpc = null)
        {
            ReturnType ret = default;
            await Task.Run(() => { ret = CallApi<ReturnType>(route, arg, httpMethod, InitRpc); });
            return ret;
        }

     

        #endregion






    



        #region static CallRemoteApi 扩展

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ApiMessage CallRemoteApi(ApiMessage request)
        {
            return Instance.CallApi(request);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="route"></param>
        /// <param name="arg"></param>
        /// <param name="httpMethod">可为 GET、POST、DELETE、PUT等,可不指定</param>
        /// <param name="InitRpc">对Rpc的额外处理,如添加header</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string CallRemoteApi(string route, string arg, string httpMethod = null, Action<RpcContextData> InitRpc = null)
        {
            return Instance.CallApi(route, arg, httpMethod, InitRpc);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="route"></param>
        /// <param name="arg"></param>
        /// <param name="httpMethod">可为 GET、POST、DELETE、PUT等,可不指定</param>
        /// <param name="InitRpc">对Rpc的额外处理,如添加header</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string CallRemoteApi(string route, Object arg = null, string httpMethod = null, Action<RpcContextData> InitRpc = null)
        {
            return Instance.CallApi(route, arg, httpMethod, InitRpc);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="ReturnType"></typeparam>
        /// <param name="route"></param>
        /// <param name="arg"></param>
        /// <param name="httpMethod">可为 GET、POST、DELETE、PUT等,可不指定</param>
        /// <param name="InitRpc">对Rpc的额外处理,如添加header</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReturnType CallRemoteApi<ReturnType>(string route, Object arg = null, string httpMethod = null, Action<RpcContextData> InitRpc = null)
        {
            return Instance.CallApi<ReturnType>(route, arg, httpMethod, InitRpc);
        }





        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<ApiMessage> CallRemoteApiAsync(ApiMessage request)
        {   
            return await Instance.CallApiAsync(request);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="ReturnType"></typeparam>
        /// <param name="route"></param>
        /// <param name="arg"></param>
        /// <param name="httpMethod">可为 GET、POST、DELETE、PUT等,可不指定</param>
        /// <param name="InitRpc">对Rpc的额外处理,如添加header</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task<ReturnType> CallRemoteApiAsync<ReturnType>(string route, Object arg = null, string httpMethod = null, Action<RpcContextData> InitRpc = null)
        {
            return await Instance.CallApiAsync<ReturnType>(route, arg, httpMethod, InitRpc);
        }
        #endregion


    }
}
