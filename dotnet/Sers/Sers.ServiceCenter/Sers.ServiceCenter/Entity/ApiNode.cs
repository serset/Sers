using Newtonsoft.Json;
using Sers.Core.Extensions;
using Sers.Core.Module.Api;
using Sers.Core.Module.Api.ApiDesc;
using Sers.Core.Module.Api.Message;
using Sers.Core.Module.Log;
using Sers.Core.Module.Rpc;
using Sers.Core.Util.Counter;
using Sers.Core.Util.Extensible;
using Sers.Core.Util.SsError;
using Sers.ServiceCenter.ApiCenter;
using System;
using System.Collections.Generic;
using System.Threading;
using Sers.Core.Module.Api.Data;

namespace Sers.ServiceCenter
{
    /// <summary>
    ///  一个api服务可能在多台服务站点上部署，在某一个服务站点上的api服务就是api节点(ApiNode)
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ApiNode : Extensible
    {
        [JsonProperty]
        public SsApiDesc apiDesc;
  
        public ServiceStation serviceStation;

        /// <summary>
        /// 所在的ApiService
        /// </summary> 
        public ApiService apiService;


        #region counter    
        [JsonIgnore]
        private Counter _counter;
        //[JsonProperty]
        public Counter counter { get => (_counter ?? (_counter = new Counter())); set => _counter = value; }
        #endregion


        public void CallApiAsync(IRpcContextData rpcContextData, ApiMessage reqMessage, Object sender,  Action<object, List<ArraySegment<byte>>> callback)
        {
            //count
            bool success = true;
            counter.Increment(success);
            apiService.counter.Increment(success);
            serviceStation.counter.Increment(success);

            //SendRequest
            serviceStation.SendRequest(sender, reqMessage, callback);

        }


    }
}
