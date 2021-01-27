using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO;
using Vit.Core.Util.Shell;

namespace Sers.Hardware.Env
{
    public class LinuxHelp
    {

        

        #region GetMachineUnqueInfo
        internal static string GetMachineUnqueInfo()
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


            AddItem("OsVersion", GetOsVersion());           
            #endregion

            #region (x.2)CpuSerialNumber
            AddItem("CpuSerialNumber", GetCpuSerialNumber());
            #endregion

  


            #region (x.4)网卡mac地址（包含虚拟网卡）
            var macs = new List<string>();

            var arr = GetMacAddrByIfconfig();
            if (arr != null && arr.Length > 0)
            {
                macs.AddRange(arr);
            }

            var macInfo = GetMacInfoByFile();
            if (macInfo != null && macInfo.Count > 0)
            {
                macs.AddRange(macInfo.Select(m => m.address));
            }
            macs = macs.Distinct().OrderBy(m => m).ToList();

            AddItem("mac list", String.Join("  ", macs));
            #endregion

            oriData = oriData.Trim();
            if (string.IsNullOrWhiteSpace(oriData)) return null;
            return oriData;
        }
        #endregion
                              
        

        #region GetCpuSerialNumber

      

        /// <summary>
        /// 读取CPU序列号信息(不一定能获取到，而且不一定唯一)
        /// </summary>
        /// <returns></returns>
        public static string GetCpuSerialNumber()
        {
            try
            {
                OsShell.Shell("dmidecode", "-t 4", out string info);
                //ID: C3 06 03 00 FF FB EB BF

                if (info == null) return null;
                foreach (var item in info.Split(new string[] { Environment.NewLine },StringSplitOptions.RemoveEmptyEntries))
                {
                    var line = Regex.Replace(item??"", @"\s", "").ToUpper();
                    if (line.StartsWith("ID"))
                    {
                        return line.Replace("ID", "").Replace(":", "");
                    }
                } 
            }
            catch (Exception ex)
            {    
            }
            return null;
        }
        #endregion



        #region GetOsVersion
        /// <summary>
        /// demo: Linux version 4.13.0-36-generic (buildd@lgw01-amd64-033) (gcc version 5.4.0 20160609 (Ubuntu 5.4.0-6ubuntu1~16.04.9)) #40~16.04.1-Ubuntu SMP Fri Feb 16 23:25:58 UTC 2018
        /// </summary>
        /// <returns></returns>
        public static string GetOsVersion()
        {
            try
            {
                return File.ReadAllText("/proc/version");
            }
            catch (Exception)
            {
            }
            return null;
        }
        #endregion


        #region GetMacAddrByIfconfig

        /// <summary>
        /// 获取网卡（包含虚拟网卡） Mac地址(自动排序且字母为大写)
        /// </summary>
        /// <returns></returns>
        public static string[] GetMacAddrByIfconfig()
        {
            /*
             ifconfig

enp3s0: flags=4163<UP,BROADCAST,RUNNING,MULTICAST>  mtu 1500
        inet 192.168.1.146  netmask 255.255.255.0  broadcast 192.168.1.255
        inet6 fe80::ec80:b112:250d:e75  prefixlen 64  scopeid 0x20<link>
        ether 40:8d:5c:12:00:98  txqueuelen 1000  (Ethernet)
        RX packets 1282303  bytes 757568182 (722.4 MiB)
        RX errors 0  dropped 2249  overruns 0  frame 0
        TX packets 365094  bytes 40872768 (38.9 MiB)
        TX errors 0  dropped 0 overruns 0  carrier 0  collisions 0
        device interrupt 18              
             */
            try
            {
                OsShell.Shell("ifconfig", null, out string info);

                Regex reg = new Regex(@"(\w\w\:){5}\w\w");
                MatchCollection mc = reg.Matches(info);
                return (from Match item in mc select item.Value.ToUpper()).Distinct().OrderBy(m => m).ToArray();
            }
            catch (Exception)
            {
            }
            return null;
        }

        #endregion



