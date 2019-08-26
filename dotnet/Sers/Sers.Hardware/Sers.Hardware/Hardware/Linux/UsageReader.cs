using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Sers.Core.Util.Hardware;
using TcpTestClient.linux;
using static Sers.Core.Util.Hardware.DeviceManage;

namespace Sers.Hardware.Hardware.Linux
{
    public class UsageReader : IUsageReader
    {
        
        
        public UsageReader()
        {
             
        }
        

        public UsageStatus ReadUsageInfo()
        {
            var info = new UsageStatus();

 
            try
            {               
                info.cpuUsage = ServerConfig.ReadCpuUsage();
            }
            catch { }

            //try
            //{
            //    info.memoryUsage = float.Parse(infos[1].Split(':')[1]);
            //}
            //catch { }

            //try
            //{
            //    info.networkIn = float.Parse(infos[2].Split(':')[1]);
            //}
            //catch { }

            //try
            //{
            //    info.networkOut = float.Parse(infos[3].Split(':')[1]);
            //}
            //catch { }
            return info;
        }

        public void Dispose()
        {
            
        }

       
    }
}
