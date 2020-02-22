
using Newtonsoft.Json.Linq;
using System;
using Sers.Core.Module.Rpc;
using Sers.Core.Module.Api.Rpc;
using Vit.Core.Util.Common;
using System.IO;
using Vit.Core.Util.ComponentModel.SsError;

namespace Vit.Extensions
{
    /// <summary>
    /// Extension methods for RpcContextData
    /// </summary>
    public static partial class IRpcContextDataExtensions
    {



        #region Init

        public static IRpcContextData Init(this IRpcContextData rpcData, String callerSource)
        {
            var rid = CommonHelp.NewGuid();


            var caller = rpcData.oriJson.GetOrCreateJObject("caller");
            caller["rid"] = rid;
            caller["source"] = callerSource;

            //rpcData.caller_rid_Set(rid);
            //rpcData.caller_source_Set(callerSource);

            return rpcData;
        }

        public static IRpcContextData Init(this IRpcContextData rpcData, ECallerSource callerSource = ECallerSource.Internal)
        {
            return Init(rpcData, callerSource.EnumToString());
        }
        #endregion

        #region InitFromRpcContext

        public static IRpcContextData InitFromRpcContext(this IRpcContextData rpcData)
        {
            rpcData.Init();

            var rpcDataFromContext = RpcContext.RpcData;
            if (null == rpcDataFromContext) return rpcData;


            #region (x.2) caller_callStack
            String parentRid = rpcDataFromContext.caller_rid_Get();
            var callStack = rpcDataFromContext.caller_callStack_Get();
            if (null == callStack || callStack.Count == 0)
            {
                callStack = new JArray();
            }
            else
            {
                callStack = new JArray(callStack);
                //callStack = callStack.ConvertBySerialize<JArray>();
            }
            callStack.Add(parentRid);
            rpcData.caller_callStack_Set(callStack);
            #endregion


            //(x.3) http
            //rpcData.http_Set(rpcDataFromContext.http_Get());

            //(x.4) user
            rpcData.user_Set(rpcDataFromContext.user_Get());


            return rpcData;
        }
        #endregion


        #region ValueSetByPath
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="value"></param>
        /// <param name="path">value在data中的路径，至少一层，例如：new []{"taskList"}</param>
        public static void ValueSetByPath(this IRpcContextData data,object value, params object[] path)
        {
            data?.oriJson?.ValueSetByPath(value, path);
        }
        #endregion

        #region StringGetByPath
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="path">value在data中的路径，至少一层，例如：new []{"taskList"}</param>
        /// <returns></returns>
        public static String StringGetByPath(this IRpcContextData data, params object[] path)
        {
            if (data?.oriJson == null) return null;
            return data.oriJson.StringGetByPath(path);
        }
        #endregion


        #region caller

        #region caller
        public static JObject caller_Get(this IRpcContextData data)
        {
            return data.oriJson.GetOrCreateJObject("caller");
        }

        public static void caller_Set(this IRpcContextData data, JObject value)
        {
            if (null == data) return;
            data.oriJson["caller"] = value;
        }
        #endregion


        #region caller_source
        public static String caller_source_Get(this IRpcContextData data)
        {
            return data?.oriJson?["caller"]?["source"]?.ConvertToString();
        }

        public static void caller_source_Set(this IRpcContextData data, String value)
        {
            if (null == data) return;
            data.oriJson.GetOrCreateJObject("caller")["source"] = value;
        }


        public static ECallerSource? caller_source_GetEnum(this IRpcContextData data)
        {
            return data.caller_source_Get()?.ToECallerSource();
        }
        public static void caller_source_Set(this IRpcContextData data, ECallerSource? value)
        {
            data?.caller_source_Set(null == value ? null : value.Value.EnumToString());
        }
        #endregion


        #region caller_rid
        public static String caller_rid_Get(this IRpcContextData data)
        {
            return data?.oriJson?["caller"]?["rid"]?.ConvertToString();
        }

        public static void caller_rid_Set(this IRpcContextData data, String value)
        {
            if (null == data) return;
            data.oriJson.GetOrCreateJObject("caller")["rid"] = value;
        }
        #endregion

        #region caller_callStack
        public static JArray caller_callStack_Get(this IRpcContextData data)
        {
            JArray ja = null;
            data?.oriJson?["caller"]?["callStack"]?.TryParse(out ja);
            return ja;
        }

        public static void caller_callStack_Set(this IRpcContextData data, JArray value)
        {
            if (null == data) return;
            data.oriJson.GetOrCreateJObject("caller")["callStack"] = value;
        }

        
        public static String caller_parentRid_Get(this IRpcContextData data)
        {
            JArray ja = caller_callStack_Get(data);
            if (null == ja || ja.Count == 0) return null;
            return ja[ja.Count - 1].ConvertToString();
        }
        public static String caller_rootRid_Get(this IRpcContextData data)
        {
            JArray ja = caller_callStack_Get(data);
            if (null == ja || ja.Count == 0) return null;
            return ja[0].ConvertToString();
        }
        #endregion

        #endregion




        #region http

        #region http
        public static JObject http_Get(this IRpcContextData data)
        {
           return data.oriJson.GetOrCreateJObject("http");
        }

        public static void http_Set(this IRpcContextData data, JObject value)
        {
            if (null == data) return;        
            data.oriJson["http"] = value;
        }
        #endregion


        #region http_url
        public static String http_url_Get(this IRpcContextData data)
        {
            return data?.oriJson?["http"]?["url"]?.ConvertToString();
        }

        public static void http_url_Set(this IRpcContextData data, String value)
        {
            if (null == data) return;
            data.oriJson.GetOrCreateJObject("http")["url"] = value;
        }
        #endregion


