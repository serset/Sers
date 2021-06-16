using System;
using Vit.Core.Util.Shell;

namespace Sers.Hardware.Usage
{
    public class LinuxUsageReader : IUsageReader
    {
        
        
        public LinuxUsageReader()
        {
             
        }

        /// <summary>
        /// 读取CPU使用率信息
        /// </summary>
        /// <returns></returns>
        public static float ReadCpuUsage()
        {
            float value = 0f;
            try
            {

                OsShell.Shell("top", "-b -n1", out string cpuInfo);


                var lines = cpuInfo.Split('\n');
                bool flags = false;
                foreach (var item in lines)
                {
                    if (!flags)
                    {
                        if (item.Contains("PID USER"))
                        {
                            flags = true;
                        }
                    }
                    else
                    {
                        var li = item.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < li.Length; i++)
                        {
                            if (li[i] == "R" || li[i] == "S")
                            {
                                value += float.Parse(li[i + 1]);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            var r = (value / 4f);
            if (r > 100) r = 100;
            return r;
        }

        public UsageStatus ReadUsageInfo()
        {
            var info = new UsageStatus();

 
            try
            {               
                info.cpuUsage = ReadCpuUsage();
            }
            catch { }
             
            return info;
        }

        public void Dispose()
        {
            
        }

       
    }
}