        #region GetMacInfoByFile
        /// <summary>
        /// 从文件 /sys/class/net/enp3s0/address 中获取mac地址
        /// </summary>
        public static List<MacInfo> GetMacInfoByFile()
        {
            DirectoryInfo TheFolder = new DirectoryInfo("/sys/class/net");
            if (!TheFolder.Exists)
                return null;

            var list = new List<MacInfo>();
            //遍历文件
            foreach (FileInfo fileInfo in TheFolder.GetFiles())
            {
                try
                {
                    var addressFilePath = Path.Combine(fileInfo.FullName, "address");
                    list.Add(new MacInfo
                    {
                        name = fileInfo.Name,
                        address = File.ReadAllText(addressFilePath).ToUpper()
                    });
                }
                catch (Exception ex)
                {
                }
            }
            return list;
        }
        public class MacInfo
        {
            /// <summary>
            /// 如 02:42:24:90:E5:6E  (字母为大写)
            /// </summary>
            public string address;
            /// <summary>
            /// 如 enp3s0
            /// </summary>
            public string name;
        }
        #endregion



        #region GetMemInfo

        /// <summary>
        /// 读取内存信息
        /// </summary>
        /// <returns></returns>
        public static MemInfo GetMemInfo()
        {
            MemInfo memInfo = new MemInfo();
            try
            {
                var mem_file_info = File.ReadAllText("/proc/meminfo");

                var lines = mem_file_info.Split(new[] { '\n' });


                int count = 0;
                foreach (var item in lines)
                {
                    if (item.StartsWith("MemTotal:"))
                    {
                        count++;
                        var tt = item.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        memInfo.Total = tt[1].Trim();
                    }
                    else if (item.StartsWith("MemAvailable:"))
                    {
                        count++;
                        var tt = item.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        memInfo.Available = tt[1].Trim();
                    }
                    if (count >= 2) break;
                }
            }
            catch (Exception)
            {
            }
            return memInfo;
        }
        #endregion


        #region GetHddInfo


        /// <summary>
        /// 读取硬盘信息
        /// </summary>
        /// <returns></returns>
        public static List<HDDInfo> GetHddInfo()
        {
            var list = new List<HDDInfo>();
            try
            {
                OsShell.Shell("df", "-h /", out string hddInfo);


                var lines = hddInfo.Split('\n');
                foreach (var item in lines)
                {
                    try
                    {


                        if (item.Contains("/dev/"))
                        {
                            var li = item.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            for (int i = 0; i < li.Length; i++)
                            {
                                if (li[i].Contains("%"))
                                {
                                    list.Add(new HDDInfo()
                                    {
                                        Size = li[i - 3],
                                        Used = li[i - 2],
                                        Avail = li[i - 1],
                                        Usage = li[i]
                                    });
                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            catch (Exception)
            {
            }
            return list;
        }
        #endregion


        #region GetCpuTemperature

        /// <summary>
        /// 读取CPU温度信息(单位：摄氏度)
        /// </summary>
        /// <returns></returns>
        public static float GetCpuTemperature()
        {
            try
            {
                var values = File.ReadAllText("/sys/class/thermal/thermal_zone0/temp");
                return float.Parse(values) / 1000f;
            }
            catch (Exception)
            {
            }
            return 0;
        }
        #endregion



        #region GetServerInfo       
        /// <summary>
        /// 获取服务器软硬件信息
        /// </summary>
        /// <returns></returns>
        public static ServerInfo GetServerInfo()
        {
            var serverInfo = new ServerInfo
            {
                HDDInfo = GetHddInfo()?.FirstOrDefault(),
                MemInfo = GetMemInfo(),
                CpuSerialNumber = LinuxHelp.GetCpuSerialNumber(),
                CpuTemperature = GetCpuTemperature()
            };

            return serverInfo;
        }
        #endregion




        #region Model

        public class ServerInfo
        {
            /// <summary>
            /// 内存
            /// </summary>
            public MemInfo MemInfo { get; set; }

            /// <summary>
            ///硬盘信息
            /// </summary>
            public HDDInfo HDDInfo { get; set; }



            /// <summary>
            /// CPU序列号
            /// </summary>
            public string CpuSerialNumber { get; set; }

            /// <summary>
            /// CPU温度
            /// </summary>
            public float CpuTemperature { get; set; }
        }







        public class MemInfo
        {
            /// <summary>
            /// 总计内存大小
            /// </summary>
            public string Total { get; set; }
            /// <summary>
            /// 可用内存大小
            /// </summary>
            public string Available { get; set; }
        }




        public class HDDInfo
        {
            /// <summary>
            /// 硬盘大小
            /// </summary>
            public string Size { get; set; }

            /// <summary>
            /// 已使用大小
            /// </summary>
            public string Used { get; set; }

            /// <summary>
            /// 可用大小
            /// </summary>
            public string Avail { get; set; }

            /// <summary>
            /// 使用率
            /// </summary>
            public string Usage { get; set; }
        }
        #endregion
    }
}
