using System;
using System.Collections.Generic;

namespace Sers.Core.CL.MessageDelivery
{
    public interface IDeliveryConnection
    {

        /// <summary>
        /// 连接状态(0:waitForCertify; 2:certified; 4:waitForClose; 8:closed;)
        /// </summary>
        byte state { get; set; }

        void SendFrameAsync(Vit.Core.Util.Pipelines.ByteData data);
        Action<IDeliveryConnection, ArraySegment<byte>> OnGetFrame { set; }
        void Close();


    }
}
