using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Sers.Core.Module.Api.Message;
using Sers.Core.Module.Env;
using Sers.Core.Util.Counter;
using Sers.Core.Util.Extensible;
using Sers.Hardware.Hardware;

namespace Sers.ServiceCenter
{
    /// <summary>
    /// 对应一个部署的服务站点
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ServiceStation: Extensible
    {

        public string mqConnGuid;

        /// <summary>
        /// 站点软硬件环境信息
        /// </summary>
        [JsonProperty]
        public DeviceInfo deviceInfo;

        /// <summary>
        /// 站点信息。同一硬件可以部署多个站点，它们软硬件环境是一样的，但站点信息不一样。
        /// </summary>
        [JsonProperty]
        public ServiceStationInfo serviceStationInfo;
     
 
        [JsonProperty]
        public List<ApiNode> apiNodes;

        /// <summary>
        /// 系统当前占用率
        /// </summary>
        //[JsonProperty]
        public UsageStatus usageStatus;

        [JsonIgnore]
        public string serviceStationKey=> serviceStationInfo?.serviceStationKey;


        #region counter    
        [JsonIgnore]
        private Counter _counter;
        //[JsonProperty]
        public Counter counter { get => (_counter ?? (_counter = new Counter())); set => _counter = value; }
        #endregion


        public string GetApiStationNames()
        {
            var stationNames = apiNodes.Select(m => m.apiDesc.GetApiStationName());
            return String.Join(",", stationNames); 
        }

        /// <summary>
        /// 函数原型： List<ArraySegment<byte>> SendRequest(string connGuid, ApiRequestMessage apiReqMessage)
        /// </summary>       
        public Func<string, ApiMessage, List<ArraySegment<byte>>> OnSendRequest { get; set; }


        public List<ArraySegment<byte>> SendRequest(ApiMessage apiReqMessage)
        {
            return OnSendRequest(mqConnGuid, apiReqMessage);
        }
    }
}
