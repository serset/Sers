
using Vit.Extensions;
using System;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Vit.Core.Module.Serialization
{
    public class Serialization_Text : Serialization
    {

        public new static readonly Serialization_Text Instance = new Serialization_Text();

        public JsonSerializerOptions serializeOptions = new JsonSerializerOptions
        {
            IncludeFields = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        public JsonSerializerOptions deserializeOptions = new JsonSerializerOptions
        {
            IncludeFields = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };


        #region (x.2)object <--> String

        #region SerializeToString

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string SerializeToString<T>(T value)
        {  
            return JsonSerializer.Serialize(value, serializeOptions);
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string SerializeToString(object value,Type type)
        {
            return JsonSerializer.Serialize(value, type, serializeOptions);
        }

        #endregion

        #region DeserializeFromString

   

        /// <summary>
        /// 使用Newtonsoft反序列化。T也可为值类型（例如 int?、bool） 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override object DeserializeFromString(string value, Type type)
        {
            if (null == value || null == type) return null;

            if (type.TypeIsValueTypeOrStringType())
            {
                return DeserializeStruct(value, type);
            }
            return JsonSerializer.Deserialize(value, type, deserializeOptions);
        }

        #endregion

        #endregion



        #region (x.3)object <--> bytes

        #region SerializeToBytes

        /// <summary>
        /// obj 可以为   byte[]、string、 object       
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override byte[] SerializeToBytes<T>(T value)
        {
            if (null == value) return new byte[0];
            
            if (value is byte[] bytes)
            {
                return bytes;
            }

            return JsonSerializer.SerializeToUtf8Bytes(value, serializeOptions); 
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override byte[] SerializeToBytes(object value,Type type)
        {
            if (null == value) return new byte[0];

            if (value is byte[] bytes)
            {
                return bytes;
            }

            return JsonSerializer.SerializeToUtf8Bytes(value,type, serializeOptions);
        }
        #endregion


        #region DeserializeFromBytes

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override T DeserializeFromBytes<T>(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return default;
            return JsonSerializer.Deserialize<T>(bytes, deserializeOptions);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override object DeserializeFromBytes(byte[] bytes, Type type)
        {
            if (bytes == null || bytes.Length == 0) return default;
            //if (type == typeof(byte[]))
            //{
            //    return bytes;
            //}
            //if (type == typeof(ArraySegment<byte>))
            //{
            //    return bytes.BytesToArraySegmentByte();
            //}
            return JsonSerializer.Deserialize(bytes,type, deserializeOptions);
        }
        #endregion

        #endregion




        #region DeserializeFromSpan

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override T DeserializeFromSpan<T>(ReadOnlySpan<byte> bytes)
        {
            if (bytes.Length == 0) return default;
            return JsonSerializer.Deserialize<T>(bytes, deserializeOptions);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override object DeserializeFromSpan(ReadOnlySpan<byte> bytes, Type type)
        {
            if (bytes.Length == 0) return default;

            return JsonSerializer.Deserialize(bytes, type, deserializeOptions);
        }
        #endregion


        #region (x.4)object <--> ArraySegmentByte

        #region SerializeToArraySegmentByte
        /// <summary>
        /// obj 可以为   byte[]、string、 object       
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ArraySegment<byte> SerializeToArraySegmentByte(object obj)
        {
            if (null == obj) return ArraySegmentByteExtensions.Null;

            if (obj is ArraySegment<byte> asbs)
            {
                return asbs;
            }
            if (obj is byte[] bytes)
            {
                return bytes.BytesToArraySegmentByte();
            }
            
            return SerializeToBytes(obj).BytesToArraySegmentByte();
        }
        #endregion

        #region DeserializeFromArraySegmentByte

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T DeserializeFromArraySegmentByte<T>(ArraySegment<byte> bytes)
        {
            if (bytes.Count == 0) return default;

            return JsonSerializer.Deserialize<T>(bytes, deserializeOptions);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object DeserializeFromArraySegmentByte(ArraySegment<byte> bytes, Type type)
        {
            if (bytes.Count == 0) return default;

            //if (type == typeof(byte[]))
            //{
            //    return bytes.ArraySegmentByteToBytes();
            //}
            //if (type == typeof(ArraySegment<byte>))
            //{
            //    return bytes;
            //}
            return JsonSerializer.Deserialize(bytes, type, deserializeOptions);
        }
        #endregion

        #endregion


 

    }
}
