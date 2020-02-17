using System.Runtime.InteropServices;

namespace Sers.Hardware.Usage
{
    public class UsageHelp
    {
        #region 实时硬件利用率


        private static IUsageReader _usageReader;
        private static IUsageReader usageReader => _usageReader ?? (_usageReader =
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? (IUsageReader)new LinuxUsageReader() :
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? (IUsageReader)new WindowsUsageReader() :
             null
            );


        public static UsageStatus GetUsageInfo()
        {
            return usageReader?.ReadUsageInfo();
        }

        public static void Dispose()
        {
            _usageReader?.Dispose();
            _usageReader = null;
        }

        #endregion
    }
}
