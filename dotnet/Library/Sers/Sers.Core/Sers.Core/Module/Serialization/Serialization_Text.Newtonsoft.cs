using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;


#region 适配 Newtonsoft

namespace Sers.Core.Module.Serialization
{

    using Newtonsoft.Json.Linq;

    using Vit.Core.Module.Serialization;
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