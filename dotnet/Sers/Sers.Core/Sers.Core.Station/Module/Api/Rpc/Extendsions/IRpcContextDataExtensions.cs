
using Newtonsoft.Json.Linq;
using System;
using Sers.Core.Module.Rpc;
using Sers.Core.Module.Api.Rpc;
using Sers.Core.Util.Common;
using Sers.Core.Util.SsError;
using Newtonsoft.Json;

namespace Sers.Core.Extensions
{
    /// <summary>
    /// Extension methods for RpcContextData
    /// </summary>
    public static class IRpcContextDataExtensions
    {

      


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
        public static string caller_source_Get(this IRpcContextData data)
        {
            return data?.oriJson?["caller"]?["source"]?.ConvertToString();
        }

        public static void caller_source_Set(this IRpcContextData data, string value)
        {
            if (null == data) return;
            data.oriJson.GetOrCreateJObject("caller")["source"] = value;
        }


        public static ECallerSource? caller_source_GetEnum(this IRpcContextData data)
        {
            return data.caller_source_Get()?.StringToEnum<ECallerSource>();
        }
        public static void caller_source_Set(this IRpcContextData data, ECallerSource? value)
        {
            data?.caller_source_Set(null == value ? null : value.Value.EnumToString());
        }
        #endregion


        #region caller_rid
        public static string caller_rid_Get(this IRpcContextData data)
        {
            return data?.oriJson?["caller"]?["rid"]?.ConvertToString();
        }

        public static void caller_rid_Set(this IRpcContextData data, string value)
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

        public static void caller_callStack_Push(this IRpcContextData data, string parentRequestGuid)
        {
            if (null == data) return;

            var callStack = caller_callStack_Get(data) ?? new JArray();
            callStack.Add(parentRequestGuid);
            caller_callStack_Set(data, callStack);
        }
        public static string caller_parentRequestGuid_Get(this IRpcContextData data)
        {
            JArray ja = caller_callStack_Get(data);
            if (null == ja || ja.Count == 0) return null;
            return ja[ja.Count - 1].ConvertToString();
        }
        public static string caller_rootRequestGuid_Get(this IRpcContextData data)
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
        public static string http_url_Get(this IRpcContextData data)
        {
            return data?.oriJson?["http"]?["url"]?.ConvertToString();
        }

        public static void http_url_Set(this IRpcContextData data, string value)
        {
            if (null == data) return;
            data.oriJson.GetOrCreateJObject("http")["url"] = value;
        }
        #endregion


        #region http_url_search_Get
        /// <summary>
        /// 获取泛接口 路由 * 实际传递的内容。
        /// （若 route为"/Station1/fold1/a/*"，url为"http://127.0.0.1/Station1/fold1/a/b?c=9",则search为"b?c=9"）
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string http_url_search_Get(this IRpcContextData data)
        {
            // route  /Station1/fold1/a/*
            var route = data.route;

            // url  http://127.0.0.1/Station1/fold1/a/b?c=9
            var url = data.http_url_Get();
            string search=null;
            try
            {
                // route  /Station1/fold1/a/
                route = route.Substring(0, route.Length-1);
                var index = url.IndexOf(route);
                if (index >= 0)
                {
                    search = url.Substring(index+ route.Length);
                }
            }
            catch { }
            return search;
        }

       
        #endregion

        #region http_headers
        public static JObject http_headers_Get(this IRpcContextData data)
        {
            return data?.oriJson?["http"]?["headers"].GetValue() as JObject;
        }

        public static T http_header_Get<T>(this IRpcContextData data,string key)
        {
            var value = data.http_headers_Get()?[key];      
            return value.Deserialize<T>();
        }
        public static string http_header_Get(this IRpcContextData data, string key)
        {             
            return data.http_header_Get<string>(key);
        }
        public static void http_headers_Set(this IRpcContextData data, JObject value)
        {
            if (null == data) return;
            data.oriJson.GetOrCreateJObject("http")["headers"] = value;
        }
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
        }
        #endregion


















        #region Init
        public static IRpcContextData Init(this IRpcContextData rpcData, ECallerSource callerSource = ECallerSource.Internal)
        {
            return Init(rpcData, callerSource.EnumToString());
        }
        public static IRpcContextData Init(this IRpcContextData rpcData, string callerSource)
        {           

            var rid = CommonHelp.NewGuid();
            rpcData.caller_rid_Set(rid);

            rpcData.caller_source_Set(callerSource);

            return rpcData;
        }
        #endregion

        #region InitFromRpcContext

        public static IRpcContextData InitFromRpcContext(this IRpcContextData rpcData)
        {
            rpcData.Init();

            var rpcDataFromContext = RpcContext.RpcData;
            if (null == rpcDataFromContext) return rpcData;

            //caller_callStack 
            rpcData.caller_callStack_Set(rpcDataFromContext.caller_callStack_Get());

            rpcData.caller_callStack_Push(rpcDataFromContext.caller_rid_Get());

            //http 
            //rpcData.http_Set(rpcDataFromContext.http_Get());

            //user 
            rpcData.user_Set(rpcDataFromContext.user_Get());


            return rpcData;
        }
        #endregion


    }
}
