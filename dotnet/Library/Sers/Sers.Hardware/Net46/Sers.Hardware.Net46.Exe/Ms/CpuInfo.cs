// v3

using System;
using System.Diagnostics;

using Microsoft.VisualBasic.Devices;

namespace Sers.Hardware.Net46.Exe.Ms
{


    public class CpuInfo : IDisposable
    {

        readonly PerformanceCounter cpu;

        readonly ComputerInfo cinf;

        NetworkInfo network = new NetworkInfo();

        public CpuInfo()
        {
            cpu = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            cpu.NextValue();
            cinf = new ComputerInfo();
        }

        public double GetCpuPercent()
        {
            var percentage = this.cpu.NextValue();
            return Math.Round(percentage, 2, MidpointRounding.AwayFromZero);
        }

        public double GetMemoryPercent()
        {
            var usedMem = this.cinf.TotalPhysicalMemory - this.cinf.AvailablePhysicalMemory;//总内存减去可用内存
            return Math.Round(
                     (double)(usedMem / Convert.ToDecimal(this.cinf.TotalPhysicalMemory) * 100),
                     2,
                     MidpointRounding.AwayFromZero);
        }

        public long[] GetNetworkBps()
        {
            return network.GetBps();
        }

        public double[] GetNetworkMbps()
        {
            return network.GetMbps();
        }


        /// <summary>
        /// [0] CpuPercent
        /// [1] MemoryPercent
        /// [2] received Mbps
        /// [3] sent Mbps
        /// </summary>
        /// <returns></returns>
        public double[] GetInfo()
        {
            var netInfo = network.GetMbps();
            return new double[] {
                 GetCpuPercent(),
                  GetMemoryPercent(),
                  netInfo[0],
                  netInfo[1]
            };
        }



        public void Dispose()
        {
            cpu.Dispose();
            network.Dispose();
        }
    }

}
