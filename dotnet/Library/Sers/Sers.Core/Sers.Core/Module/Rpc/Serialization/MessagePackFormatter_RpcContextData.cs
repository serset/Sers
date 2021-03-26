using System;
using System.Runtime.CompilerServices;
using MessagePack.Formatters;
using MessagePack;
using Newtonsoft.Json.Linq;
using Vit.Core.Module.Serialization;
using Vit.Extensions;
 

namespace Sers.Core.Module.Rpc.Serialize
{
    public class MessagePackFormatter_RpcContextData : IMessagePackFormatter<RpcContextData>
    {

        public static readonly MessagePackFormatter_Newtonsoft_Object Instance = new MessagePackFormatter_Newtonsoft_Object();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Serialize(
          ref MessagePackWriter writer, RpcContextData value_, MessagePackSerializerOptions options)
        {
            if (value_ == null)
            {
                writer.WriteNil();
                return;
            }

 
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RpcContextData Deserialize(
          ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return default;
            }

   
            return default;
        }
    }
}
