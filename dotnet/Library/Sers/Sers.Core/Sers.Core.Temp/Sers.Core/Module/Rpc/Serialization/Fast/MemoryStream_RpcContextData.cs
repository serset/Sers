using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace Sers.Core.Module.Rpc.Serialization.Fast
{
    public class MemoryStream_RpcContextData : IRpcSerialize
    {
        public static readonly MemoryStream_RpcContextData Instance = new MemoryStream_RpcContextData();

        public RpcContextData DeserializeFromBytes(ArraySegment<byte> data)
        {
            throw new NotImplementedException();
        }





        #region SerializeToBytes

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] SerializeToBytes(RpcContextData data)
        {
            using (var stream = new MemoryStream(1000))
            {
                byte[] bytes;

                //(x.1)route
                stream.WriteByte((byte)ERpcPropertyName.route);
                stream.WriteByte((byte)data.route.Length);
                bytes = Encoding.UTF8.GetBytes(data.route);
                stream.Write(bytes, 0, bytes.Length);


                #region (x.2)caller             
                //(x.x.1)caller_rid
                stream.WriteByte((byte)ERpcPropertyName.caller_rid);
                stream.WriteByte((byte)data.caller.rid.Length);
                bytes = Encoding.UTF8.GetBytes(data.caller.rid);
                stream.Write(bytes, 0, bytes.Length);

                //(x.x.2)caller_source
                stream.WriteByte((byte)ERpcPropertyName.caller_source);
                stream.WriteByte((byte)data.caller.source.Length);
                bytes = Encoding.UTF8.GetBytes(data.caller.source);
                stream.Write(bytes, 0, bytes.Length);
                #endregion


                #region (x.3)http             
                //(x.x.1)http_url
                stream.WriteByte((byte)ERpcPropertyName.http_url);
                stream.WriteByte((byte)data.http.url.Length);
                bytes = Encoding.UTF8.GetBytes(data.http.url);
                stream.Write(bytes, 0, bytes.Length);

                //(x.x.2)http_method
                stream.WriteByte((byte)ERpcPropertyName.http_method);
                stream.WriteByte((byte)data.http.method.Length);
                bytes = Encoding.UTF8.GetBytes(data.http.method);
                stream.Write(bytes, 0, bytes.Length);
                #endregion

                return stream.ToArray();
            }

        }
        #endregion













    }
}
