using Sers.Hardware.Process;
using Sers.Hardware.Usage;

namespace Sers.Core.Module.Env
{
    public class EnvUsageInfo
    {
        public string deviceKey;
        public string serviceStationKey;    

        public UsageStatus usageStatus;

        public ProcessInfo Process;
    }
}
