using System.Collections.Generic;
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
        /// 状态：正常、手动关闭、断线
        /// </summary>
        public string status;

        public DeviceInfo deviceInfo;

        public ServiceStationInfo serviceStationInfo;

        public UsageStatus usageStatus;

        public Counter counter;

        public int apiNodeCount;
        public int activeApiNodeCount;
        public List<string> apiStationNames;
    }
}
