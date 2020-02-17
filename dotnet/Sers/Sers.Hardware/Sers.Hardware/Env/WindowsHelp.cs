using System;
using System.Text.RegularExpressions;
using System.Linq;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Sers.Hardware.Env
{
    public class WindowsHelp
    {    

        #region GetMachineUnqueInfo
        internal static string GetMachineUnqueInfo()
        {
            string oriData = "";
            void AddItem(string name,string value)
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

            #region (x.2)CpuSerialNumber
            AddItem("CpuSerialNumber", GetCpuSerialNumber());          
            #endregion
 


            #region (x.3)网卡mac地址（包含虚拟网卡）
            var macs = new List<string>();

            var arr = GetMacAddrByGetmac();
            if (arr != null && arr.Length > 0)
            {
                macs.AddRange(arr);
            }

            arr = GetMacAddrByIpconfig();
            if (arr != null && arr.Length > 0)
            {
                macs.AddRange(arr);
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
        public static string GetCpuSerialNumber()
        {
            try
            {
                OsShell.Shell("wmic", "CPU get ProcessorID", out string info);
                //ProcessorId
                //BFEBFBFF000306C3

                Regex reg = new Regex(@"\w{16}");
                MatchCollection mc = reg.Matches(info);
                foreach (Match item in mc)
                {
                    return item.Value.ToUpper();
                }               
            }
            catch (System.Exception)
            {               
            }
            return null;
        }
        #endregion


        #region GetMacAddrByIfconfig

        /// <summary>
        /// 获取网卡（包含虚拟网卡） Mac地址(自动排序且字母为大写)。demo:D8-CB-8A-13-14-C4
        /// </summary>
        /// <returns></returns>
        public static string[] GetMacAddrByGetmac()
        {
            /*
             getmac
       物理地址            传输名称
=================== ==========================================================
D8-CB-8A-13-14-C4   \Device\Tcpip_{9591F127-9F16-4DDA-B536-049BFC2F0CB3}
0A-00-27-00-00-16   \Device\Tcpip_{C875985D-0839-47E3-8149-AE34C5F7BC02}
E8-4E-06-43-A9-17   \Device\Tcpip_{2B617931-B9AD-4CE1-8A33-7A94B13C4A6C}
             */
            try
            {
                OsShell.Shell("getmac", null, out string info);

                Regex reg = new Regex(@"(\w\w\-){5}\w\w");
                MatchCollection mc = reg.Matches(info);
                return (from Match item in mc select item.Value.ToUpper()).Distinct().OrderBy(m => m).ToArray();
            }
            catch (Exception)
            {
            }
            return null;
        }

        #endregion

        #region GetMacAddrByIfconfig

        /// <summary>
        /// 获取网卡（包含虚拟网卡） Mac地址(自动排序且字母为大写)。demo:D8-CB-8A-13-14-C4
        /// </summary>
        /// <returns></returns>
        public static string[] GetMacAddrByIpconfig()
        {
            /*
             ipconfig /all

无线局域网适配器 WLAN 2:

   连接特定的 DNS 后缀 . . . . . . . :
   描述. . . . . . . . . . . . . . . : 802.11ac Wireless LAN Card
   物理地址. . . . . . . . . . . . . : E8-4E-06-43-A9-17
   DHCP 已启用 . . . . . . . . . . . : 是
   自动配置已启用. . . . . . . . . . : 是
   本地链接 IPv6 地址. . . . . . . . : fe80::4064:c8f6:16c7:1785%5(首选)
   IPv4 地址 . . . . . . . . . . . . : 192.168.0.92(首选)
   子网掩码  . . . . . . . . . . . . : 255.255.255.0
   获得租约的时间  . . . . . . . . . : 2019年11月22日 11:24:53
   租约过期的时间  . . . . . . . . . : 2019年11月22日 14:04:25
   默认网关. . . . . . . . . . . . . : 192.168.0.1
   DHCP 服务器 . . . . . . . . . . . : 192.168.0.1
   DHCPv6 IAID . . . . . . . . . . . : 585649670
   DHCPv6 客户端 DUID  . . . . . . . : 00-01-00-01-23-7D-4D-82-08-00-27-8C-57-61
   DNS 服务器  . . . . . . . . . . . : 192.168.0.1
                                       0.0.0.0
   TCPIP 上的 NetBIOS  . . . . . . . : 已启用
             */
            try
            {
                OsShell.Shell("ipconfig", "/all", out string info);

                Regex reg = new Regex(@"(\w\w\-){5}\w\w");
                MatchCollection mc = reg.Matches(info);
                return (from Match item in mc select item.Value.ToUpper()).Distinct().OrderBy(m => m).ToArray();
            }
            catch (Exception)
            {
            }
            return null;
        }
        #endregion        

       

    }
}
