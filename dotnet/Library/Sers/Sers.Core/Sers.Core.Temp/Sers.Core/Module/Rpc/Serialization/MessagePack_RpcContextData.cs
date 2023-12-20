using System.Runtime.CompilerServices;
using MessagePack.Formatters;
using MessagePack;
using System.Collections.Generic;
using Vit.Core.Module.Serialization;
using System;

namespace Sers.Core.Module.Rpc.Serialization
{
    public class MessagePack_RpcContextData : IMessagePackFormatter<RpcContextData>, IRpcSerialize
    {

        public static readonly MessagePack_RpcContextData Instance = new MessagePack_RpcContextData();



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] SerializeToBytes(RpcContextData data)
        {
            return Serialization_MessagePack.Instance.SerializeToBytes(data);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RpcContextData DeserializeFromBytes(ArraySegment<byte> data)
        {
            MessagePackReader reader = new MessagePackReader(data);
            
            return Instance.Deserialize(ref reader, MessagePackSerializer.DefaultOptions);
        }






        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Serialize(
          ref MessagePackWriter writer, RpcContextData value, MessagePackSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNil();
                return;
            }
             
    
            //(x.1)route
            writer.WriteMapHeader(
                3
                + (value.error != null ? 1 : 0) 
                + (value.user != null ? 1 : 0)
                );
            writer.Write("route"); writer.Write(value.route);

            //(x.2)caller
            writer.Write("caller");
            writer.WriteMapHeader(
                2
                + (value.caller.callStack != null ? 1 : 0)
                );
            writer.Write("rid"); writer.Write(value.caller.rid);
            writer.Write("source"); writer.Write(value.caller.source);
      
            if (value.caller.callStack != null)
            {
                writer.Write("callStack");
                writer.WriteArrayHeader(value.caller.callStack.Count);
                foreach (var v in value.caller.callStack)
                    writer.Write(v);
            }


            //(x.3)http
            writer.Write("http");
            writer.WriteMapHeader(
                2
                + (value.http.statusCode.HasValue ? 1 : 0)
                + (value.http.protocol != null ? 1 : 0)
                + (value.http.headers != null ? 1 : 0)
                );
            writer.Write("url"); writer.Write(value.http.url);
            writer.Write("method"); writer.Write(value.http.method);
            
            if (value.http.statusCode.HasValue)
            {
                writer.Write("statusCode"); 
                writer.Write(value.http.statusCode.Value); 
            }

            if (value.http.protocol != null) 
            {
                writer.Write("protocol"); writer.Write(value.http.protocol);
            }
            
            if (value.http.headers != null)            
            {
                writer.Write("headers");
                writer.WriteMapHeader(value.http.headers.Count);
                foreach (var kv in value.http.headers)
                {
                    writer.Write(kv.Key);
                    writer.Write(kv.Value);
                }
            }

            //(x.4)error          
            if (value.error != null)
            {
                writer.Write("error");
                //MessagePack_Newtonsoft_Object.Instance.Serialize(ref writer, value.error, options);
                var str = Serialization_Newtonsoft.Instance.Serialize(value.error);
                writer.Write(str);
            }


            //(x.5)user          
            if (value.user != null)
            {
                writer.Write("user");
                //MessagePack_Newtonsoft_Object.Instance.Serialize(ref writer, value.user, options);
                var str = Serialization_Newtonsoft.Instance.Serialize(value.user);
                writer.Write(str);
            }
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RpcContextData Deserialize(
          ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            if (reader.TryReadNil())
            {
                return default;
            }

            var result = new RpcContextData();
            int count = reader.ReadMapHeader();
            if (count == 0) return result;

            options.Security.DepthStep(ref reader);

            int itemCount; int arrayCount;
            for (int i = 0; i < count; i++)
            {
                switch (reader.ReadString())
                {
                    case "route":
                        result.route = reader.ReadString();
                        break;
                  
                    case "caller":
                        itemCount = reader.ReadMapHeader();
                        options.Security.DepthStep(ref reader);

                        for (int t = 0; t < itemCount; t++)
                        {
                            switch (reader.ReadString())
                            {
                                case "rid":
                                    result.caller.rid = reader.ReadString();
                                    break;
                                case "source":
                                    result.caller.source = reader.ReadString();
                                    break;
                                case "callStack":
                                    arrayCount = reader.ReadArrayHeader();
                                    if (arrayCount > 0)
                                    {
                                        options.Security.DepthStep(ref reader);
                                        var list = new List<string>(arrayCount);
                                        while ((arrayCount--) > 0)
                                        {
                                            list.Add(reader.ReadString());
                                        }
                                        result.caller.callStack = list;
                                        reader.Depth--;
                                    }
                                    break;
                                default:
                                    reader.Skip(); break; 
                            }
                        }

                        reader.Depth--;
                        break;

                    case "http":
                        itemCount = reader.ReadMapHeader();
                        options.Security.DepthStep(ref reader);

                        for (int t = 0; t < itemCount; t++)
                        {
                            switch (reader.ReadString())
                            {
                                case "url":
                                    result.http.url = reader.ReadString();
                                    break;
                                case "method":
                                    result.http.method = reader.ReadString();
                                    break;
                                case "statusCode":
                                    if (!reader.TryReadNil())
                                        result.http.statusCode = reader.ReadInt32();
                                    break;
                                case "protocol":
                                    result.http.protocol = reader.ReadString();
                                    break;
                                case "headers":
                                    arrayCount = reader.ReadMapHeader();
                                    if (arrayCount > 0)
                                    {
                                        options.Security.DepthStep(ref reader);
                                        var headers=new Dictionary<string, string>(arrayCount);
                                        while ((arrayCount--) > 0)
                                        {
                                            headers[reader.ReadString()]= reader.ReadString();
                                        }
                                        result.http.headers = headers;
                                        reader.Depth--;
                                    }
                                    break;
                                default:
                                    reader.Skip(); break;
                            }
                        }
                        reader.Depth--;
                        break;
                    case "error":
                        if (!reader.TryReadNil())
                        {
                            //result.error = MessagePack_Newtonsoft_Object.Instance.Deserialize(ref reader, options);
                            result.error = Serialization_Newtonsoft.Instance.Deserialize<object>(reader.ReadString());
                        }
                        break;
                    case "user":
                        if (!reader.TryReadNil())
                        {
                            //result.user = MessagePack_Newtonsoft_Object.Instance.Deserialize(ref reader, options);
                            result.user = Serialization_Newtonsoft.Instance.Deserialize<object>(reader.ReadString());
                        }
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return result;
        }
    }
}
