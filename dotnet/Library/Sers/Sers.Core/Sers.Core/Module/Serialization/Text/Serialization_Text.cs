using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Unicode;

using Vit.Core.Module.Serialization;
using Vit.Core.Util.ConfigurationManager;
using Vit.Extensions;

namespace Sers.Core.Module.Serialization.Text
{
    /// <summary>
    ///  https://github.com/dotnet/runtime/tree/main/src/libraries/System.Text.Json
    ///  
    /// System.Text.Json 自定义Converter实现时间转换 https://my.oschina.net/u/4359742/blog/3314243
    /// </summary>
    public partial class Serialization_Text : ISerialization
    {

        public static readonly Serialization_Text Instance = new Serialization_Text();



        public readonly JsonSerializerOptions options = new JsonSerializerOptions
        {
            //中文不转义 如 {"title":"\u4ee3\u7801\u6539\u53d8\u4e16\u754c"}
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(UnicodeRanges.All),
            IncludeFields = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };



        public readonly JsonConverter_DateTime jsonConverter_DateTime;

        public Serialization_Text()
        {
            options.AddConverter_Newtonsoft();


            //日期格式化
            var DateTimeFormat = Appsettings.json.GetByPath<string>("Vit.Serialization.DateTimeFormat")
              ?? "yyyy-MM-dd HH:mm:ss";

            jsonConverter_DateTime = options.AddConverter_DateTime(DateTimeFormat);

        }










        #region (x.1)object <--> String

        #region Serialize

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string Serialize<T>(T value)
        {
            return JsonSerializer.Serialize(value, options);
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string Serialize(object value, Type type)
        {
            return JsonSerializer.Serialize(value, type, options);
        }

        #endregion



        #region Deserialize 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Deserialize<T>(string value)
        {
            return (T)Deserialize(value, typeof(T));
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Deserialize(string value, Type type)
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