        #region http_url_search_Get
        /// <summary>
        /// 获取泛接口 路由 * 实际传递的内容。
        /// <para>（若 route为"/Station1/fold1/a/*"，url为"http://127.0.0.1/Station1/fold1/a/1/2.html?c=9",则search为"1/2.html?c=9"）</para>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static String http_url_search_Get(this IRpcContextData data)
        {
            // "http://127.0.0.1/Station1/fold1/a/1/2.html?c=9"
            var http_url = data.http_url_Get();

            // "/Station1/fold1/a/*"
            var route = data.route;

            int index = http_url.IndexOf(route.Substring(0, route.Length - 1));
            if (index < 0) return null;

            return http_url.Substring(index + route.Length - 1);   
        }
        #endregion


        #region http_url_RelativePath_Get

        /// <summary>
        /// 获取当前url对应的相对文件路径(若不合法，则返回null)。demo:"rpc/2.html"
        /// <para>（若 route为"/Station1/fold1/a/*"，url为"http://127.0.0.1/Station1/fold1/a/1/2.html?c=9",则 relativePath为"1/2.html"）</para>
        /// </summary>
        /// <returns></returns>
        public static string http_url_RelativeUrl_Get(this IRpcContextData rpcData)
        {
            // "1/2.html?c=9"
            var search = rpcData.http_url_search_Get();
            if (String.IsNullOrEmpty(search)) return null;
            var index = search.IndexOf('?');

            String relativePath = search;
            if (index >= 0)
            {
                relativePath = relativePath.Substring(0, index);
            }
            return relativePath;
        }
        #endregion


        #region http_url_RelativePath_Get

        /// <summary>
        /// 获取当前url对应的相对文件路径(若不合法，则返回null,会自动转换文件夹分隔符)。demo:"rpc\\2.html"
        /// <para>（若 route为"/Station1/fold1/a/*"，url为"http://127.0.0.1/Station1/fold1/a/1/2.html?c=9",则 relativePath为"1\\2.html"）</para>
        /// </summary>
        /// <returns></returns>
        public static string http_url_RelativePath_Get(this IRpcContextData rpcData)
        {
            String relativePath= http_url_RelativeUrl_Get(rpcData);
            
            return relativePath?.Replace('/', Path.DirectorySeparatorChar).Replace("..","");
        }
        #endregion

        #region http_method

        public static String http_method_Get(this IRpcContextData data)
        {
            return data?.StringGetByPath("http", "method");
        }

        public static IRpcContextData http_method_Set(this IRpcContextData data, String value)
        {
            data?.ValueSetByPath(value, "http", "method");
            return data;
        }
        #endregion

        #region http_statusCode

        public static int? http_statusCode_Get(this IRpcContextData data)
        {
            int status=200;
            if (data?.oriJson?.JTokenGetByPath("http", "statusCode")?.TryParseIgnore(out status) ?? false)
            {
                return status;
            }
            return null;           
        }

        public static IRpcContextData http_statusCode_Set(this IRpcContextData data, int? value)
        {
            data?.ValueSetByPath(value, "http", "statusCode");
            return data;
        }
        #endregion

        #region http_protocol
        /// <summary>
        /// 如 "HTTP/2.0"
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static String http_protocol_Get(this IRpcContextData data)
        {
            return data?.StringGetByPath("http", "protocol");
        }

        /// <summary>
        /// 如 "HTTP/2.0"
        /// </summary>
        /// <param name="data"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IRpcContextData http_protocol_Set(this IRpcContextData data, String value)
        {
            data?.ValueSetByPath(value, "http", "protocol");
            return data;
        }
        #endregion      

        #region http_headers
        public static JObject http_headers_Get(this IRpcContextData data)
        {
            return data?.oriJson?.JTokenGetByPath("http", "headers") as JObject;
        }

        public static IRpcContextData http_headers_Set(this IRpcContextData data, JObject value)
        {
            data?.ValueSetByPath(value, "http", "headers");
            return data;
        }




        public static String http_header_Get(this IRpcContextData data, String key)
        {
            return data?.StringGetByPath("http", "headers", key);
        }       

        public static IRpcContextData http_header_Set(this IRpcContextData data,String key,String value)
        {
            data?.ValueSetByPath(value, "http", "headers", key);
            return data;
        }


        #region Content-Type
        public static String http_header_ContentType_Get(this IRpcContextData data)
        {
            return http_header_Get(data,"Content-Type");
        }

        public static IRpcContextData http_header_ContentType_Set(this IRpcContextData data, String value)
        {         
            return http_header_Set(data,"Content-Type", value);
        }
        #endregion

        #endregion


        #endregion


        #region user

        #region user
        public static JObject user_Get(this IRpcContextData data)
        {
            return data.oriJson.GetOrCreateJObject("user");
        }

        public static void user_Set(this IRpcContextData data, JObject value)
        {
            if (null == data) return;
            data.oriJson["user"] = value;
        }
        #endregion

        #region user_userInfo
        public static JObject user_userInfo_Get(this IRpcContextData data)
        {
            return data?.oriJson?["user"]?["userInfo"].GetValue() as JObject;
        }
        public static void user_userInfo_Set(this IRpcContextData data, JObject value)
        {
            if (null == data) return;
            data.oriJson.GetOrCreateJObject("user")["userInfo"] = value;
        }
        #endregion

        #endregion


        #region error
        public static SsError error_Get(this IRpcContextData data)
        {
            return data.oriJson["error"].Deserialize<SsError>();
        }

        public static void error_Set(this IRpcContextData data, SsError value)
        {
            data.oriJson["error"] = value.ConvertBySerialize<JObject>();

            //data.http_statusCode_Set(value.errorCode);
        }
        #endregion










    }
}
