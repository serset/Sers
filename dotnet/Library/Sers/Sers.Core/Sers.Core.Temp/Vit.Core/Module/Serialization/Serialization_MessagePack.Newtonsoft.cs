using System;
using System.Runtime.CompilerServices;

using MessagePack;
using MessagePack.Formatters;



#region 适配 Newtonsoft

namespace Vit.Core.Module.Serialization
{
    using Newtonsoft.Json.Linq;

    using Vit.Extensions.Newtonsoft_Extensions;

    public partial class Serialization_MessagePack
    {



        #region MessagePackFormatter JObject JArray 


        public class MessagePackFormatter_JObject : IMessagePackFormatter<Newtonsoft.Json.Linq.JObject>
        {

            public static readonly MessagePackFormatter_JObject Instance = new MessagePackFormatter_JObject();

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Serialize(
              ref MessagePackWriter writer, Newtonsoft.Json.Linq.JObject value, MessagePackSerializerOptions options)
            {
                if (value.IsNull())
                {
                    writer.WriteNil();
                    return;
                }

                writer.WriteMapHeader(value.Count);

                foreach (var kv in value)
                {
                    //key
                    writer.Write(kv.Key);


                    //value
                    if (kv.Value.IsNull())
                    {
                        writer.WriteNil();
                        continue;
                    }

                    switch (kv.Value)
                    {
                        case JObject jo:
                            //if (objectFormatter == null) objectFormatter = options.Resolver.GetFormatterWithVerify<JObject>();
                            //objectFormatter?.Serialize(ref writer, jo, options); 
                            Serialize(ref writer, jo, options);
                            break;
                        case JArray ja:
                            //if (arrayFormatter == null) arrayFormatter = options.Resolver.GetFormatterWithVerify<JArray>();
                            //arrayFormatter?.Serialize(ref writer, ja, options);
                            MessagePackFormatter_JArray.Instance.Serialize(ref writer, ja, options);
                            break;

                        case JValue jv:
                            switch (jv.Type)
                            {
                                case JTokenType.String: writer.Write(jv.Value<string>()); break;
                                case JTokenType.Integer: writer.Write(jv.Value<long>()); break;
                                case JTokenType.Float: writer.Write(jv.Value<double>()); break;
                                case JTokenType.Boolean: writer.Write(jv.Value<bool>()); break;
                                case JTokenType.Date: writer.Write(jv.Value<DateTime>().ToString("yyyy-MM-dd HH:mm:ss")); break;
                                default: writer.Write(Serialization_Newtonsoft.Instance.Serialize(jv)); break;
                            }
                            break;
                        default:
                            string str = Serialization_Newtonsoft.Instance.Serialize(kv.Value);
                            writer.Write(str);
                            break;
                    }
                }
            }



            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Newtonsoft.Json.Linq.JObject Deserialize(
              ref MessagePackReader reader, MessagePackSerializerOptions options)
            {
                if (reader.TryReadNil())
                {
                    return default;
                }

                var result = new JObject();
                int count = reader.ReadMapHeader();
                if (count == 0) return result;

                options.Security.DepthStep(ref reader);

                for (int i = 0; i < count; i++)
                {
                    string key = reader.ReadString();

                    switch (reader.NextMessagePackType)
                    {
                        case MessagePackType.Map:
                            result[key] = Deserialize(ref reader, options);

                            break;
                        case MessagePackType.Array:
                            result[key] = MessagePackFormatter_JArray.Instance.Deserialize(ref reader, options);
                            break;
                        case MessagePackType.Integer: result[key] = reader.ReadInt64(); break;
                        case MessagePackType.Boolean: result[key] = reader.ReadBoolean(); break;
                        case MessagePackType.Float: result[key] = reader.ReadDouble(); break;
                        case MessagePackType.String: result[key] = reader.ReadString(); break;
                        case MessagePackType.Nil:
                            result[key] = null;
                            reader.Skip();
                            break;
                        default:
                            result[key] = null;
                            reader.Skip();
                            break;
                    }

                }
                reader.Depth--;

                return result;
            }
        }

