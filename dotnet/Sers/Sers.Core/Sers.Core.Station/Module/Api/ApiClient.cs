using System;
using System.Collections.Generic;
using Sers.Core.Module.Api.Message;
using Sers.Core.Module.Log;
using Sers.Core.Util.SsError;
using Sers.Core.Extensions;
using System.Threading;
using System.Threading.Tasks;

namespace Sers.Core.Module.Api
{
    public class ApiClient
    {

        #region ApiClient

        public Func<List<ArraySegment<byte>>, ArraySegment<byte>> OnSendRequest;


        #region CallApi 原始

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reqOri"></param>
        /// <returns></returns>
        public ArraySegment<byte> CallApi(List<ArraySegment<byte>> reqOri)
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
                    //Logger.Error(new Exception().SsError_Set(SsError.Err_Timeout));
                    //返回请求超时，无回应数据
                    return new ApiMessage().InitByError(SsError.Err_Timeout);
                }
                return new ApiMessage(reply);
            }
            catch (Exception ex) when (!(ex is ThreadInterruptedException))
            {
                ex = ex.GetBaseException();

                Logger.Error(ex);
       
                return new ApiMessage().InitByError(ex);
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
        /// <returns></returns>
        public ReturnType CallApi<ReturnType>(string route, Object arg)
        {
            var apiRequestMessage = new ApiMessage().InitAsApiRequestMessage(route, arg);

            var apiReplyMessage = CallApi(apiRequestMessage);

            return Serialization.Serialization.Instance.Deserialize<ReturnType>(apiReplyMessage.value_OriData);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="route"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        public string CallApi(string route, Object arg)
        {
            return CallApi<string>(route, arg);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="ReturnType"></typeparam>
        /// <param name="route"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        public async Task<ReturnType> CallApiAsync<ReturnType>(string route, Object arg)
        {
            ReturnType ret = default(ReturnType);
            await Task.Run(() => { ret = CallApi<ReturnType>(route, arg); });
            return ret;
        }

        #endregion


        #endregion



        #region static

        public static readonly ApiClient Instance = new ApiClient();



        #region CallRemoteApi 扩展

        
        public static string CallRemoteApi(string route, string arg)
        {
            return Instance.CallApi(route, arg);
        }

       

        public static ApiMessage CallRemoteApi(ApiMessage request)
        {
            return Instance.CallApi(request); 
        }

        public static string CallRemoteApi(string route, Object arg)
        {
            return Instance.CallApi(route, arg);
        }


        public static ReturnType CallRemoteApi<ReturnType>(string route, Object arg)
        {
            return Instance.CallApi<ReturnType>(route, arg);
        }


        public static Task<ReturnType> CallRemoteApiAsync<ReturnType>(string route, Object arg)
        {
            return Instance.CallApiAsync<ReturnType>(route, arg);
        }
        #endregion

        #endregion
    }
}
