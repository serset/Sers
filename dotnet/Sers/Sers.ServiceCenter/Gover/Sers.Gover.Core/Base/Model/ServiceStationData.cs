using Sers.Core.Module.Env;
using Sers.Core.Util.Counter;
using Sers.Hardware.Hardware;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sers.Gover.Core.Model
{
    public class ServiceStationData
    {
        public string mqConnKey;
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
