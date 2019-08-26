using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace TcpTestClient.linux
{
    public class ServerConfig
    {
        //static private log4net.ILog log = log4net.LogManager.GetLogger(typeof(ServerConfig));
        /// <summary>
        /// 获取Linux服务器资源信息
        /// </summary>
        private const string NETWORK_CONFIG_FILE_PATH = @"/etc/NetworkManager/system-connections/";
        private static string logs_service_port = "";// ConfigurationManager.AppSettings["logs_service_port"]; //网口 配置文件中获取 
        /// <summary>
        /// 获取网关 IP信息
        /// </summary>
        /// <returns></returns>
        public static NetworkInfo ReadIpConfig()
        {
            NetworkInfo networkInfo = new NetworkInfo();
            var process = new Process
            {
                StartInfo = new ProcessStartInfo("ifconfig", logs_service_port)
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                }
            };
            process.Start();
            var hddInfo = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            process.Dispose();

            var lines = hddInfo.Split('\n');
            foreach (var item in lines)
            {
                if (item.Contains("inet"))
                {
                    var li = item.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    // inet 192.168.122.1  netmask 255.255.255.0  broadcast 192.168.122.255
                    networkInfo.IPAddress = li[1];
                    networkInfo.SubnetMark = li[3];
                    networkInfo.Gateway = li[5];
                    break;
                }
            }
            return networkInfo;
        }

        /// <summary>
        /// 读取服务器配置
        /// </summary>
        /// <returns></returns>
        public static NetworkInfo ReadNetWorkConfig()
        {
            NetworkInfo networkInfo = new NetworkInfo();
            var config_file = GetNetworkConfigFile();
            try
            {
                if (!string.IsNullOrEmpty(config_file))
                {
                    ConfigFile config = new ConfigFile(config_file);

                    var setting_group = config.SettingGroups;
                    var ip_info = setting_group["ipv4"].Settings;
                    networkInfo.DNS = ip_info["dns"].GetValue();

                    string address_info = ip_info["address1"].GetValue();
                    var address = address_info.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    networkInfo.IPAddress = address[0];

                    var gateway = address[1].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    networkInfo.SubnetMark = NetworkConfig.GetSubnetMarkString(int.Parse(gateway[0]));
                    networkInfo.Gateway = gateway[1];

                    if (setting_group.ContainsKey("ethernet"))
                    {
                        ip_info = setting_group["ethernet"].Settings;
                        if (ip_info.ContainsKey("mac-address"))
                        {
                            networkInfo.MacAddress = ip_info["mac-address"].GetValue();
                        }
                    }
                }
            }
            catch (Exception e)
            {
            }
            return networkInfo;
        }

        /// <summary>
        /// 保存网络设置信息
        /// </summary>
        /// <param name="networkInfo"></param>
        public static void SaveNetworkConfig(NetworkInfo networkInfo)
        {
            var config_file = GetNetworkConfigFile();
            if (!string.IsNullOrEmpty(config_file))
            {
                ConfigFile config = new ConfigFile(config_file);
                var ip_mark_gateway = string.Format("{0}/{1},{2}", networkInfo.IPAddress,
                                    NetworkConfig.GetSubnetMarkString(NetworkConfig.SubnetMarkValue(networkInfo.SubnetMark)),
                                    networkInfo.Gateway);

                var ip_address = config.SettingGroups["ipv4"].Settings;
                ip_address["dns"].SetValue(networkInfo.DNS);
                ip_address["address1"].SetValue(ip_mark_gateway);
                config.Save(config_file);
            }
        }

        /// <summary>
        /// 获取服务器网络配置信息
        /// </summary>
        /// <returns></returns>
        private static string GetNetworkConfigFile()
        {
            var files = Directory.GetFiles(NETWORK_CONFIG_FILE_PATH);
            string config = string.Empty;
            if (files != null && files.Length > 0)
            {
                config = files[0];
            }
            return config;
        }

        /// <summary>
        /// 读取CPU序列号信息
        /// </summary>
        /// <returns></returns>
        public static string ReadCpuSerialNumber()
        {
            try
            {


                const string CPU_FILE_PATH = "/proc/cpuinfo";
                var s = File.ReadAllText(CPU_FILE_PATH);
                var lines = s.Split(new[] { '\n' });
                s = string.Empty;

                foreach (var item in lines)
                {
                    if (item.StartsWith("Serial"))
                    {
                        var temp = item.Split(new[] { ':' });
                        s = temp[1].Trim();
                        break;
                    }
                }
                return s;
            }
            catch (Exception)
            {
            }
            return null;
        }

        /// <summary>
        /// 重启服务器命令
        /// </summary>
        public static void Reboot()
        {
            var process = new Process();
            process.StartInfo = new ProcessStartInfo("reboot");
            process.Start();
            process.WaitForExit();
            process.Dispose();
        }

        /// <summary>
        /// 读取内存信息
        /// </summary>
        /// <returns></returns>
        public static MemInfo ReadMemInfo()
        {
            MemInfo memInfo = new MemInfo();
            const string CPU_FILE_PATH = "/proc/meminfo";
            var mem_file_info = File.ReadAllText(CPU_FILE_PATH);
            var lines = mem_file_info.Split(new[] { '\n' });
            mem_file_info = string.Empty;

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
            return memInfo;
        }

        /// <summary>
        /// 同步系统时间
        /// </summary>
        public static void SyncSystemDatetime()
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    using (var process = new Process())
                    {
                        process.StartInfo = new ProcessStartInfo("ntpdate", "ntp1.aliyun.com");
                        process.Start();
                        process.WaitForExit(50000);
                    }
                }
                catch (Exception e)
                {

                }
            });
        }

        /// <summary>
        /// 读取系统时间
        /// </summary>
        /// <param name="time"></param>
        public static void SetSystemDateTime(DateTime time)
        {
            using (var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo("date", $"-s \"{time.ToString("yyyy-MM-dd HH:mm:ss")}\"");
                process.Start();
                process.WaitForExit(3000);
            }
        }

        /// <summary>
        /// 读取硬盘信息
        /// </summary>
        /// <returns></returns>
        public static HDDInfo ReadHddInfo()
        {
            HDDInfo hdd = null;
            var process = new Process
            {
                StartInfo = new ProcessStartInfo("df", "-h /")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                }
            };
            process.Start();
            var hddInfo = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            process.Dispose();

            var lines = hddInfo.Split('\n');
            foreach (var item in lines)
            {

                if (item.Contains("/dev/sda4") || item.Contains("/dev/mapper/cl-root") || item.Contains("/dev/mapper/centos-root"))
                {
                    var li = item.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < li.Length; i++)
                    {
                        if (li[i].Contains("%"))
                        {
                            hdd = new HDDInfo()
                            {
                                Size = li[i - 3],
                                Used = li[i - 2],
                                Avail = li[i - 1],
                                Usage = li[i]
                            };
                            break;
                        }
                    }
                }
            }
            return hdd;
        }

        /// <summary>
        /// 读取CPU温度信息
        /// </summary>
        /// <returns></returns>
        public static float ReadCpuTemperature()
        {
            const string CPU_Path = "/sys/class/thermal/thermal_zone0/temp";
            var values = File.ReadAllText(CPU_Path);
            float valuef = float.Parse(values);
            return valuef / 1000f;
        }

        /// <summary>
        /// 读取CPU使用率信息
        /// </summary>
        /// <returns></returns>
        public static float ReadCpuUsage()
        {
            float value = 0f;
            var process = new Process
            {
                StartInfo = new ProcessStartInfo("top", "-b -n1")
            };
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            var cpuInfo = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            process.Dispose();

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
            var r =  (value / 4f);
            if (r > 100) r = 100;
            return r;
        }

        /// <summary>
        /// 字符处理
        /// </summary>
        /// <param name="strings"></param>
        /// <returns></returns>
        private static string TrimString(string strings)
        {
            char[] tempchat = new char[strings.Length];
            int n = 0;
            foreach (var item in strings)
            {
                if ((item >= 32 && item <= 126) || item == '\n' || item == ' ')
                {
                    tempchat[n++] = item;
                }
            }
            return new String(tempchat, 0, n);
        }

        /// <summary>
        /// 获取服务器运行时信息
        /// </summary>
        /// <returns></returns>
        public static ServerInfo GetServerInfo()
        {
            var serverInfo = new ServerInfo
            {
                HDDInfo = ReadHddInfo(),
                MemInfo = ReadMemInfo(),
                //NetworkInfo = ReadIpConfig(),
                CpuSerialNumber = ReadCpuSerialNumber()
            };

            //serverInfo.CpuTemperature = ReadCpuTemperature();

            // serverInfo.CpuTemperature = 24;

            serverInfo.CpuUsage = ReadCpuUsage();
            //从rdis取PacketCount、SessionCount
            serverInfo.PacketCount = 0;
            serverInfo.SessionCount = 0;
            return serverInfo;
        }
    }

}
