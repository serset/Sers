using System;
using System.Collections.Generic;
using System.Text;
using static Sers.Core.Util.Hardware.DeviceManage;

namespace Sers.Hardware.Hardware
{
    public interface IUsageReader : IDisposable
    {
        UsageStatus ReadUsageInfo();
    }
}
