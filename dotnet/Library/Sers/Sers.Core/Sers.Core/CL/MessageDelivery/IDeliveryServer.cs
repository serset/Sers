using System;
using System.Collections.Generic;

namespace Sers.Core.CL.MessageDelivery
{
    public interface IDeliveryServer
    {

        /// <summary>
        /// 请勿处理耗时操作，需立即返回
        /// </summary>
        Action<IDeliveryConnection> Conn_OnConnected { set; }

        /// <summary>
        /// 请勿处理耗时操作，需立即返回
        /// </summary>
        Action<IDeliveryConnection> Conn_OnDisconnected { set; } 

        IEnumerable<IDeliveryConnection> ConnectedList { get; }

        bool Start();
        void Stop();

    }
}
