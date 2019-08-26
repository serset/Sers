using Sers.Hardware.Hardware;
using Sers.Hardware.Hardware.Linux;
using TcpTestClient.linux;

namespace TcpTestClient.Device.linux
{
    public class LinuxHelp
    {
        public static IUsageReader GetUsageReader()
        {
            return new UsageReader();
        }



        public static TcpTestClient.linux.ServerInfo GetServerInfo()
        {
            return ServerConfig.GetServerInfo();
        }


      

    }
}
