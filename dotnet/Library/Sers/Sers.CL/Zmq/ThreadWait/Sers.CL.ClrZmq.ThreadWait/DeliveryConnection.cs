using System;
using System.Collections.Generic;
using Sers.Core.CL.MessageDelivery;
using Vit.Core.Module.Log;
using Vit.Extensions;

namespace Sers.CL.ClrZmq.ThreadWait
{
    public class DeliveryConnection : IDeliveryConnection
    {       
        ~DeliveryConnection()
        {
            Close();
        }

        public Sers.Core.Util.StreamSecurity.SecurityManager securityManager { set => _securityManager = value; }
        Sers.Core.Util.StreamSecurity.SecurityManager _securityManager;

        /// <summary>
        /// 连接状态(0:waitForCertify; 2:certified; 4:waitForClose; 8:closed;)
        /// </summary>
        public byte state { get; set; } = DeliveryConnState.waitForCertify;


        internal byte[] zmqIdentity { get; set; }


        Action<IDeliveryConnection, ArraySegment<byte>> _OnGetFrame;
        public Action<IDeliveryConnection, ArraySegment<byte>> OnGetFrame
        {
            internal get=> _OnGetFrame;
            set
            {
              
                if (_securityManager != null)
                {
                    value =
                        (conn, data) => { _securityManager.Decryption(data); }
                    + value;
                }
                _OnGetFrame = value;
            }
        }


        public Action<DeliveryConnection, byte[]> OnSendFrameAsync { private get; set; }
        public void SendFrameAsync(List<ArraySegment<byte>> data)
        {
            var bytes = data.ByteDataToBytes();

            _securityManager?.Encryption(bytes.BytesToArraySegmentByte());
         
            OnSendFrameAsync(this, bytes);
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
