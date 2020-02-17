using System;
using System.Collections.Generic;
using Sers.Core.CL.MessageDelivery;
using Vit.Core.Module.Log;
using Vit.Extensions;

namespace Sers.CL.Zmq.FullDuplex
{
    public class DeliveryConnection : IDeliveryConnection
    {       
        ~DeliveryConnection()
        {
            Close();
        }

        /// <summary>
        /// 连接状态(0:waitForCertify; 2:certified; 4:waitForClose; 8:closed;)
        /// </summary>
        public byte state { get; set; } = DeliveryConnState.waitForCertify;


        public void SetIdentity(long identity)
        {
            identity <<= 1;
            identityOfWriter = identity.Int64ToBytes();
            identityOfReader = (++ identity ).Int64ToBytes();
        }

        internal byte[] identityOfReader { get; set; }
        internal byte[] identityOfWriter { get; set; }


        public Action<IDeliveryConnection, ArraySegment<byte>> OnGetFrame { internal get; set; }


        public Action<DeliveryConnection, List<ArraySegment<byte>>> OnSendFrameAsync { private get; set; }
        public void SendFrameAsync(List<ArraySegment<byte>> data)
        {
            OnSendFrameAsync(this,data);
        }
        

        public Action<IDeliveryConnection> Conn_OnDisconnected { get; set; }

        public void Close()
        {             
            state = DeliveryConnState.closed;             
            try
            {
                Conn_OnDisconnected?.Invoke(this);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}
