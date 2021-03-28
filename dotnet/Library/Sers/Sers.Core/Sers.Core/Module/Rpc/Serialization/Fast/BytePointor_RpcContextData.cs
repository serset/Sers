
using System;
using System.Runtime.CompilerServices;
using System.Text;
using Vit.Extensions;

namespace Sers.Core.Module.Rpc.Serialization.Fast
{

    /// <summary>
    /// SerializeToBytes qps:560万
    /// </summary>
    public class BytePointor_RpcContextData : IRpcSerialize
    {
        public static readonly BytePointor_RpcContextData Instance = new BytePointor_RpcContextData();



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
                    bytes[t++] = (byte)ERpcKey.route;
                    bytes[t++] = (byte)data.route.Length;
                    t += StringToBytes(data.route, bytes + t);
                }

                #region (x.2)caller             
                //(x.x.1)caller_rid
                if (data.caller.rid != null)
                {
                    bytes[t++] = (byte)ERpcKey.caller_rid;
                    bytes[t++] = (byte)data.caller.rid.Length;
                    t += StringToBytes(data.caller.rid, bytes + t);
                }

                //(x.x.2)caller_source
                if (data.caller.source != null)
                {
                    bytes[t++] = (byte)ERpcKey.caller_source;
                    bytes[t++] = (byte)data.caller.source.Length;
                    t += StringToBytes(data.caller.source, bytes + t);
                }
                #endregion


                #region (x.3)http             
                //(x.x.1)http_url
                if (data.http.url != null)
                {
                    bytes[t++] = (byte)ERpcKey.http_url;
                    bytes[t++] = (byte)data.http.url.Length;
                    t += StringToBytes(data.http.url, bytes + t);
                }

                //(x.x.2)http_method
                if (data.http.method != null)
                {
                    bytes[t++] = (byte)ERpcKey.http_method;
                    bytes[t++] = (byte)data.http.method.Length;
                    t += StringToBytes(data.http.method, bytes + t);
                }
                #endregion     
            
            }

            return fileContent.Clone(0, t);
        }



        #endregion




        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe RpcContextData DeserializeFromBytes(ArraySegment<byte> data)
        {
            if (data.Count == 0) return null;

            var result = new RpcContextData();

            int t = 0;
            ERpcKey rpcKey;
            fixed (byte* bytes = data.AsSpan())
            {
                while (t < data.Count)
                {
                    rpcKey = (ERpcKey)bytes[t++];
                    int len = (int)bytes[t++];

                    switch (rpcKey)
                    {
                        case ERpcKey.route:
                            result.route = BytesToString(bytes + t, len);
                            break;
                        case ERpcKey.caller_rid:
                            result.caller.rid = BytesToString(bytes + t, len);
                            break;
                        case ERpcKey.caller_source:
                            result.caller.source = BytesToString(bytes + t, len);
                            break;
                        case ERpcKey.http_url:
                            result.http.url = BytesToString(bytes + t, len);
                            break;
                        case ERpcKey.http_method:
                            result.http.method = BytesToString(bytes + t, len);
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
