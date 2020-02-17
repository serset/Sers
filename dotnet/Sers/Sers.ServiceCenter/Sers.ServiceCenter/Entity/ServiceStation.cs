using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Sers.Core.CL.MessageOrganize;
using Sers.Core.Module.Counter;
using Sers.Core.Module.Env;
using Sers.Core.Module.Message;
using Sers.Hardware.Env;
using Sers.Hardware.Usage;
using Vit.Core.Util.Extensible;
using Vit.Extensions;

namespace Sers.ServiceCenter.Entity
{
    /// <summary>
    /// 对应一个部署的服务站点
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class ServiceStation: Extensible
    {

        public IOrganizeConnection  connection { get; set; }

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
            var stationNames = apiNodes.Select(m => m.apiDesc.ApiStationNameGet());
            return String.Join(",", stationNames); 
        }

     
        public void SendRequestAsync(Object sender, ApiMessage apiReqMessage, Action<object, List<ArraySegment<byte>>> callback)
        {
            connection.SendRequestAsync(sender, apiReqMessage.Package(), callback);            
        }
    }
}