        public class MessagePackFormatter_JArray : IMessagePackFormatter<JArray>
        {
            public static readonly MessagePackFormatter_JArray Instance = new MessagePackFormatter_JArray();


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Serialize(
              ref MessagePackWriter writer, Newtonsoft.Json.Linq.JArray value, MessagePackSerializerOptions options)
            {
                if (value.IsNull())
                {
                    writer.WriteNil();
                    return;
                }


                writer.WriteArrayHeader(value.Count);

                foreach (var token in value)
                {
                    if (token.IsNull())
                    {
                        writer.WriteNil();
                        continue;
                    }
                    switch (token)
                    {
                        case JObject jo:
                            MessagePackFormatter_JObject.Instance.Serialize(ref writer, jo, options);
                            break;
                        case JArray ja:
                            Serialize(ref writer, ja, options);
                            break;
                        case JValue jv:
                            switch (jv.Type)
                            {
                                case JTokenType.String: writer.Write(jv.Value<string>()); break;
                                case JTokenType.Integer: writer.Write(jv.Value<long>()); break;
                                case JTokenType.Float: writer.Write(jv.Value<double>()); break;
                                case JTokenType.Boolean: writer.Write(jv.Value<bool>()); break;
                                case JTokenType.Date: writer.Write(jv.Value<DateTime>().ToString("yyyy-MM-dd HH:mm:ss")); break;
                                default: writer.Write(Serialization_Newtonsoft.Instance.Serialize(jv)); break;
                            }
                            break;
                        default:
                            string str = Serialization_Newtonsoft.Instance.Serialize(token);
                            writer.Write(str);
                            break;
                    }

                }
            }


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Newtonsoft.Json.Linq.JArray Deserialize(
              ref MessagePackReader reader, MessagePackSerializerOptions options)
            {
                if (reader.TryReadNil())
                {
                    return default;
                }

                var result = new JArray();
                int count = reader.ReadArrayHeader();
                if (count == 0) return result;

                options.Security.DepthStep(ref reader);


                for (int i = 0; i < count; i++)
                {

                    switch (reader.NextMessagePackType)
                    {
                        case MessagePackType.Map:
                            result.Add(MessagePackFormatter_JObject.Instance.Deserialize(ref reader, options));
                            break;
                        case MessagePackType.Array:
                            result.Add(Deserialize(ref reader, options));
                            break;
                        case MessagePackType.Integer: result.Add(reader.ReadInt64()); break;
                        case MessagePackType.Boolean: result.Add(reader.ReadBoolean()); break;
                        case MessagePackType.Float: result.Add(reader.ReadDouble()); break;
                        case MessagePackType.String: result.Add(reader.ReadString()); break;
                        case MessagePackType.Nil:
                            result.Add(null);
                            reader.Skip();
                            break;
                        default:
                            result.Add(null);
                            reader.Skip();
                            break;
                    }

                }
                reader.Depth--;


                return result;
            }
        }

        #endregion








        #region MessagePackFormatter_NewtonsoftAsString
        public class MessagePackFormatter_NewtonsoftAsString<NewtonsoftType> : IMessagePackFormatter<NewtonsoftType>
        //where NewtonsoftType: Newtonsoft.Json.Linq.JToken
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Serialize(
              ref MessagePackWriter writer, NewtonsoftType value, MessagePackSerializerOptions options)
            {
                if (value == null)
                {
                    writer.WriteNil();
                    return;
                }

                var str = Serialization_Newtonsoft.Instance.Serialize(value);
                writer.Write(str);
            }


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public NewtonsoftType Deserialize(
              ref MessagePackReader reader, MessagePackSerializerOptions options)
            {
                if (reader.TryReadNil())
                {
                    return default;
                }

                options.Security.DepthStep(ref reader);

                var data = reader.ReadString();

                reader.Depth--;

                return Newtonsoft.Json.JsonConvert.DeserializeObject<NewtonsoftType>(data);
            }
        }


        #endregion
    }
}

#endregion
