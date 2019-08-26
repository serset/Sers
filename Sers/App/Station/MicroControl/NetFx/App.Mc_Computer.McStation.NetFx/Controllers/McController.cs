using Sers.Core.Module.Api.Data;
using Sers.Core.Module.Api.Rpc;
using Sers.Core.Module.ApiDesc.Attribute;
using Sers.Core.Module.SsApiDiscovery;
using Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute;
using Sers.Hardware.Hardware.Os;
using System;
using System.Net;
using System.Net.Sockets;

namespace App.Mc_Computer.McStation.Controllers
{
    public class McController : IApiController
    {
        /// <summary>
        /// Shell
        /// </summary>
        /// <returns></returns>
        [SsRoute("Shell")]
        [SsCallerSource(ECallerSource.Internal)]
        public ApiReturn Shell(string fileName, string arguments)
        {
            OsShell.Shell(fileName, arguments);
            return true;
        }

        /// <summary>
        /// ShellWithReturn
        /// </summary>
        /// <returns></returns>
        [SsRoute("ShellWithReturn")]
        [SsCallerSource(ECallerSource.Internal)]
        public ApiReturn<string> ShellWithReturn(string fileName, string arguments)
        {
            OsShell.Shell(fileName, arguments, out var ret);
            return ret;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="mac">Mac地址。如 "010203040506" (6 bytes)</param>
        /// <param name="port"></param>
        /// <returns></returns>
        [SsRoute("WakeupOnLan")]
        [SsCallerSource(ECallerSource.Internal)]
        public ApiReturn<string> WakeUp(string ip, string mac, int port = 77)
        {
            if (port <= 0) port = 77;
            WakeupOnLan(ip, mac, port);

            return "已发送唤醒指令";
        }

        #region WakeupOnLan
        public static void WakeupOnLan(string ip, string mac, int port = 77)
        {
            #region 发送唤醒魔术包(UDP)
            {

                using (UdpClient sender = new UdpClient())
                {

                    IPAddress GroupAddress = IPAddress.Parse(ip);
                    IPEndPoint groupEP = new IPEndPoint(GroupAddress, port);

                    string data = "";
                    #region 构建唤醒指令
                    /*
                     你可以在任何协议的数据包（如在TCP/IP、IPX包）中填上 "FFFFFFFFFFFF"+连续重复16次的MAC地址，
                     就可利用该协议作出一个使用该协议的MagicPacket。只要NIC检测到数据包中任何地方有这样的片段,便会将计算机唤醒。                     
                     */
                    data = "FFFFFFFFFFFF";

                    for (int i = 0; i < 16; i++)
                    {
                        data += mac;
                    }
                    #endregion
                    byte[] bytes = strToToHexByte(data);

                    sender.Send(bytes, bytes.Length, groupEP);

                    sender.Close();
                }

                #region function strToToHexByte
                byte[] strToToHexByte(string hexString)
                {
                    hexString = hexString.Replace(" ", "");
                    if ((hexString.Length % 2) != 0)
                        hexString += "0";
                    byte[] returnBytes = new byte[hexString.Length / 2];
                    for (int i = 0; i < returnBytes.Length; i++)
                        returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
                    return returnBytes;
                }
                #endregion
            }
            #endregion
        }
        #endregion
    }
}
