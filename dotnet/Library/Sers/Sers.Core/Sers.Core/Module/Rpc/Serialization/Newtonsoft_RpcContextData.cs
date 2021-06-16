using System;
using System.Runtime.CompilerServices;
using Vit.Core.Module.Serialization;

namespace Sers.Core.Module.Rpc.Serialization
{
    public class Newtonsoft_RpcContextData : IRpcSerialize
    {

        public static readonly Newtonsoft_RpcContextData Instance = new Newtonsoft_RpcContextData();


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] SerializeToBytes(RpcContextData data) 
        {
            return Serialization_Newtonsoft.Instance.SerializeToBytes(data);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RpcContextData DeserializeFromBytes(ArraySegment<byte> data)
        {
            return Serialization_Newtonsoft.Instance.DeserializeFromArraySegmentByte<RpcContextData>(data);
        }



 
    }
}
