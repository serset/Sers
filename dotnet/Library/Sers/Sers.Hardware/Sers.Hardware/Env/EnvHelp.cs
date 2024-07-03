using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Sers.Hardware.Env
{
    public class EnvHelp
    {
        #region GetDeviceInfo


        public static DeviceInfo GetDeviceInfo()
        {
            var osPlatform = GetOSPlatform();

            var osInfo = new DeviceInfo
            {
                OSPlatform = "" + osPlatform,
                OSArchitecture = "" + RuntimeInformation.OSArchitecture,
                OSDescription = "" + RuntimeInformation.OSDescription,
                ProcessArchitecture = "" + RuntimeInformation.ProcessArchitecture,
                Is64BitOperatingSystem = Environment.Is64BitOperatingSystem,
                ProcessorCount = Environment.ProcessorCount,
                MachineName = "" + Environment.MachineName,
                OSVersion = "" + Environment.OSVersion,
                WorkingSet = Environment.WorkingSet
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

        #endregion


        #region GetOSPlatform
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
        #endregion


        #region GetMachineUnqueInfo
        public static string GetMachineUnqueInfo()
        {
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    return LinuxHelp.GetMachineUnqueInfo();
                }

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return WindowsHelp.GetMachineUnqueInfo();
                }

                return GetMachineSimpleInfo();
            }
            catch
            {
            }
            return null;

            #region GetMachineSimpleInfo
            string GetMachineSimpleInfo()
            {
                string oriData = "";
                void AddItem(string name, string value)
                {
                    if (!string.IsNullOrWhiteSpace(value)) oriData += name + ":" + value + Environment.NewLine;
                }



                #region (x.1)系统硬件信息  
                AddItem("OSArchitecture", "" + RuntimeInformation.OSArchitecture);
                AddItem("OSDescription", "" + RuntimeInformation.OSDescription);
                AddItem("ProcessArchitecture", "" + RuntimeInformation.ProcessArchitecture);

                AddItem("Is64BitOperatingSystem", "" + Environment.Is64BitOperatingSystem);
                AddItem("ProcessorCount", "" + Environment.ProcessorCount);
                AddItem("MachineName", "" + Environment.MachineName);
                AddItem("OSVersion", "" + Environment.OSVersion);
                #endregion

                return string.IsNullOrWhiteSpace(oriData) ? null : oriData;
            }
            #endregion
        }

        #endregion




        public static string MachineUnqueKey { get; } = GetMachineUnqueKey();

        #region GetMachineUnqueKey
        public static string GetMachineUnqueKey()
        {
            var oriData = GetMachineUnqueInfo();

            if (string.IsNullOrWhiteSpace(oriData)) return null;
            return EnvHelp.MD5(oriData);
        }
        #endregion


        public static string AppUnqueKey { get; } = GetAppUnqueKey();

        #region GetAppUnqueKey
        public static string GetAppUnqueKey()
        {
            var oriData = GetAppUnqueInfo();

            if (string.IsNullOrWhiteSpace(oriData)) return null;
            return EnvHelp.MD5(oriData);
        }
        #endregion

        #region GetAppUnqueInfo
        public static string GetAppUnqueInfo()
        {
            if (MachineUnqueKey == null) return null;


            string oriData = "";
            void AddItem(string name, string value)
            {
                if (!string.IsNullOrWhiteSpace(value)) oriData += name + ":" + value + Environment.NewLine;
            }


            //(x.1)MachineUnqueKey
            AddItem("MachineUnqueKey", MachineUnqueKey);


            #region (x.2) app directory
            //AddItem("BaseDirectory", AppContext.BaseDirectory);
            AddItem("EntryAssembly", Assembly.GetEntryAssembly().Location);
            #endregion


            #region (x.3)EnvironmentVariable
            try
            {
                var vars = Environment.GetEnvironmentVariables();

                var kvs = new List<KeyValuePair<string, string>>();
                foreach (var item in vars.Keys)
                {
                    kvs.Add(new KeyValuePair<string, string>(item.ToString(), vars[item].ToString()));
                }

                kvs.OrderBy(kv => kv.Key).ToList().ForEach(kv =>
                {
                    AddItem("var_" + kv.Key, kv.Value);
                });
            }
            catch { }
            #endregion


            #region (x.4) Linux.cgroup
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                AddItem("cgroup", LinuxHelp.GetCgroupInfo());
            }
            #endregion


            return oriData;
        }
        #endregion


        /// <summary>
        /// MD5加密字符串（32位大写）
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns>加密后的字符串</returns>
        internal static string MD5(string source)
        {
            return Vit.Core.Util.Common.CommonHelp.MD5(source);
        }




    }
}
