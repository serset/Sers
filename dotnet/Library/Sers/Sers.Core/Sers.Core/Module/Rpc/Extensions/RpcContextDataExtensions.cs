using System;
using Sers.Core.Module.Rpc;
using Vit.Core.Util.Common;
using System.IO;
using System.Runtime.CompilerServices;

namespace Vit.Extensions
{
    /// <summary>
    /// Extension methods for RpcContextData
    /// </summary>
    public static partial class RpcContextDataExtensions
    {



        #region Init

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RpcContextData Init(this RpcContextData rpcData, String callerSource = nameof(ECallerSource.Internal))
        {
            var rid = CommonHelp.NewGuid();

            rpcData.caller.rid = rid;
            rpcData.caller.source = callerSource;

            return rpcData;
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RpcContextData Init(this RpcContextData data, string url, string httpMethod = null)
        {
            data.http.url = "http://sers.internal" + url;

            #region (x.2)设置route
            //去除query string(url ?后面的字符串)           
            {
                //问号的位置
                var queryIndex = url.IndexOf('?');

                // b2?a=c
                if (queryIndex >= 0)
                {
                    data.route = url.Substring(0, queryIndex);
                }
                else
                {
                    data.route = url;
                }
            }
            #endregion              

            //(x.3)设置httpMethod
            if (httpMethod != null) data.http.method = httpMethod;

            return data;
        }

        #endregion

        #region InitFromRpcContext

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RpcContextData InitFromRpcContext(this RpcContextData rpcData)
        {
            rpcData.Init();

            var rpcDataFromContext = RpcContext.RpcData;
            if (null == rpcDataFromContext) return rpcData;


            #region (x.2) caller_callStack
            String parentRid = rpcDataFromContext.caller.rid;
            if (rpcData.caller.callStack == null)
            {
                rpcData.caller.callStack = new System.Collections.Generic.List<string>();
            }
            if (rpcDataFromContext.caller.callStack != null)
            {
                rpcData.caller.callStack.AddRange(rpcDataFromContext.caller.callStack);
            }
            rpcData.caller.callStack.Add(parentRid);

            #endregion


            //(x.3) http
            //rpcData.http=rpcDataFromContext.http;

            //(x.4) user
            rpcData.user = rpcDataFromContext.user;


            return rpcData;
        }
        #endregion




        #region http_url_search_Get
        /// <summary>
        /// 获取泛接口 路由 * 实际传递的内容。
        /// <para>（若 route为"/Station1/fold1/a/*"，url为"http://127.0.0.1/Station1/fold1/a/1/2.html?c=9",则search为"1/2.html?c=9"）</para>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static String http_url_search_Get(this RpcContextData data)
        {
            // "http://127.0.0.1/Station1/fold1/a/1/2.html?c=9"
            var http_url = data.http.url;

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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string http_url_RelativeUrl_Get(this RpcContextData rpcData)
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string http_url_RelativePath_Get(this RpcContextData rpcData)
        {
            String relativePath = http_url_RelativeUrl_Get(rpcData);

            return relativePath?.Replace('/', Path.DirectorySeparatorChar).Replace("..", "");
        }
        #endregion



        #region ContentType
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static String http_header_ContentType_Get(this RpcContextData data)
        {
            return data.http.GetHeader("Content-Type");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RpcContextData http_header_ContentType_Set(this RpcContextData data, String value)
        {
            data.http.Headers()["Content-Type"] = value;
            return data;
        }
        #endregion

        #region ContentType
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static String http_header_Get(this RpcContextData data, string key)
        {
            return data.http.GetHeader(key);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RpcContextData http_header_Set(this RpcContextData data, string key, String value)
        {
            data.http.Headers()[key] = value;
            return data;
        }
        #endregion
        






        #region apiStationName_Get
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string apiStationName_Get(this RpcContextData data)
        {
            var arr = data.route?.Split('/');
            if (arr == null || arr.Length <= 1) return null;
            return arr[1];
        }
        #endregion

        #region caller_callStack

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static String caller_parentRid_Get(this RpcContextData data)
        {
            var ja = data.caller.callStack;
            if (null == ja || ja.Count == 0) return null;
            return ja[ja.Count - 1];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static String caller_rootRid_Get(this RpcContextData data)
        {
            var ja = data.caller.callStack;
            if (null == ja || ja.Count == 0) return null;
            return ja[0];
        }
        #endregion
    }
}
