using System;


namespace Sers.Hardware.Usage
{
    public interface IUsageReader : IDisposable
    {
        UsageStatus ReadUsageInfo();
    }
}
