using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sers.Core.Module.Counter;
using Sers.Core.Module.Env;
using Sers.Hardware.Env;
using Sers.Hardware.Usage;

namespace Sers.Gover.Base.Model
{
    public class ServiceStationData
    {
        public string connKey { get; set; }

        /// <summary>
        /// 服务站点开启时间
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? startTime;

        /// <summary>
        /// 状态：正常、手动关闭、断线
        /// </summary>
        public string status;

        public DeviceInfo deviceInfo;

        public ServiceStationInfo serviceStationInfo;

        public UsageStatus usageStatus;

        public Counter counter;

        public float qps;

        public int apiNodeCount;
        public int activeApiNodeCount;
        public List<string> apiStationNames;

    }
}
