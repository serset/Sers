using System;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

using Sers.CL.Socket.Iocp.Base;

using Vit.Core.Module.Log;
using Vit.Extensions;
using Vit.Extensions.Json_Extensions;

namespace Sers.CL.Socket.Iocp.Mode.Simple
{
    public class DeliveryConnection : DeliveryConnection_Base
    {


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void SendFrameAsync(Vit.Core.Util.Pipelines.ByteData data)
        {
            if (data == null || socket == null) return;
            try
            {
                Int32 len = data.Count();
                data.Insert(0, len.Int32ToArraySegmentByte());

                var bytes = data.ToBytes();
                _securityManager?.Encryption(new ArraySegment<byte>(bytes, 4, bytes.Length - 4));
                socket.SendAsync(bytes.BytesToArraySegmentByte(), SocketFlags.None);
                //socket.SendAsync(data, SocketFlags.None);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Close();
            }
        }
    }
}
