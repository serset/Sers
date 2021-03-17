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
        public static void SetOnSendRequest(Func<Vit.Core.Util.Pipelines.ByteData, ArraySegment<byte>>[] callbacks)
        {
            Instances = new ApiClient[callbacks.Length];

            Instances[0] = Instance;
            Instance.OnSendRequest = callbacks[0];

            for (int i = 1; i < callbacks.Length; i++)
            {
                Instances[i] = new ApiClient { OnSendRequest = callbacks[i] };
            }
        }

        #endregion




        #region CallApi

        private Func<Vit.Core.Util.Pipelines.ByteData, ArraySegment<byte>> OnSendRequest { get; set; }


        #region CallApi 原始

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reqOri"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ArraySegment<byte> CallApi(Vit.Core.Util.Pipelines.ByteData reqOri)
        {
            return OnSendRequest(reqOri);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ApiMessage CallApi(ApiMessage request)
        {
            try
            {
                var reply = CallApi(request.Package());
                if (null == reply || reply.Count == 0)
                {
                    //Logger.Error(SsError.Err_Timeout.ToException());
                    //返回请求超时，无回应数据
                    return new ApiMessage().InitAsApiReplyMessageByError(SsError.Err_Timeout);
                }
                return new ApiMessage(reply);
            }
            catch (Exception ex) when (!(ex.GetBaseException() is ThreadInterruptedException))
            {
                ex = ex.GetBaseException();

                Logger.Error(ex);
       
                return new ApiMessage().InitAsApiReplyMessageByError(ex);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<ApiMessage> CallApiAsync(ApiMessage request)
        {
            ApiMessage reply = null;
            await Task.Run(() => { reply = CallApi(request); });
            return reply;
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
        public ReturnType CallApi<ReturnType>(string route, Object arg = null, string httpMethod = null, Action<RpcContextData> InitRpc = null)
        {
            var apiRequestMessage = new ApiMessage().InitAsApiRequestMessage(route, arg, httpMethod, InitRpc);

            var apiReplyMessage = CallApi(apiRequestMessage);

            return apiReplyMessage.value_OriData.DeserializeFromArraySegmentByte<ReturnType>();
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
            ReturnType ret = default(ReturnType);
            await Task.Run(() => { ret = CallApi<ReturnType>(route, arg, httpMethod,InitRpc); });
            return ret;
        }

        #endregion


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
