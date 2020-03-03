using System;
using System.Collections.Generic;
using Vit.Core.Module.Log;
using Vit.Extensions;
using System.Threading;
using System.Threading.Tasks;
using Sers.Core.Module.Message;
using Vit.Core.Util.ComponentModel.SsError;

namespace Sers.Core.Module.Api
{
    public class ApiClient
    {
        #region static

        public static readonly ApiClient Instance = new ApiClient();
        public static ApiClient[] Instances { get; private set; }

        /// <summary>
        /// callbacks长度必须大于1
        /// </summary>
        /// <param name="callbacks"></param>
        public static void SetOnSendRequest(Func<List<ArraySegment<byte>>, ArraySegment<byte>>[] callbacks)
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




        #region ApiClient

        private Func<List<ArraySegment<byte>>, ArraySegment<byte>> OnSendRequest { get; set; }


        #region CallApi 原始

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reqOri"></param>
        /// <returns></returns>
        private ArraySegment<byte> CallApi(List<ArraySegment<byte>> reqOri)
        {
            return OnSendRequest(reqOri);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
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

        #endregion


        #region CallApi 扩展

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="ReturnType"></typeparam>
        /// <param name="route"></param>
        /// <param name="arg"></param>
        /// <param name="httpMethod">可为 GET、POST、DELETE、PUT等,可不指定</param>
        /// <returns></returns>
        public ReturnType CallApi<ReturnType>(string route, Object arg, string httpMethod = null)
        {
            var apiRequestMessage = new ApiMessage().InitAsApiRequestMessage(route, arg, httpMethod);

            var apiReplyMessage = CallApi(apiRequestMessage);

            return apiReplyMessage.value_OriData.DeserializeFromArraySegmentByte<ReturnType>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="route"></param>
        /// <param name="arg"></param>
        /// <param name="httpMethod">可为 GET、POST、DELETE、PUT等,可不指定</param>
        /// <returns></returns>
        public string CallApi(string route, Object arg, string httpMethod = null)
        {
            return CallApi<string>(route, arg, httpMethod);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="ReturnType"></typeparam>
        /// <param name="route"></param>
        /// <param name="arg"></param>
        /// <param name="httpMethod">可为 GET、POST、DELETE、PUT等,可不指定</param>
        /// <returns></returns>
        public async Task<ReturnType> CallApiAsync<ReturnType>(string route, Object arg, string httpMethod = null)
        {
            ReturnType ret = default(ReturnType);
            await Task.Run(() => { ret = CallApi<ReturnType>(route, arg, httpMethod); });
            return ret;
        }

        #endregion


        #endregion





        #region CallRemoteApi 扩展

        /// <summary>
        /// 
        /// </summary>
        /// <param name="route"></param>
        /// <param name="arg"></param>
        /// <param name="httpMethod">可为 GET、POST、DELETE、PUT等,可不指定</param>
        public static string CallRemoteApi(string route, string arg, string httpMethod = null)
        {
            return Instance.CallApi(route, arg, httpMethod);
        }

       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
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
        public static string CallRemoteApi(string route, Object arg, string httpMethod = null)
        {
            return Instance.CallApi(route, arg, httpMethod);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="ReturnType"></typeparam>
        /// <param name="route"></param>
        /// <param name="arg"></param>
        /// <param name="httpMethod">可为 GET、POST、DELETE、PUT等,可不指定</param>
        /// <returns></returns>
        public static ReturnType CallRemoteApi<ReturnType>(string route, Object arg, string httpMethod = null)
        {
            return Instance.CallApi<ReturnType>(route, arg, httpMethod);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="ReturnType"></typeparam>
        /// <param name="route"></param>
        /// <param name="arg"></param>
        /// <param name="httpMethod">可为 GET、POST、DELETE、PUT等,可不指定</param>
        /// <returns></returns>
        public static Task<ReturnType> CallRemoteApiAsync<ReturnType>(string route, Object arg, string httpMethod = null)
        {
            return Instance.CallApiAsync<ReturnType>(route, arg, httpMethod);
        }
        #endregion

       
    }
}
