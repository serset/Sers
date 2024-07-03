using System;

using Sers.Core.CL.MessageDelivery;

using Vit.Core.Module.Log;
using Vit.Extensions.Json_Extensions;

namespace Sers.CL.Zmq.FullDuplex
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


        public void SetIdentity(long identity)
        {
            identity <<= 1;
            identityOfWriter = identity.Int64ToBytes();
            identityOfReader = (++identity).Int64ToBytes();
        }

        internal byte[] identityOfReader { get; set; }
        internal byte[] identityOfWriter { get; set; }


        Action<IDeliveryConnection, ArraySegment<byte>> _OnGetFrame;
        public Action<IDeliveryConnection, ArraySegment<byte>> OnGetFrame
        {
            internal get => _OnGetFrame;
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
        public void SendFrameAsync(Vit.Core.Util.Pipelines.ByteData data)
        {
            var bytes = data.ToBytes();

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
