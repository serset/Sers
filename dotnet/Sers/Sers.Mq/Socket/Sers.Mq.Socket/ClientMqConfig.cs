using System;
using System.Collections.Generic;
using System.Text;

namespace Sers.Mq.Socket
{
    public class ClientMqConfig: MqConnectConfig
    {
        /// <summary>
        /// 服务端 host地址
        /// </summary>
        public String host = "127.0.0.1";
        /// <summary>
        /// 服务端 监听端口号
        /// </summary>
        public int port = 10345;


    }
}
