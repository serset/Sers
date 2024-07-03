using System;
using System.Collections.Generic;
using System.Linq;

using Sers.Core.Module.Rpc;

namespace Vit.Extensions
{
    /// <summary>
    /// Extension methods for RpcContextData
    /// </summary>
    public static partial class RpcContextDataExtensions_Cookie
    {

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static string Cookie_Get(this RpcContextData rpcData, string cookieKey)
        {
            // #1
            var cookie = rpcData.http_header_Get("Cookie");
            if (string.IsNullOrEmpty(cookie)) return null;


            // #2
            // "a=b;c=7"
            string str = cookie;
            char entrySeparator = ';';
            char kvSeparator = '=';
            Dictionary<string, string> dictionary;
            {
                //dictionary = str.Split(new string[] { entrySeparator }, StringSplitOptions.RemoveEmptyEntries)
                //    .GroupBy(x => x.Split(new string[] { kvSeparator }, StringSplitOptions.None)[0], x => x.Split(new string[] { kvSeparator }, StringSplitOptions.None)[1])
                //    .ToDictionary(x => x.Key, x => x.First());
                dictionary = str.Split(new[] { entrySeparator }, StringSplitOptions.RemoveEmptyEntries)
                   //.GroupBy(x => x.Split(new string[] { kvSeparator }, StringSplitOptions.None)[0], x => x.Split(new string[] { kvSeparator }, StringSplitOptions.None)[1])
                   .Select(x => x.Split(new[] { kvSeparator })).ToDictionary(kv => kv[0]?.Trim(), kv => kv[1]?.Trim());
            }
            return dictionary.TryGetValue(cookieKey, out var v) ? v : null;

        }



    }
}
