using System.Net;
using System.Net.Sockets;

namespace Vit.Core.Util.Net
{
    public class NetHelp
    {

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static IPAddress ParseToIPAddress(string host)
        {
            if (!IPAddress.TryParse(host, out var ipAddress))
            {
                IPHostEntry hostInfo = Dns.GetHostEntry(host);
                ipAddress = hostInfo.AddressList[0];
            }
            return ipAddress;
        }


        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public bool TestIsConnectedByPoll(Socket socket, int microSeconds = 500)
        {
            return null != socket && socket.Connected && !(socket.Poll(microSeconds, SelectMode.SelectRead) && (socket.Available == 0));
        }

    }
}
