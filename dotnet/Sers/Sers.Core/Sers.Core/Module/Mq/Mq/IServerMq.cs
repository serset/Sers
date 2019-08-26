using System;
using System.Collections.Generic;

namespace Sers.Core.Module.Mq.Mq
{
    public interface IServerMq
    {

        /// <summary>
        /// 请勿处理耗时操作，需立即返回
        /// </summary>
        Action<IMqConn> Conn_OnConnected { get; set; }

        /// <summary>
        /// 请勿处理耗时操作，需立即返回
        /// </summary>
        Action<IMqConn> Conn_OnDisconnected { get; set; }


        /// <summary>
        /// 请勿处理耗时操作，需立即返回。收到数据事件          public delegate void OnGetFrame(IMqConn token, ArraySegment&lt;byte&gt; messageFrame);
        /// </summary>
        Action<IMqConn, ArraySegment<byte>> Conn_OnGetFrame { set; get; }

        IEnumerable<IMqConn> ConnectedList { get; }

        bool Start();
        void Stop();

    }
}
