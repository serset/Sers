using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

using Vit.Core.Util.ConfigurationManager;

namespace Vit.Core.Module.Serialization
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



#region 适配 Newtonsoft

namespace Vit.Core.Module.Serialization
{
   
    using Newtonsoft.Json.Linq;
    using Vit.Extensions;



    #region JsonConverter_JObject
    class JsonConverter_JObject : JsonConverter<JObject>
    {
        public static readonly JsonConverter_JObject Instance = new JsonConverter_JObject();



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Write(Utf8JsonWriter writer, JObject value, JsonSerializerOptions options)
        {
            if (value.IsNull())
            {
                writer.WriteNullValue();
                return;
            }


            writer.WriteStartObject();

            foreach (var kv in value)
            {
                if (kv.Value.IsNull())
                {
                    continue;
                }


                writer.WritePropertyName(kv.Key);

                switch (kv.Value)
                {
                    case JObject jo:                      
                        Write(writer, jo, options);
                        break;
                    case JArray ja:                      
                        JsonConverter_JArray.Instance.Write(writer, ja, options);
                        break;

                    case JValue jv:
                        switch (jv.Type)
                        {
                            case JTokenType.String: writer.WriteStringValue(jv.Value<string>()); break;
                            case JTokenType.Integer: writer.WriteNumberValue(jv.Value<long>()); break;
                            case JTokenType.Float: writer.WriteNumberValue(jv.Value<double>()); break;
                            case JTokenType.Boolean: writer.WriteBooleanValue(jv.Value<bool>()); break;
                            case JTokenType.Date: writer.WriteStringValue(jv.Value<DateTime>().ToString("yyyy-MM-dd HH:mm:ss")); break;

                            default: writer.WriteStringValue(Serialization_Newtonsoft.Instance.SerializeToString(jv)); break;
                        }
                        break;
                    default:
                        string str = Serialization_Newtonsoft.Instance.SerializeToString(kv.Value);
                        writer.WriteStringValue(str);
                        break;
                }
            }


            writer.WriteEndObject();

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override JObject Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) return null;
           

            var result = new JObject();

            while (true)
            {
                if (!reader.Read())
                {
                    throw new ArgumentException("json read error");
                }

                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    reader.Skip();
                    break;
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new ArgumentException("json read error");
                }

                string key = reader.GetString();

                if (!reader.Read())
                {
                    throw new ArgumentException("json read error");
                }


                switch (reader.TokenType)
                {
                    case JsonTokenType.StartObject:
                        result[key] = Read(ref reader, typeof(JObject), options);
                        break;
                    case JsonTokenType.StartArray:
                        result[key] = JsonConverter_JArray.Instance.Read(ref reader, typeof(JArray), options);
                        break;
                    case JsonTokenType.String: result[key] = reader.GetString(); break;
                    case JsonTokenType.Number:
                        if (reader.TryGetInt32(out var int32))
                        {
                            result[key] = int32;
                            break;
                        }
                        if (reader.TryGetInt64(out var int64))
                        {
                            result[key] = int64;
                            break;
                        }
                        if (reader.TryGetDouble(out var d))
                        {
                            result[key] = d;
                            break;
                        }

                        if (reader.TryGetSingle(out var sing))
                        {
                            result[key] = sing;
                            break;
                        }
                        if (reader.TryGetDecimal(out var dec))
                        {
                            result[key] = dec;
                            break;
                        }
                        break;
                    case JsonTokenType.False: result[key] = false; break;
                    case JsonTokenType.True: result[key] = true; break;
                        //case JsonTokenType.Null: break;
                        //case JsonTokenType.None: break;
                        //default: break;
                }
            }

            return result;

        }


    }
    #endregion


    #region JsonConverter_JArray
    class JsonConverter_JArray : JsonConverter<JArray>
    {
        public static readonly JsonConverter_JArray Instance = new JsonConverter_JArray();



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Write(Utf8JsonWriter writer, JArray value, JsonSerializerOptions options)
        {
            if (value.IsNull())
            {
                writer.WriteNullValue();
                return;
            }


            writer.WriteStartArray();

            foreach (var token in value)
            {
                if (token.IsNull())
                {
                    writer.WriteNullValue();
                    continue;
                }

                switch (token)
                {

                    case JObject jo:
                        JsonConverter_JObject.Instance.Write(writer, jo, options);
                        break;
                    case JArray ja:
                        Write(writer, ja, options);
                        break;
                    case JValue jv:
                        switch (jv.Type)
                        {
                            case JTokenType.String: writer.WriteStringValue(jv.Value<string>()); break;
                            case JTokenType.Integer: writer.WriteNumberValue(jv.Value<long>()); break;
                            case JTokenType.Float: writer.WriteNumberValue(jv.Value<double>()); break;
                            case JTokenType.Boolean: writer.WriteBooleanValue(jv.Value<bool>()); break;
                            case JTokenType.Date: writer.WriteStringValue(jv.Value<DateTime>().ToString("yyyy-MM-dd HH:mm:ss")); break;

                            default: writer.WriteStringValue(Serialization_Newtonsoft.Instance.SerializeToString(jv)); break;
                        }
                        break;
                    default:
                        string str = Serialization_Newtonsoft.Instance.SerializeToString(token);
                        writer.WriteStringValue(str);
                        break;
                }
            }


            writer.WriteEndArray();

        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override JArray Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray) return null;


            var result = new JArray();

            while (true)
            {
                if (!reader.Read())
                {
                    throw new ArgumentException("json read error");
                }

                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    reader.Skip();
                    break;
                }              


                switch (reader.TokenType)
                {
                    case JsonTokenType.StartObject:
                        result.Add(JsonConverter_JObject.Instance.Read(ref reader, typeof(JObject), options));
                        break;
                    case JsonTokenType.StartArray:
                        result.Add(Read(ref reader, typeof(JArray), options));
                        break;
                    case JsonTokenType.String: result.Add(reader.GetString()); break;
                    case JsonTokenType.Number:
                        if (reader.TryGetInt32(out var int32))
                        {
                            result.Add(int32);
                            break;
                        }
                        if (reader.TryGetInt64(out var int64))
                        {
                            result.Add(int64);
                            break;
                        }
                        if (reader.TryGetDouble(out var d))
                        {
                            result.Add(d);
                            break;
                        }

                        if (reader.TryGetSingle(out var sing))
                        {
                            result.Add(sing);
                            break;
                        }
                        if (reader.TryGetDecimal(out var dec))
                        {
                            result.Add(dec);
                            break;
                        }
                        result.Add(null);
                        break;
                    case JsonTokenType.False: result.Add(false); break;
                    case JsonTokenType.True: result.Add(true); break;
                    //case JsonTokenType.Null: break;
                    //case JsonTokenType.None: break;
                    default: result.Add(null); break;
                }
            }

            return result;

        }


    }
    #endregion
}

#endregion