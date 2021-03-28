using System;
using System.Runtime.CompilerServices;
using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using Vit.Extensions;

namespace Vit.Core.Module.Serialization
{
    /// <summary>
    /// 参考 https://github.com/neuecc/MessagePack-CSharp
    /// </summary>
    public partial class Serialization_MessagePack : ISerialization
    {

        public static readonly Serialization_MessagePack Instance = new Serialization_MessagePack();



        public MessagePackSerializerOptions options;
        public Serialization_MessagePack()
        {
            options = MessagePack.Resolvers.ContractlessStandardResolver.Options;

            //CompatibleWithNewtonsoft();
        }

        /// <summary>
        /// 适配Newtonsoft 如 JObject JArray类型
        /// </summary>
        public void CompatibleWithNewtonsoft()
        {

            var resolver = MessagePack.Resolvers.CompositeResolver.Create(
              new IMessagePackFormatter[]
              {
                    MessagePackFormatter_JObject.Instance,
                    MessagePackFormatter_JArray.Instance,

                    //NilFormatter.Instance,
                    //new IgnoreFormatter<MethodBase>(),
              },
              new IFormatterResolver[]
              {                 
                    // ContractlessStandardResolver.Instance,
                    // ContractlessStandardResolver.Options.Resolver,
                    options.Resolver
              });

            options = options.WithResolver(resolver);
        }



        #region (x.1)object <--> String

        #region SerializeToString

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string SerializeToString<T>(T value)
        {
            return MessagePackSerializer.ConvertToJson(SerializeToBytes(value), options);
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string SerializeToString(object value, Type type)
        {
            return MessagePackSerializer.ConvertToJson(SerializeToBytes(value), options);
        }

        #endregion

        #region DeserializeFromString

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T DeserializeFromString<T>(string value)
        {
            //throw new NotImplementedException();
            var bytes = MessagePackSerializer.ConvertFromJson(value, options);
            return DeserializeFromBytes<T>(bytes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object DeserializeFromString(string value, Type type)
        {
            //throw new NotImplementedException();

            var bytes = MessagePackSerializer.ConvertFromJson(value, options);
            return DeserializeFromBytes(bytes, type);
        }

        #endregion

        #endregion



        #region (x.2)object <--> bytes

        #region SerializeToBytes


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] SerializeToBytes<T>(T value)
        {
            return MessagePackSerializer.Serialize<T>(value, options);
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] SerializeToBytes(object value, Type type)
        {
            return MessagePackSerializer.Serialize(type, value, options);
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
            return MessagePackSerializer.Deserialize<T>(bytes, options);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object DeserializeFromSpan(ReadOnlyMemory<byte> bytes, Type type)
        {
            return MessagePackSerializer.Deserialize(type, bytes, options);
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



