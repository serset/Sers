using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Diagnostics;

namespace Sers.Hardware.Net46
{
    internal class NetworkInfo : IDisposable
    {
        List<PerformanceCounter> received = new List<PerformanceCounter>();
        List<PerformanceCounter> sent = new List<PerformanceCounter>();

        public NetworkInfo()
        {

            string[] names = getAdapter();
            foreach (string name in names)
            {
                try
                {
                    PerformanceCounter pc = new PerformanceCounter("Network Interface", "Bytes Received/sec", name.Replace('(', '[').Replace(')', ']'), ".");                   
                    pc.NextValue();
                    received.Add(pc);

                    PerformanceCounter pc2 = new PerformanceCounter("Network Interface", "Bytes Sent/sec", name.Replace('(', '[').Replace(')', ']'), ".");
                    sent.Add(pc2);
                }
                catch
                {
                }
            }
        }
        ~NetworkInfo()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (null != received)
            {
                foreach (var item in received)
                {
                    item.Dispose();
                }
                received = null;
            }

            if (null != sent)
            {
                foreach (var item in sent)
                {
                    item.Dispose();
                }
                sent = null;
            }
        }

        /// <summary>
        /// [0] received bps
        /// [1] sent bps
        /// </summary>
        /// <returns></returns>
        public long[] GetBps()
        {
            long bpsRecv = 0;
            long bpsSent = 0;
            foreach (PerformanceCounter pc in received)
            {
                bpsRecv += Convert.ToInt32(pc.NextValue()) / 1000;
            }
            foreach (PerformanceCounter pc in sent)
            {
                bpsSent += Convert.ToInt32(pc.NextValue()) / 1000;
            }

            return new []{ bpsRecv, bpsSent };
        }

        /// <summary>
        /// [0] received Mbps
        /// [1] sent Mbps
        /// </summary>
        /// <returns></returns>
        public double[] GetMbps()
        {
            var bps = GetBps();

            return new double[] { bps[0]/1024.0f, bps[1] / 1024.0f };

        }

        string[] getAdapter()
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            string[] name = new string[adapters.Length];
            int index = 0;
            foreach (NetworkInterface ni in adapters)
            {
                name[index] = ni.Description;
                index++;
            }
            return name;
        }
    }
}

 