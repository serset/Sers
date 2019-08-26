using Newtonsoft.Json.Linq;
using Sers.Core.Module.Rpc;
using System;
using Sers.Core.Module.Api.Data;
using Sers.Core.Module.Log;
using Sers.Core.Extensions;
using Sers.Core.Module.Api.Message;
using Sers.ServiceStation.Module.Bearer.Service;

namespace Sers.ServiceStation.Module.Bearer
{
    public static partial class BearerHelp
    {


        /// <summary>
        /// Bearer。 转换at为对应的用户
        /// </summary>
        /// <param name="rpcData"></param>
        /// <param name="requestMessage"></param>
        public static void ConvertBearer(IRpcContextData rpcData, ApiMessage requestMessage)
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
                    ret = AccessTokenService.VerifyAt(bear);
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
