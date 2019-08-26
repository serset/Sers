using System;

namespace Sers.Core.Module.Mq.Mq
{
    public interface IClientMq
    {

        /// <summary>
        /// 连接秘钥，用以验证连接安全性。服务端和客户端必须一致
        /// </summary>
        string secretKey { get; set; }

        /// <summary>
        /// 请勿处理耗时操作，需立即返回。收到数据事件          public delegate void OnGetFrame(IMqConn token, ArraySegment&lt;byte&gt; messageFrame);
        /// </summary>
        Action<IMqConn, ArraySegment<byte>> OnGetFrame { set; get; }


        Action<IMqConn> Conn_OnDisconnected { set; get; }


        IMqConn mqConn { get; }


        bool Connect();

        void Close();

    }
}
