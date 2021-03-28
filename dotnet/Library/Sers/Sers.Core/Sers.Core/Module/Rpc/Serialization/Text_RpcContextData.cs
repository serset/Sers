using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sers.Core.Module.Rpc.Serialization
{
    public class Text_RpcContextData : JsonConverter<RpcContextData>, IRpcSerialize
    {

        public static readonly Text_RpcContextData Instance = new Text_RpcContextData();


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] SerializeToBytes(RpcContextData data) 
        {
            using (var stream = new MemoryStream())
            using (Utf8JsonWriter writer = new Utf8JsonWriter(stream))
            {
                Instance.Write(writer,data,null);
                writer.Flush();
                return stream.ToArray();
            }     
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RpcContextData DeserializeFromBytes(ArraySegment<byte> data)
        {
            Utf8JsonReader reader = new Utf8JsonReader(data);

            if (!reader.Read())
            {
                return null;
            }

            return Instance.Read(ref reader, null, null);
        }





        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Write(Utf8JsonWriter writer, RpcContextData value, JsonSerializerOptions options)
        {
            if (value==null)
            {
                writer.WriteNullValue();
                return;
            }


            writer.WriteStartObject();

            //(x.1)route         
            writer.WriteString("route",value.route);

            //(x.2)caller
            writer.WritePropertyName("caller");
            writer.WriteStartObject();
            writer.WriteString("rid", value.caller.rid);
            writer.WriteString("source", value.caller.source); 

            if (value.caller.callStack != null)
            {
                writer.WritePropertyName("callStack");
                writer.WriteStartArray(); 
                foreach (var v in value.caller.callStack)
                    writer.WriteStringValue(v);
                writer.WriteEndArray();
            }

            writer.WriteEndObject();

            //(x.3)http
            writer.WritePropertyName("http");
            writer.WriteStartObject();

            writer.WriteString("url", value.http.url);
            writer.WriteString("method", value.http.method);

            if (value.http.statusCode.HasValue)
            {
                writer.WriteNumber("statusCode", value.http.statusCode.Value);
            }

            if (value.http.protocol != null)
            {
                writer.WriteString("protocol", value.http.protocol);            
            }

            if (value.http.headers != null)
            {
                writer.WritePropertyName("headers");
                writer.WriteStartObject();
                foreach (var kv in value.http.headers)
                    writer.WriteString(kv.Key,kv.Value);
                writer.WriteEndObject();                 
            }
            writer.WriteEndObject();

            //(x.4)error          
            if (value.error != null)
            {
                writer.WritePropertyName("error");
                writer.WriteStartObject();               
              
                JsonSerializer.Serialize(writer, value.error, options);

                writer.WriteEndObject();
            }


            //(x.5)user          
            if (value.user != null)
            {
                writer.WritePropertyName("user");
                writer.WriteStartObject();

                JsonSerializer.Serialize(writer, value.error, options);

                writer.WriteEndObject();
            }


            writer.WriteEndObject();

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override RpcContextData Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) return null;

 

            var result = new RpcContextData();
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


                switch (key)
                {

                    case "route":
                        result.route = reader.GetString();
                        break;

                    case "caller":
                        if (reader.TokenType != JsonTokenType.StartObject)
                            throw new ArgumentException("json read error");

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
                            key = reader.GetString();
                            if (!reader.Read())
                            {
                                throw new ArgumentException("json read error");
                            }

                            switch (key)
                            {
                                case "rid":
                                    result.caller.rid = reader.GetString();
                                    break;
                                case "source":
                                    result.caller.source = reader.GetString();
                                    break;
                                //case "callStack":
                                    //if (reader.TokenType != JsonTokenType.StartArray)
                                    //    throw new ArgumentException("json read error");

                                    //arrayCount = reader.ReadArrayHeader();
                                    //if (arrayCount > 0)
                                    //{
                                    //    options.Security.DepthStep(ref reader);
                                    //    var list = new List<string>(arrayCount);
                                    //    while ((arrayCount--) > 0)
                                    //    {
                                    //        list.Add(reader.ReadString());
                                    //    }
                                    //    result.caller.callStack = list;
                                    //    reader.Depth--;
                                    //}
                                    //break;
                                //default:
                                //    reader.Skip(); break;
                            }
                        } 
                        break;

                    case "http":
                        if (reader.TokenType != JsonTokenType.StartObject)
                            throw new ArgumentException("json read error");

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
                            key = reader.GetString();
                            if (!reader.Read())
                            {
                                throw new ArgumentException("json read error");
                            }

                            switch (key)
                            {
                                case "url":
                                    result.http.url = reader.GetString();
                                    break;
                                case "method":
                                    result.http.method = reader.GetString();
                                    break;
                                case "statusCode":
                                    if (reader.TokenType == JsonTokenType.Number)  
                                        result.http.statusCode = reader.GetInt32();
                                    break;
                                case "protocol":
                                    result.http.protocol = reader.GetString();
                                    break;
                                //case "headers":
                                //    arrayCount = reader.ReadMapHeader();
                                //    if (arrayCount > 0)
                                //    {
                                //        options.Security.DepthStep(ref reader);
                                //        var headers = new Dictionary<string, string>(arrayCount);
                                //        while ((arrayCount--) > 0)
                                //        {
                                //            headers[reader.ReadString()] = reader.ReadString();
                                //        }
                                //        result.http.headers = headers;
                                //        reader.Depth--;
                                //    }
                                //    break;                             
                              
                            }
                        }
                        break;


                        //case "error":
                        //    if (!reader.TryReadNil())
                        //    {
                        //        result.error = MessagePackFormatter_Newtonsoft_Object.Instance.Deserialize(ref reader, options);
                        //    }
                        //    break;
                        //case "user":
                        //    if (!reader.TryReadNil())
                        //    {
                        //        result.error = MessagePackFormatter_Newtonsoft_Object.Instance.Deserialize(ref reader, options);
                        //    }
                        //    break;

                }
            }

            return result;

        }
    }
}
