using System;
using System.Runtime.CompilerServices;
using MessagePack.Formatters;
using MessagePack;
using Newtonsoft.Json.Linq;
using Vit.Core.Module.Serialization;
using Vit.Extensions;

using static Vit.Core.Module.Serialization.Serialization_MessagePack;
 

namespace Sers.Core.Module.Rpc.Serialize
{
    public class MessagePackFormatter_Newtonsoft_Object : IMessagePackFormatter<object>
    {

        public static readonly MessagePackFormatter_Newtonsoft_Object Instance = new MessagePackFormatter_Newtonsoft_Object();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Serialize(
          ref MessagePackWriter writer, object value_, MessagePackSerializerOptions options)
        {
            if (value_ == null)
            {
                writer.WriteNil();
                return;
            }


            Newtonsoft.Json.Linq.JObject value = value_ as JObject;
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



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Deserialize(
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
                for (int i = 0; i < count; i++)
                {
                    string key = reader.ReadString();

                    switch (reader.NextMessagePackType)
                    {
                        case MessagePackType.Map:
                            result[key] = Deserialize(ref reader, options) as JObject;

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
            }
            finally
            {
                reader.Depth--;
            }

            return result;
        }
    }
}
