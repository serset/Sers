using System;
using System.Runtime.CompilerServices;
using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using Newtonsoft.Json.Linq;
using Vit.Extensions;

namespace Vit.Core.Module.Serialization
{
    /// <summary>
    /// 参考 https://github.com/neuecc/MessagePack-CSharp
    /// </summary>
    public class Serialization_MessagePack : ISerialization
    {

        public static readonly Serialization_MessagePack Instance = new Serialization_MessagePack();



        protected MessagePackSerializerOptions options;
        public Serialization_MessagePack()
        {
 
            var resolver = MessagePack.Resolvers.CompositeResolver.Create(
                new IMessagePackFormatter[]
                {  

                    NewtonsoftFormatter_JObject.Instance,
                    NewtonsoftFormatter_JArray.Instance,

                    //NilFormatter.Instance,
                    //new IgnoreFormatter<MethodBase>(),
                
                 },
                new IFormatterResolver[]
                {                 
                     //ContractlessStandardResolver.Options.Resolver,
                     ContractlessStandardResolver.Instance
                });

            options = MessagePack.Resolvers.ContractlessStandardResolver.Options.WithResolver(resolver);
        }



        #region (x.1)object <--> String

        #region SerializeToString

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string SerializeToString<T>(T value)
        {
            return MessagePackSerializer.ConvertToJson(SerializeToBytes(value), options);
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string SerializeToString(object value,Type type)
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
        public byte[] SerializeToBytes(object value,Type type)
        {
            return MessagePackSerializer.Serialize(type,value, options);
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







        #region NewtonsoftFormatter_SaveAsString
        private class NewtonsoftFormatter_SaveAsString<NewtonsoftType> : IMessagePackFormatter<NewtonsoftType>
            //where NewtonsoftType: Newtonsoft.Json.Linq.JToken
        {
            public void Serialize(
              ref MessagePackWriter writer, NewtonsoftType value, MessagePackSerializerOptions options)
            {
                if (value == null)
                {
                    writer.WriteNil();
                    return;
                }

                var str = Serialization_Newtonsoft.Instance.SerializeToString(value);
                writer.Write(str);
            }

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


        #region NewtonsoftFormatter JObject JArray 


        private class NewtonsoftFormatter_JObject  : IMessagePackFormatter<Newtonsoft.Json.Linq.JObject>         
        {

            public static readonly NewtonsoftFormatter_JObject Instance = new NewtonsoftFormatter_JObject();

            public void Serialize(
              ref MessagePackWriter writer, Newtonsoft.Json.Linq.JObject value, MessagePackSerializerOptions options)
            {
                if (value == null)
                {
                    writer.WriteNil();
                    return;
                }


                writer.WriteMapHeader(value.Count);
             

                //IMessagePackFormatter<JObject> objectFormatter = null;
                //IMessagePackFormatter<JArray> arrayFormatter = null;

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
                            NewtonsoftFormatter_JArray.Instance.Serialize(ref writer, ja, options);
                            break;
                     
                        case JValue jv:
                            switch (jv.Type) 
                            {
                                case JTokenType.String: writer.Write(jv.Value<string>()); break;
                                case JTokenType.Integer: writer.Write(jv.Value<long>()); break;
                                case JTokenType.Float: writer.Write(jv.Value<double>()); break;
                                case JTokenType.Boolean: writer.Write(jv.Value<bool>()); break;
                                case JTokenType.Date: writer.Write(jv.Value<DateTime>().ToString("yyyy-MM-dd HH:mm:ss")); break;
                                default: writer.Write(Serialization_Newtonsoft.Instance.SerializeToString(jv)); break;
                            }
                            break;
                        default:
                            string str = Serialization_Newtonsoft.Instance.SerializeToString(kv.Value);
                            writer.Write(str); 
                            break;
                    }
                } 
            }

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
                try
                {
                    //IMessagePackFormatter<JObject> objectFormatter = null;
                    //IMessagePackFormatter<JArray> arrayFormatter = null;

                    for (int i = 0; i < count; i++)
                    {
                        string key = reader.ReadString();

                        switch (reader.NextMessagePackType)
                        {
                            case MessagePackType.Map:
                                //if (objectFormatter == null) objectFormatter = options.Resolver.GetFormatterWithVerify<JObject>();
                                //result[key] = objectFormatter?.Deserialize(ref reader, options);
                                result[key] = Deserialize(ref reader, options);

                                break;
                            case MessagePackType.Array:
                                //if (arrayFormatter == null) arrayFormatter = options.Resolver.GetFormatterWithVerify<JArray>();
                                //result[key] = arrayFormatter?.Deserialize(ref reader, options);
                                result[key] = NewtonsoftFormatter_JArray.Instance.Deserialize(ref reader, options);
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
                }
                finally
                {
                    reader.Depth--;
                }


                return result;
            }
        }

        private class NewtonsoftFormatter_JArray : IMessagePackFormatter<Newtonsoft.Json.Linq.JArray>
        {
            public static readonly NewtonsoftFormatter_JArray Instance = new NewtonsoftFormatter_JArray();


            public void Serialize(
              ref MessagePackWriter writer, Newtonsoft.Json.Linq.JArray value, MessagePackSerializerOptions options)
            {
                if (value == null)
                {
                    writer.WriteNil();
                    return;
                }


                writer.WriteArrayHeader(value.Count);


                //IMessagePackFormatter<JObject> objectFormatter = null;
                //IMessagePackFormatter<JArray> arrayFormatter = null;

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
                            //if (objectFormatter == null) objectFormatter = options.Resolver.GetFormatterWithVerify<JObject>();
                            //objectFormatter?.Serialize(ref writer, jo, options);
                            NewtonsoftFormatter_JObject.Instance.Serialize(ref writer, jo, options);
                            break;
                        case JArray ja:
                            //if (arrayFormatter == null) arrayFormatter = options.Resolver.GetFormatterWithVerify<JArray>();
                            //arrayFormatter?.Serialize(ref writer, ja, options);
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
                                default: writer.Write(Serialization_Newtonsoft.Instance.SerializeToString(jv)); break;
                            }
                           break;
                        default:
                            string str = Serialization_Newtonsoft.Instance.SerializeToString(token);
                            writer.Write(str);
                            break;
                    }

                }
            }

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
                try
                {
                    //IMessagePackFormatter<JObject> objectFormatter = null;
                    //IMessagePackFormatter<JArray> arrayFormatter = null;

                    for (int i = 0; i < count; i++)
                    {                      

                        switch (reader.NextMessagePackType)
                        {
                            case MessagePackType.Map:
                                //if (objectFormatter == null) objectFormatter = options.Resolver.GetFormatterWithVerify<JObject>();
                                //result.Add( objectFormatter?.Deserialize(ref reader, options));
                                result.Add(NewtonsoftFormatter_JObject.Instance.Deserialize(ref reader, options));
                                break;
                            case MessagePackType.Array:
                                //if (arrayFormatter == null) arrayFormatter = options.Resolver.GetFormatterWithVerify<JArray>();
                                //result.Add(arrayFormatter?.Deserialize(ref reader, options));
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
                }
                finally
                {
                    reader.Depth--;
                }


                return result;
            }
        }

        #endregion

 

    }
}
