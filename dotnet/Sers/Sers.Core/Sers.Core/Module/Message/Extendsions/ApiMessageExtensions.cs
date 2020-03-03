using Sers.Core.Module.Rpc;
using System;
using Sers.Core.Module.Message;
using Vit.Core.Util.ComponentModel.Data;
using Vit.Core.Util.ComponentModel.SsError;
using Newtonsoft.Json.Linq;

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
        /// <param name="url"> /api/cotrollers1/value?name=lith </param>
        /// <param name="arg"></param>
        /// <param name="httpMethod">可为 GET、POST、DELETE、PUT等,可不指定</param>
        /// <returns></returns>
        public static ApiMessage InitAsApiRequestMessage(this ApiMessage apiRequestMessage, string url, Object arg=null,string httpMethod=null)
        {   

            var rpcData = RpcFactory.Instance.CreateRpcContextData().InitFromRpcContext();

            //(x.1)设置url
            rpcData.http_url_Set("http://sers.internal" + url);

            //问号的位置
            var queryIndex = url.IndexOf('?');

            #region (x.2)设置route
            //去除query string(url ?后面的字符串)           
            {
                // b2?a=c
                if (queryIndex >= 0)
                {
                    rpcData.route = url.Substring(0, queryIndex);
                }
                else
                {
                    rpcData.route = url;
                }
            }
            #endregion

            #region (x.3)设置body
            {
                ArraySegment<byte> bodyData;
                if (arg != null && (bodyData = arg.SerializeToArraySegmentByte()).HasData())
                {
                    apiRequestMessage.value_OriData = bodyData;
                }
                else 
                {
                    //从 query获取数据
                    if (queryIndex >= 0)
                    {
                        try
                        {
                            // ?a=1&b=2
                            var query = url.Substring(queryIndex );
                            var kvs = System.Web.HttpUtility.ParseQueryString(query);

                            JObject data = new JObject();
                            foreach (string key in kvs)
                            {
                                var value = kvs.Get(key);
                                data[key] = value;
                            }
                            apiRequestMessage.value_OriData = data.SerializeToArraySegmentByte();
                        }
                        catch (Exception ex)
                        {                           
                        }                       
                    }
                }
                
            }
            #endregion

                       
            if (httpMethod != null) rpcData.http_method_Set(httpMethod);


            apiRequestMessage.RpcContextData_OriData_Set(rpcData);

            return apiRequestMessage;
        }
        #endregion
    }
}
