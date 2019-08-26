using System;
using System.Collections.Generic;
using System.Text;

namespace Sers.Mq.Socket
{
    public class ServerMqConfig: MqConnectConfig
    {

        /// <summary>
        /// 服务端 监听端口号
        /// </summary>
        public int port = 10345;

    }
}
