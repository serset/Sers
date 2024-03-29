﻿using System.Net;
using System.Net.Sockets;

namespace Vit.Core.Util.Net
{
    public class NetHelp
    {
        #region ParseToIPAddress
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static IPAddress ParseToIPAddress(string host)
        {
            IPAddress ipAddress;
            #region 获取ip地址
            if (!IPAddress.TryParse(host, out ipAddress))
            {
                IPHostEntry hostInfo = Dns.GetHostEntry(host);
                ipAddress = hostInfo.AddressList[0];
            }
            #endregion
            return ipAddress;
        }
        #endregion


        #region TestIsConnectedByPoll
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public bool TestIsConnectedByPoll(Socket socket,int microSeconds = 500)
        {        
            return null != socket && socket.Connected && !(socket.Poll(microSeconds, SelectMode.SelectRead) && (socket.Available == 0));
        }
        #endregion
    }
}
