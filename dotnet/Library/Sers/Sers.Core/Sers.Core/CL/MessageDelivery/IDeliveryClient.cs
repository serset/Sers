using System;

namespace Sers.Core.CL.MessageDelivery
{
    public interface IDeliveryClient
    {

        /// <summary>
        /// 请勿处理耗时操作，需立即返回。收到数据事件
        /// </summary>
        Action<IDeliveryConnection, ArraySegment<byte>> Conn_OnGetFrame { set; }


        Action<IDeliveryConnection> Conn_OnDisconnected { set; }


        IDeliveryConnection conn { get; }


        bool Connect();

        void Close();

    }
}
