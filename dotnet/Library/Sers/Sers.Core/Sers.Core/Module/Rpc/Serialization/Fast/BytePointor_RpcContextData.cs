
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Vit.Core.Module.Serialization;
using Vit.Extensions;

namespace Sers.Core.Module.Rpc.Serialization.Fast
{

    /// <summary>
    /// SerializeToBytes qps:560万
    /// </summary>
    public class BytePointor_RpcContextData : IRpcSerialize
    {
        public static readonly BytePointor_RpcContextData Instance = new BytePointor_RpcContextData();



        private static ISerialization Serialization = Serialization_Text.Instance;


        #region SerializeToBytes  

        [ThreadStatic]
        static byte[] fileContent;
 


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe byte[] SerializeToBytes(RpcContextData data)
        {
            if (fileContent == null) fileContent = new byte[102400];
            int t = 0;
            fixed (byte* bytes = fileContent)
            { 

                //(x.1)route
                if (data.route != null)
                {
                    AppendStringValue(bytes, ref t, ERpcPropertyName.route, data.route);               
                }

                #region (x.2)caller             
                //(x.x.1)caller_rid
                if (data.caller.rid != null)
                {
                    AppendStringValue(bytes, ref t, ERpcPropertyName.caller_rid, data.caller.rid);
                }

                //(x.x.2)caller_callStack
                if (data.caller.callStack != null)
                {
                    AppendStringValue(bytes, ref t, ERpcPropertyName.caller_callStack, Serialization.SerializeToString(data.caller.callStack));
                }

                //(x.x.3)caller_source
                if (data.caller.source != null)
                {
                    AppendStringValue(bytes, ref t, ERpcPropertyName.caller_source, data.caller.source);
                }
                #endregion


                #region (x.3)http             
                //(x.x.1)http_url
                if (data.http.url != null)
                {
                    AppendStringValue(bytes, ref t, ERpcPropertyName.http_url, data.http.url);
                }

                //(x.x.2)http_method
                if (data.http.method != null)
                {
                    AppendStringValue(bytes, ref t, ERpcPropertyName.http_method, data.http.method);
                }

                //(x.x.3)http_statusCode
                if (data.http.statusCode != null)
                {
                    bytes[t] = (byte)ERpcPropertyName.http_statusCode;
                    ((short*)(bytes + t + 1))[0] = (short)4;
                    ((int*)(bytes + t + 3))[0] = data.http.statusCode.Value;
                    t += 7;
                }

                //(x.x.4)http_protocol
                if (data.http.protocol != null)
                {
                    AppendStringValue(bytes, ref t, ERpcPropertyName.http_protocol, data.http.protocol);
                }

                //(x.x.5)http_headers
                if (data.http.headers != null)
                {
                    AppendStringValue(bytes, ref t, ERpcPropertyName.http_headers, Serialization.SerializeToString(data.http.headers));
                }

                #endregion


                //(x.4)error
                if (data.error != null)
                {
                    AppendStringValue(bytes, ref t, ERpcPropertyName.error, Serialization.SerializeToString(data.error));
                }

                //(x.5)user
                if (data.user != null)
                {
                    AppendStringValue(bytes, ref t, ERpcPropertyName.user, Serialization.SerializeToString(data.user));
                }

            }

            return fileContent.Clone(0, t);
        }



        #endregion


        #region AppendStringValue
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        unsafe void AppendStringValue(byte* bytes, ref int t, ERpcPropertyName property, string value)
        {
            bytes[t] = (byte)property;

            int valueByteCount = StringToBytes(value, bytes + t + 3);

            ((short*)(bytes + t + 1))[0] = (short)valueByteCount;

            t += valueByteCount + 3;
        }

        #endregion





        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe RpcContextData DeserializeFromBytes(ArraySegment<byte> data)
        {
            if (data.Count == 0) return null;

            var result = new RpcContextData();

            int t = 0;
            ERpcPropertyName rpcKey;
            fixed (byte* bytes = data.AsSpan())
            {
                while (t < data.Count)
                {
                    rpcKey = (ERpcPropertyName)bytes[t];
                    short len = ((short*)(bytes+t+1))[0];
                    t += 3;
                    switch (rpcKey)
                    {
                        //(x.1)
                        case ERpcPropertyName.route:
                            result.route = BytesToString(bytes + t, len);
                            break;

                        //(x.2)
                        case ERpcPropertyName.caller_rid:
                            result.caller.rid = BytesToString(bytes + t, len);
                            break;                     
                        case ERpcPropertyName.caller_callStack:
                            result.caller.callStack = Serialization.DeserializeFromArraySegmentByte<List<string>>(data.Slice(t, len));
                            break;
                        case ERpcPropertyName.caller_source:
                            result.caller.source = BytesToString(bytes + t, len);
                            break;

                        //(x.3)
                        case ERpcPropertyName.http_url:
                            result.http.url = BytesToString(bytes + t, len);
                            break;
                        case ERpcPropertyName.http_method:
                            result.http.method = BytesToString(bytes + t, len);
                            break;
                        case ERpcPropertyName.http_statusCode:
                            result.http.statusCode = ((int*)(bytes + t))[0];
                            break;
                        case ERpcPropertyName.http_protocol:
                            result.http.protocol = BytesToString(bytes + t, len);
                            break;
                        case ERpcPropertyName.http_headers:
                            result.http.headers = Serialization.DeserializeFromArraySegmentByte<Dictionary<string, string>>(data.Slice(t, len));
                            break;

                        //(x.4)
                        case ERpcPropertyName.error:
                            result.error = Serialization.DeserializeFromArraySegmentByte<JObject>(data.Slice(t, len));
                            break;

                        //(x.5)
                        case ERpcPropertyName.user:
                            result.user = Serialization.DeserializeFromArraySegmentByte<JObject>(data.Slice(t, len));
                            break;
                    }
                    t += len;
                }
            }
            return result;
        }





        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static unsafe int StringToBytes(string value, byte* ptr, int maxCount = 10240)
        {
            fixed (char* chars = value)
            {
                return Encoding.UTF8.GetBytes(chars, value.Length, ptr, maxCount);
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static unsafe string BytesToString(byte* ptr, int count)
        {
            return Encoding.UTF8.GetString(ptr, count);
        }












    }
}
