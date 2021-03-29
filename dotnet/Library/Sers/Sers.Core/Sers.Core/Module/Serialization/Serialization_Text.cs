using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Vit.Core.Module.Serialization;
using Vit.Core.Util.ConfigurationManager;

namespace Sers.Core.Module.Serialization
{
    /// <summary>
    ///  https://github.com/dotnet/runtime/tree/main/src/libraries/System.Text.Json
    ///  
    /// System.Text.Json 自定义Converter实现时间转换 https://my.oschina.net/u/4359742/blog/3314243
    /// </summary>
    public class Serialization_Text : ISerialization
    {

        public static readonly Serialization_Text Instance = new Serialization_Text();



        public readonly JsonSerializerOptions options = new JsonSerializerOptions
        {
            //中文不转义 如 {"title":"\u4ee3\u7801\u6539\u53d8\u4e16\u754c"}
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(UnicodeRanges.All),            
            IncludeFields = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };



        public readonly JsonConverter_DateTime jsonConverter_DateTime = new Serialization_Text.JsonConverter_DateTime();

        public Serialization_Text()
        {
            options.Converters.Add(jsonConverter_DateTime);
            options.Converters.Add(JsonConverter_JObject.Instance);
            options.Converters.Add(JsonConverter_JArray.Instance);


            //日期格式化
            var DateTimeFormat = ConfigurationManager.Instance.GetByPath<string>("Vit.Serialization.DateTimeFormat")
                ?? "yyyy-MM-dd HH:mm:ss";
            jsonConverter_DateTime.DateTimeFormat = DateTimeFormat;

        }

        #region JsonConverter
        public class JsonConverter_DateTime : JsonConverter<DateTime>
        {
            public string DateTimeFormat;

            public JsonConverter_DateTime(string dateFormatString = "yyyy-MM-dd HH:mm:ss")
            {
                DateTimeFormat = dateFormatString;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return DateTime.Parse(reader.GetString());
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.ToString(DateTimeFormat));
            }
        }
        #endregion









        #region (x.1)object <--> String

        #region SerializeToString

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string SerializeToString<T>(T value)
        {
            return JsonSerializer.Serialize(value, options);
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string SerializeToString(object value, Type type)
        {
            return JsonSerializer.Serialize(value, type, options);
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
            return JsonSerializer.Deserialize(value, type, options);
        }

        #endregion

        #endregion



        #region  (x.2)object <--> bytes

        #region SerializeToBytes

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] SerializeToBytes<T>(T value)
        {

            return JsonSerializer.SerializeToUtf8Bytes(value, options);
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] SerializeToBytes(object value, Type type)
        {
            return JsonSerializer.SerializeToUtf8Bytes(value, type, options);
        }
        #endregion


        #region DeserializeFromBytes

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T DeserializeFromBytes<T>(byte[] bytes)
        {
            return JsonSerializer.Deserialize<T>(bytes, options);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object DeserializeFromBytes(byte[] bytes, Type type)
        {
            return JsonSerializer.Deserialize(bytes, type, options);
        }
        #endregion

        #endregion




        #region (x.3)DeserializeFromSpan

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T DeserializeFromSpan<T>(ReadOnlySpan<byte> bytes)
        {
            if (bytes.Length == 0) return default;
            return JsonSerializer.Deserialize<T>(bytes, options);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object DeserializeFromSpan(ReadOnlySpan<byte> bytes, Type type)
        {
            if (bytes.Length == 0) return default;
            return JsonSerializer.Deserialize(bytes, type, options);
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

