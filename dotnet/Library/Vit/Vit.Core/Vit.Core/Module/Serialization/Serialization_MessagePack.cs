using System;
using System.Runtime.CompilerServices;
using MessagePack;

namespace Vit.Core.Module.Serialization
{
    /// <summary>
    /// 参考 https://github.com/neuecc/MessagePack-CSharp
    /// </summary>
    public class Serialization_MessagePack : ISerialization
    {

        public static readonly Serialization_MessagePack Instance = new Serialization_MessagePack();



        #region (x.1)object <--> String

        #region SerializeToString

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string SerializeToString<T>(T value)
        {
            return MessagePackSerializer.ConvertToJson(SerializeToBytes(value));
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string SerializeToString(object value,Type type)
        {
            return MessagePackSerializer.ConvertToJson(SerializeToBytes(value));
        }

        #endregion

        #region DeserializeFromString

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T DeserializeFromString<T>(string value)
        {
            throw new NotImplementedException(); 
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object DeserializeFromString(string value, Type type)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion



        #region (x.2)object <--> bytes

        #region SerializeToBytes

 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] SerializeToBytes<T>(T value)
        {
            return MessagePackSerializer.Serialize<T>(value,
             MessagePack.Resolvers.ContractlessStandardResolver.Options);          
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] SerializeToBytes(object value,Type type)
        {
            return MessagePackSerializer.Serialize(type,value,
             MessagePack.Resolvers.ContractlessStandardResolver.Options);
        }
        #endregion


        #region DeserializeFromBytes

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T DeserializeFromBytes<T>(byte[] bytes)
        {
            return DeserializeFromSpan<T>(bytes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object DeserializeFromBytes(byte[] bytes, Type type)
        {        
            return DeserializeFromSpan(bytes, type);
        }
        #endregion

        #endregion




        #region (x.3)DeserializeFromSpan

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T DeserializeFromSpan<T>(ReadOnlyMemory<byte> bytes)
        {        
            return MessagePackSerializer.Deserialize<T>(bytes,
             MessagePack.Resolvers.ContractlessStandardResolver.Options);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object DeserializeFromSpan(ReadOnlyMemory<byte> bytes, Type type)
        {    
            return MessagePackSerializer.Deserialize(type, bytes,
             MessagePack.Resolvers.ContractlessStandardResolver.Options);
        }
        #endregion


        #region (x.4)DeserializeFromArraySegmentByte

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T DeserializeFromArraySegmentByte<T>(ArraySegment<byte> bytes)
        {
            return DeserializeFromSpan<T>(bytes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object DeserializeFromArraySegmentByte(ArraySegment<byte> bytes, Type type)
        {
            return DeserializeFromSpan(bytes, type);
        }
        #endregion

    }
}
