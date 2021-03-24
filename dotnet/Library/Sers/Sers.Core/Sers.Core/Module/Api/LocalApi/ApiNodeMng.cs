using Sers.Core.Module.Rpc;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using Vit.Extensions;

namespace Sers.Core.Module.Api.LocalApi
{
    /// <summary>
    ///  https://www.xin3721.com/ArticlecSharp/net17449.html
    /// </summary>
    public class ApiNodeMng
    {
        //SortedDictionary

        /// <summary>
        /// 映射  /{httpMethod}{route} -> LocalApiNode
        /// 例如 "/POST/api/value"
        /// </summary>
        protected ImmutableSortedDictionary<string, IApiNode> apiNodeMapWithMethod =  ImmutableSortedDictionary<string, IApiNode>.Empty;

        /// <summary>
        /// 映射  {route} -> LocalApiNode
        /// 例如 "/api/value"
        /// </summary>
        protected ImmutableSortedDictionary<string, IApiNode> apiNodeMapWithoutMethod = ImmutableSortedDictionary<string, IApiNode>.Empty;
        

        public IEnumerable<IApiNode> apiNodes => apiNodeMapWithMethod.Select((kv) => kv.Value).Concat(apiNodeMapWithoutMethod.Select((kv) => kv.Value));

        public void AddApiNode( IEnumerable<IApiNode> apiNodes)
        {
            if (apiNodes == null) return;

            foreach (var apiNode in apiNodes)
            {
                AddApiNode(apiNode);
            }
        }
        public void AddApiNode(IApiNode apiNode)
        {
            if (apiNode == null) return;

            var method = apiNode.apiDesc.HttpMethodGet()?.ToUpper();

            var route = apiNode.apiDesc.route;

            if (string.IsNullOrEmpty(method))
            {
                //apiNodeMapWithoutMethod[route] = apiNode;
                apiNodeMapWithoutMethod = apiNodeMapWithoutMethod.SetItem(route, apiNode);            
            }
            else
            {
                //apiNodeMapWithMethod[method + route] = apiNode;
                apiNodeMapWithMethod = apiNodeMapWithMethod.SetItem(method + route, apiNode);         
            }            
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGet(RpcContextData rpcData, out IApiNode apiNode)
        {
            var route = rpcData.route;
            var method = rpcData.http.method?.ToUpper();
           
            if (!string.IsNullOrEmpty(method))
            {
                if (apiNodeMapWithMethod.TryGetValue(method + route, out apiNode)) return true;
            }           

            return apiNodeMapWithoutMethod.TryGetValue(route, out apiNode);
        }
    }
}
