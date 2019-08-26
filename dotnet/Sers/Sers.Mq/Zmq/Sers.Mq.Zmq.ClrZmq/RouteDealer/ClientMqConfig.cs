using System;
using System.Collections.Generic;
using System.Text;

namespace Sers.Mq.Zmq.ClrZmq.RouteDealer
{
    public class ClientMqConfig
    {
        /// <summary>
        /// 服务端 host地址
        /// </summary>
        public string host = "127.0.0.1";
        /// <summary>
        /// 服务端 监听端口号
        /// </summary>
        public int port = 5555;


        //(x.1)workThread
        /// <summary>
        /// 后台处理消息的线程个数（单位个，默认16）
        /// </summary>
        public int workThreadCount = 16;


        //(x.2)ping
        /// <summary>
        /// 心跳测试超时时间（单位ms，默认2000）
        /// </summary>
        public int pingTimeout = 2000;
        /// <summary>
        /// 心跳测试失败重试次数（单位次，默认3）
        /// </summary>
        public int pingRetryCount = 3;
        /// <summary>
        /// 心跳测试时间间隔（单位ms，默认1000）
        /// </summary>
        public int pingInterval = 1000;

        //(x.3)request
        /// <summary>
        /// 请求超时时间（单位ms，默认300000）
        /// </summary>
        public int requestTimeout = 300000;
    }
}
