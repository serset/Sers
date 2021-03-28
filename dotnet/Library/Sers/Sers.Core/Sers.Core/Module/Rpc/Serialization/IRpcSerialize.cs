using System;
using System.Runtime.CompilerServices;

namespace Sers.Core.Module.Rpc.Serialization
{
    public interface IRpcSerialize
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        byte[] SerializeToBytes(RpcContextData data);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        RpcContextData DeserializeFromBytes(ArraySegment<byte> data);
    }
}
