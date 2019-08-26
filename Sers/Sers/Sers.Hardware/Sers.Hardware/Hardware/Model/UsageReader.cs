using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Sers.Core.Util.Hardware;
using static Sers.Core.Util.Hardware.DeviceManage;

namespace Sers.Hardware.Hardware.Windows
{
    public class UsageReader : IUsageReader
    {
        Process process;

        public void Start()
        {
            Dispose();
            process = new Process
            {
                StartInfo = new ProcessStartInfo("Sers.Hardware.Net46.Exe.exe")
                {
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    UseShellExecute = false
                }
            };

            process.Start();
        }

        public UsageReader()
        {
            Start();
        }
        ~UsageReader()
        {
            Dispose();
        }


        public UsageStatus ReadUsageInfo()
        {
            // Cpu:2.8,Memory:34.72,NetworkIn:0,NetworkOut:0
            string usageInfo=null;

          
            lock (this)
            {                
                process.StandardInput.WriteLine(" ");
                var res = process.StandardOutput.ReadLineAsync();
                if (res.Wait(1000))
                {
                    usageInfo = res.Result;                    
                }

                //失败重启
                if (string.IsNullOrWhiteSpace(usageInfo))
                {
                    Start();
                    return null;
                }
            }

            var info = new UsageStatus();
            var infos = usageInfo.Split(',');
            try
            {
                info.cpuUsage = float.Parse(infos[0].Split(':')[1]);
            }
            catch { }

            try
            {
                info.memoryUsage = float.Parse(infos[1].Split(':')[1]);
            }
            catch { }

            try
            {
                info.networkIn = float.Parse(infos[2].Split(':')[1]);
            }
            catch { }

            try
            {
                info.networkOut = float.Parse(infos[3].Split(':')[1]);
            }
            catch { }
            return info;

        }

        public void Dispose()
        {
            if (null != process)
            {
                try
                {
                    process.StandardInput.WriteLine("exit");
                }
                catch { }

                try
                {
                    process.Kill(); 
                }
                catch { }

                try
                {                   
                    process.Dispose();
                }
                catch { }


                process = null;
            }
        }


    }
}
