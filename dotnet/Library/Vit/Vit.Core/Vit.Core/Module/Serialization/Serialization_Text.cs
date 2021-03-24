using System;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Vit.Core.Module.Serialization
{
    public class Serialization_Text : ISerialization
    {

        public static readonly Serialization_Text Instance = new Serialization_Text();


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


        #region (x.1)object <--> String

        #region SerializeToString

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string SerializeToString<T>(T value)
        {  
            return JsonSerializer.Serialize(value, serializeOptions);
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string SerializeToString(object value,Type type)
        {
            return JsonSerializer.Serialize(value, type, serializeOptions);
        }

        #endregion



        #region DeserializeFromString   
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T DeserializeFromString<T>(string value)
        {
            return (T)DeserializeFromString(value, typeof(T));
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object DeserializeFromString(string value, Type type)
        {  
            return JsonSerializer.Deserialize(value, type, deserializeOptions);
        }

        #endregion

        #endregion



        #region  (x.2)object <--> bytes

        #region SerializeToBytes

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] SerializeToBytes<T>(T value)
        {           

            return JsonSerializer.SerializeToUtf8Bytes(value, serializeOptions); 
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] SerializeToBytes(object value,Type type)
        {
            return JsonSerializer.SerializeToUtf8Bytes(value,type, serializeOptions);
        }
        #endregion


        #region DeserializeFromBytes

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T DeserializeFromBytes<T>(byte[] bytes)
        {          
            return JsonSerializer.Deserialize<T>(bytes, deserializeOptions);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object DeserializeFromBytes(byte[] bytes, Type type)
        {
            return JsonSerializer.Deserialize(bytes, type, deserializeOptions);
        }
        #endregion

        #endregion




        #region (x.3)DeserializeFromSpan

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T DeserializeFromSpan<T>(ReadOnlySpan<byte> bytes)
        {
            if (bytes.Length == 0) return default;
            return JsonSerializer.Deserialize<T>(bytes, deserializeOptions);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object DeserializeFromSpan(ReadOnlySpan<byte> bytes, Type type)
        {
            if (bytes.Length == 0) return default;
            return JsonSerializer.Deserialize(bytes, type, deserializeOptions);
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
