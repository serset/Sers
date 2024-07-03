using System.Collections.Generic;

using Newtonsoft.Json;

using Sers.Core.Module.Api.ApiDesc;
using Sers.Core.Module.Counter;

namespace Sers.ServiceCenter.Entity
{
    /// <summary>
    /// 对应一个指定路由对应的api服务
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ApiService
    {

        [JsonIgnore]
        public readonly List<ApiNode> apiNodes = new List<ApiNode>();

        public int apiNodeCount => apiNodes.Count;


        [JsonProperty]
        public SsApiDesc apiDesc;

        #region counter    
        [JsonIgnore]
        //private Counter _counter;
        [JsonProperty]
        public Counter counter = new Counter();
        #endregion

        public void AddApiNode(ApiNode apiNode)
        {
            apiNode.apiService = this;

            apiNodes.Insert(0, apiNode);
            //apiNodes.Add(apiNode);

            apiDesc = apiNode.apiDesc;
        }

        public void RemoveApiNode(ApiNode apiNode)
        {
            apiNodes.Remove(apiNode);

            if (apiNode.apiService == this)
                apiNode.apiService = null;
        }



    }
}
