using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

using Vit.Core.Module.Serialization;
using Vit.Core.Util.ConfigurationManager;
using Vit.Extensions;
using Vit.Extensions.Json_Extensions;

namespace Sers.Core.Module.Serialization.Text
{
    /// <summary>
    ///  https://github.com/dotnet/runtime/tree/main/src/libraries/System.Text.Json
    ///  
    /// System.Text.Json convert DateTime https://my.oschina.net/u/4359742/blog/3314243
    /// </summary>
    public partial class Serialization_Text : ISerialization
    {

        public static readonly Serialization_Text Instance = new Serialization_Text();



        public readonly JsonSerializerOptions options = new JsonSerializerOptions
        {
            // Do not escape Chinese , for example {"title":"\u4ee3\u7801\u6539\u53d8\u4e16\u754c"}
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(UnicodeRanges.All),
            IncludeFields = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };



        public readonly JsonConverter_DateTime jsonConverter_DateTime;

        public Serialization_Text()
        {
            options.AddConverter_Newtonsoft();

            // serialize enum to string not int
            options.Converters.Add(new JsonStringEnumConverter());

            // format DateTime
            var DateTimeFormat = Appsettings.json.GetByPath<string>("Vit.Serialization.DateTimeFormat") ?? "yyyy-MM-dd HH:mm:ss";

            jsonConverter_DateTime = options.AddConverter_DateTime(DateTimeFormat);
        }





        #region #1 object <--> String

        #region Serialize

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string Serialize(object value, Type type = null)
        {
            if (value == default) return null;
            return JsonSerializer.Serialize(value, type ?? value?.GetType(), options);
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
            if (string.IsNullOrWhiteSpace(value)) return type.DefaultValue();
            return JsonSerializer.Deserialize(value, type, options);
        }

        #endregion

        #endregion



        #region  #2 object <--> bytes

        #region SerializeToBytes

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] SerializeToBytes(object value, Type type = null)
        {
            if (value == default) return null;
            return JsonSerializer.SerializeToUtf8Bytes(value, type ?? value?.GetType(), options);
        }
        #endregion


        #region DeserializeFromBytes

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T DeserializeFromBytes<T>(byte[] bytes)
        {
            if (bytes == null) return (T)typeof(T).DefaultValue();
            return JsonSerializer.Deserialize<T>(bytes, options);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object DeserializeFromBytes(byte[] bytes, Type type)
        {
            if (bytes == null) return type.DefaultValue();
            return JsonSerializer.Deserialize(bytes, type, options);
        }
        #endregion

        #endregion




        #region #3 DeserializeFromSpan

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T DeserializeFromSpan<T>(ReadOnlySpan<byte> bytes)
        {
            if (bytes.Length == 0) return (T)typeof(T).DefaultValue();
            return JsonSerializer.Deserialize<T>(bytes, options);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object DeserializeFromSpan(ReadOnlySpan<byte> bytes, Type type)
        {
            if (bytes.Length == 0) return type.DefaultValue();
            return JsonSerializer.Deserialize(bytes, type, options);
        }
        #endregion


        #region #4 DeserializeFromArraySegmentByte

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

