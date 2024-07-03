using System;

using Newtonsoft.Json.Linq;

using Sers.Core.Module.Message;
using Sers.Core.Module.Rpc;

using Vit.Core.Module.Log;
using Vit.Core.Util.ComponentModel.Data;
using Vit.Extensions;
using Vit.Extensions.Json_Extensions;

namespace Sers.Core.Module.Api.ApiEvent.BeforeCallApi.Bearer
{
    /// <summary>
    /// 在调用接口前，会获取 rpcData.http.headers.Authorization(格式为 "Bearer xxxxxx")，并作为参数调用接口api_verifyAt，把返回数据放到 rpcData.user.userInfo
    /// </summary>
    public class Bearer : IBeforeCallApi
    {
        public void Init(JObject config)
        {
            Api_verifyAt = config["api_verifyAt"].Value<String>();

            Api_httpMethod = config["api_httpMethod"].Value<String>();
            if (string.IsNullOrWhiteSpace(Api_httpMethod)) Api_httpMethod = null;
        }


        string Api_verifyAt;
        string Api_httpMethod;

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        ApiReturn<Object> VerifyAt(string at)
        {
            //var arg = "{\"at\":\"" + at + "\"}";
            return ApiClient.CallRemoteApi<ApiReturn<Object>>(Api_verifyAt, new { at }, Api_httpMethod);
        }




        /// <summary>
        /// Bearer。 转换at为对应的用户
        /// </summary>
        /// <param name="rpcData"></param>
        /// <param name="requestMessage"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void BeforeCallApi(RpcContextData rpcData, ApiMessage requestMessage)
        {
            try
            {
                #region Bearer 转换为对应的用户
                var bear = rpcData.Authorization_Get();
                if (string.IsNullOrWhiteSpace(bear))
                    return;

                if (null != rpcData.user) return;

                ApiReturn<Object> ret;
                using (var rpcContext = new RpcContext())
                {
                    //RpcContext.Current.rpcData = rpcData;
                    rpcContext.rpcData = rpcData;
                    ret = VerifyAt(bear);
                }

                if (null != ret && ret.success)
                {
                    rpcData.user = new { userInfo = ret.data };
                    requestMessage.rpcContextData_OriData = ArraySegmentByteExtensions.Null;
                }
                #endregion
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}
