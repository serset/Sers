using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Sers.Core.Module.Message;
using Sers.Core.Module.Rpc;

using Vit.Core.Util.ComponentModel.Data;
using Vit.Core.Util.ComponentModel.SsError;
using Vit.Extensions.Serialize_Extensions;
using Vit.Extensions.Serialize_Extensions;

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

            #region #1 headers
            var headers = rpcData.http.Headers();
            rpcData.error = error;

            headers["Content-Type"] = Response_ContentType_Json;

            headers["responseState"] = "fail";
            headers["responseError_Base64"] = error?.SerializeToBytes()?.BytesToBase64String();

            #endregion

            // #2 statusCode
            rpcData.http.statusCode = error.errorCode;
            data.RpcContextData_OriData_Set(rpcData);


            // #3 set body
            ApiReturn ret = error;
            data.value_OriData = ret.SerializeToArraySegmentByte();

            return data;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ApiMessage SetValue(this ApiMessage apiRequestMessage, string url, Object arg = null)
        {
            ArraySegment<byte> bodyData;
            if (arg is ArraySegment<byte> asByte)
            {
                apiRequestMessage.value_OriData = asByte;
            }
            else if (arg is byte[] bytes)
            {
                apiRequestMessage.value_OriData = bytes.BytesToArraySegmentByte();
            }
            else if (arg is string str)
            {
                apiRequestMessage.value_OriData = str.StringToArraySegmentByte();
            }
            else if (arg != null && (bodyData = arg.SerializeToArraySegmentByte()).HasData())
            {
                apiRequestMessage.value_OriData = bodyData;
            }
            else
            {
                var queryIndex = url.IndexOf('?');

                // get arguments from QueryString
                if (queryIndex >= 0)
                {
                    try
                    {
                        // ?a=1&b=2
                        var query = url.Substring(queryIndex);
                        var kvs = System.Web.HttpUtility.ParseQueryString(query);

                        var data = new Dictionary<string, string>();
                        foreach (string key in kvs)
                        {
                            var value = kvs.Get(key);
                            data[key] = value;
                        }
                        apiRequestMessage.value_OriData = data.SerializeToArraySegmentByte();
                    }
                    catch
                    { }
                }
            }
            return apiRequestMessage;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiRequestMessage"></param>
        /// <param name="url"> /api/controllers1/value?name=lith </param>
        /// <param name="arg"></param>
        /// <param name="httpMethod"> could be GET , POST , DELETE , PUT ...  and also could be null</param>
        /// <param name="InitRpc">extra actions to rpc, like add extra headers</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ApiMessage InitAsApiRequestMessage(this ApiMessage apiRequestMessage, string url, Object arg = null, string httpMethod = null, Action<RpcContextData> InitRpc = null)
        {
            // #1 init rpcData
            var rpcData = new RpcContextData().InitFromRpcContext().Init(url, httpMethod);
            InitRpc?.Invoke(rpcData);
            apiRequestMessage.RpcContextData_OriData_Set(rpcData);

            // #2 set body
            SetValue(apiRequestMessage, url, arg);

            return apiRequestMessage;
        }
        #endregion
    }
}
