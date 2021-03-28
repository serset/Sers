using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Sers.Core.Module.Rpc.Serialization.Fast
{
    public class Bytes_RpcContextData : IRpcSerialize
    {
        public static readonly Bytes_RpcContextData Instance = new Bytes_RpcContextData();


       


        #region SerializeToBytes_Bytes     

        /// <summary>
        /// qps:440万
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] SerializeToBytes(RpcContextData data)
        {
            var fileContent = new byte[1024];
            int t = 0;

            //(x.1)route
            fileContent[t++] = (byte)ERpcPropertyName.route;
            fileContent[t++] = (byte)data.route.Length;
            t += StringToByte(data.route, fileContent.AsSpan(t));

            #region (x.2)caller             
            //(x.x.1)caller_rid
            fileContent[t++] = (byte)ERpcPropertyName.caller_rid;
            fileContent[t++] = (byte)data.caller.rid.Length;
            t += StringToByte(data.caller.rid, fileContent.AsSpan(t));

            //(x.x.2)caller_source
            fileContent[t++] = (byte)ERpcPropertyName.caller_source;
            fileContent[t++] = (byte)data.caller.source.Length;
            t += StringToByte(data.caller.source, fileContent.AsSpan(t));

            #endregion


            #region (x.3)http             
            //(x.x.1)http_url
            fileContent[t++] = (byte)ERpcPropertyName.http_url;
            fileContent[t++] = (byte)data.http.url.Length;
            t += StringToByte(data.http.url, fileContent.AsSpan(t));

            //(x.x.2)http_method
            fileContent[t++] = (byte)ERpcPropertyName.http_method;
            fileContent[t++] = (byte)data.http.method.Length;
            t += StringToByte(data.http.method, fileContent.AsSpan(t));

            #endregion

            return fileContent;

        }
        #endregion


        public RpcContextData DeserializeFromBytes(ArraySegment<byte> data)
        {
            throw new NotImplementedException();
        }







        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static unsafe int StringToByte(string value, Span<byte> bytes)
        {
            fixed (char* chars = value)
            {
                fixed (byte* ptr = bytes)
                {
                    return Encoding.UTF8.GetBytes(chars, value.Length, ptr, bytes.Length);
                }
            }
        }

        

     

      





    }
}
