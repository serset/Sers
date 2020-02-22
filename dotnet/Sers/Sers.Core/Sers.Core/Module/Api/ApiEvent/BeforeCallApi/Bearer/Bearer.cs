using Newtonsoft.Json.Linq;
using Sers.Core.Module.Rpc;
using System;
using Sers.Core.Module.Message;
using Vit.Core.Module.Log;
using Vit.Extensions;
using Vit.Core.Util.ComponentModel.Data;

namespace Sers.Core.Module.Api.ApiEvent.BeforeCallApi.Bearer
{
    /// <summary>
    /// 在调用接口前，会获取 rpcData.http.headers.Authorization(格式为 "Bearer xxxxxx")，并作为参数调用接口api_verifyAt，把返回数据放到 rpcData.user.userInfo
    /// </summary>
    public class Bearer: IBeforeCallApi
    {
        public void Init(JObject config)
        {   
            Api_verifyAt = config["api_verifyAt"].Value<String>();

            Api_httpMethod = config["api_httpMethod"].Value<String>();
            if (string.IsNullOrWhiteSpace(Api_httpMethod)) Api_httpMethod = null;
        }


        string Api_verifyAt;
        string Api_httpMethod;

        public  ApiReturn<JObject> VerifyAt(string at)
        {
            //var arg = "{\"at\":\"" + at + "\"}";
            return ApiClient.CallRemoteApi<ApiReturn<JObject>>(Api_verifyAt, new { at }, Api_httpMethod);
        }


      

        /// <summary>
        /// Bearer。 转换at为对应的用户
        /// </summary>
        /// <param name="rpcData"></param>
        /// <param name="requestMessage"></param>
        public void BeforeCallApi(IRpcContextData rpcData, ApiMessage requestMessage)
        {
            try
            {
                #region Bearer 转换为对应的用户
                var bear = rpcData.Bearer_Get();
                if (string.IsNullOrWhiteSpace(bear))
                    return;

                if (null != rpcData.user_userInfo_Get()) return;

                ApiReturn<JObject> ret;
                using (var rpcContext = RpcFactory.Instance.CreateRpcContext())
                {
                    RpcContext.Current.rpcData=rpcData;
                    ret = VerifyAt(bear);
                }

                if (null != ret && ret.success)
                {
                    rpcData.user_userInfo_Set(ret.data);
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
