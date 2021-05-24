using Sers.Core.Module.Rpc;
using System;
using Sers.Core.Module.Message;
using Vit.Core.Util.ComponentModel.Data;
using Vit.Core.Util.ComponentModel.SsError;
using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;

namespace Vit.Extensions
{
    public static partial class ApiMessageExtensions
    {
        #region Init

        static readonly string Response_ContentType_Json = ("application/json; charset=" + Vit.Core.Module.Serialization.Serialization_Newtonsoft.Instance.charset);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ApiMessage InitAsApiReplyMessageByError(this ApiMessage data, SsError error)
        {
            if (data == null || error == null) return data;


            var rpcData = new RpcContextData();

            #region (x.1) headers
            var headers = rpcData.http.Headers();
            rpcData.error = error;

            headers["Content-Type"] = Response_ContentType_Json;

            headers["responseState"] = "fail";
            headers["responseError_Base64"] = error?.SerializeToBytes()?.BytesToBase64String();

            #endregion

            //(x.2)statusCode
            rpcData.http.statusCode = error.errorCode;
            data.RpcContextData_OriData_Set(rpcData);


            #region (x.3) set body          
            ApiReturn ret = error;
            data.value_OriData = ret.SerializeToArraySegmentByte();
            #endregion

            return data;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ApiMessage SetValue(this ApiMessage apiRequestMessage, string url, Object arg = null)
        {
            #region (x.2)设置body
            {
                ArraySegment<byte> bodyData;
                if (arg != null && (bodyData = arg.SerializeToArraySegmentByte()).HasData())
                {
                    apiRequestMessage.value_OriData = bodyData;
                }
                else
                {
                    //问号的位置
                    var queryIndex = url.IndexOf('?');

                    //从 query获取数据
                    if (queryIndex >= 0)
                    {
                        try
                        {
                            // ?a=1&b=2
                            var query = url.Substring(queryIndex);
                            var kvs = System.Web.HttpUtility.ParseQueryString(query);

                            JObject data = new JObject();
                            foreach (string key in kvs)
                            {
                                var value = kvs.Get(key);
                                data[key] = value;
                            }
                            apiRequestMessage.value_OriData = data.SerializeToArraySegmentByte();
                        }
                        catch
                        {
                        }
                    }
                }
            }
            #endregion

            return apiRequestMessage;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiRequestMessage"></param>
        /// <param name="url"> /api/cotrollers1/value?name=lith </param>
        /// <param name="arg"></param>
        /// <param name="httpMethod">可为 GET、POST、DELETE、PUT等,可不指定</param>
        /// <param name="InitRpc">对Rpc的额外处理,如添加header</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ApiMessage InitAsApiRequestMessage(this ApiMessage apiRequestMessage, string url, Object arg=null,string httpMethod=null,Action<RpcContextData>InitRpc=null)
        {   
            //(x.1)初始化rpcData
            var rpcData = new RpcContextData().InitFromRpcContext().Init(url, httpMethod);
            InitRpc?.Invoke(rpcData);
            apiRequestMessage.RpcContextData_OriData_Set(rpcData);

            //(x.2)设置body
            SetValue(apiRequestMessage, url,arg);    

            return apiRequestMessage;
        }
        #endregion
    }
}
