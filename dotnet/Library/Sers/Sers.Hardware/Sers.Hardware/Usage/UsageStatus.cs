namespace Sers.Hardware.Usage
{
    public class UsageStatus
    {
        public double cpuUsage;
        public double memoryUsage;
        public double networkIn;
        public double networkOut;

        public void CopyFrom(UsageStatus ori) 
        {
            if (ori == null) return;

            cpuUsage = ori.cpuUsage;
            memoryUsage = ori.memoryUsage;
            networkIn = ori.networkIn;
            networkOut = ori.networkOut;
        }
    }
}
