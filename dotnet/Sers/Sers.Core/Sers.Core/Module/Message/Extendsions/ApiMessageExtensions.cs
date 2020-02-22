using Sers.Core.Module.Rpc;
using System;
using Sers.Core.Module.Message;
using Vit.Core.Util.ComponentModel.Data;
using Vit.Core.Util.ComponentModel.SsError;

namespace Vit.Extensions
{
    public static partial class ApiMessageExtensions
    {
        #region Init

        public static ApiMessage InitAsApiReplyMessageByError(this ApiMessage data, SsError error)
        {
            if (data == null || error == null) return data;

            #region (x.1) set rpcData
            var rpcData = RpcFactory.Instance.CreateRpcContextData();
            rpcData.error_Set(error);

            data.RpcContextData_OriData_Set(rpcData);
            #endregion

            #region (x.2) set body          
            ApiReturn ret = error;
            data.value_OriData = ret.SerializeToArraySegmentByte();
            #endregion

            return data;
        }
     

    
 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiRequestMessage"></param>
        /// <param name="route"></param>
        /// <param name="arg"></param>
        /// <param name="httpMethod">可为 GET、POST、DELETE、PUT等,可不指定</param>
        /// <returns></returns>
        public static ApiMessage InitAsApiRequestMessage(this ApiMessage apiRequestMessage, string route, Object arg=null,string httpMethod=null)
        {
            apiRequestMessage.value_OriData = arg.SerializeToArraySegmentByte();

            var rpcData = RpcFactory.Instance.CreateRpcContextData().InitFromRpcContext();

            rpcData.route = route;
            rpcData.http_url_Set(route);

            if (httpMethod != null) rpcData.http_method_Set(httpMethod);


            apiRequestMessage.RpcContextData_OriData_Set(rpcData);

            return apiRequestMessage;
        }
        #endregion
    }
}
