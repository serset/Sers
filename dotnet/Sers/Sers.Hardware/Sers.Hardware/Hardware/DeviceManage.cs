using Sers.Hardware.Hardware;
using Sers.Hardware.Hardware.Ms;
using System;
using System.Runtime.InteropServices;
using TcpTestClient.Device.linux;


namespace Sers.Core.Util.Hardware
{
    public class DeviceManage
    {
        static OSPlatform GetOSPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return OSPlatform.Linux;
            }            

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return OSPlatform.Windows;
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return OSPlatform.OSX;
            }      
            return default(OSPlatform);
        }

        private static IUsageReader _usageReader;
        private static IUsageReader usageReader => _usageReader??(_usageReader=
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? LinuxHelp.GetUsageReader() :  
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? WindowsHelp.GetUsageReader() :
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




        public static DeviceInfo GetDeviceInfo()
        {
            var osPlatform = GetOSPlatform();

            var osInfo= new DeviceInfo {
                OSPlatform = "" + osPlatform,
                OSArchitecture = "" + RuntimeInformation.OSArchitecture,
                OSDescription = "" + RuntimeInformation.OSDescription,
                ProcessArchitecture = "" + RuntimeInformation.ProcessArchitecture,
                Is64BitOperatingSystem = Environment.Is64BitOperatingSystem,
                ProcessorCount = Environment.ProcessorCount,
                MachineName = "" + Environment.MachineName,
                OSVersion = "" + Environment.OSVersion,
                WorkingSet =   Environment.WorkingSet
            }; 

            if (osPlatform == OSPlatform.Linux)
            {
                osInfo.linux_ServerInfo = LinuxHelp.GetServerInfo();
            }

            return osInfo;



            //Console.WriteLine($"Linux:{RuntimeInformation.IsOSPlatform(OSPlatform.Linux)}");
            //Console.WriteLine($"OSX:{RuntimeInformation.IsOSPlatform(OSPlatform.OSX)}");
            //Console.WriteLine($"Windows:{RuntimeInformation.IsOSPlatform(OSPlatform.Windows)}");

            //Console.WriteLine($"系统架构：{RuntimeInformation.OSArchitecture}");
            //Console.WriteLine($"系统名称：{RuntimeInformation.OSDescription}");
            //Console.WriteLine($"进程架构：{RuntimeInformation.ProcessArchitecture}");
            //Console.WriteLine($"是否64位操作系统：{Environment.Is64BitOperatingSystem}");
            //Console.WriteLine("CPU CORE:" + Environment.ProcessorCount);
            //Console.WriteLine("HostName:" + Environment.MachineName);
            //Console.WriteLine("Version:" + Environment.OSVersion);

            //Console.WriteLine("内存相关的:" + Environment.WorkingSet);


        }


    }
}
