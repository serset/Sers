using System;
using System.Runtime.CompilerServices;
using System.Text;
using Vit.Core.Module.Serialization;

namespace Sers.Core.Module.Rpc.Serialization
{
    public class StringBuilder_RpcContextData : IRpcSerialize
    {
        public static readonly StringBuilder_RpcContextData Instance = new StringBuilder_RpcContextData();


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] SerializeToBytes(RpcContextData data)
        {
            return Encoding.UTF8.GetBytes(SerializeToString(data));
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RpcContextData DeserializeFromBytes(ArraySegment<byte> data)
        {
            //throw new NotImplementedException();
            return Text_RpcContextData.Instance.DeserializeFromBytes(data);
        }



        [ThreadStatic]
        static StringBuilder buffer;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string SerializeToString(RpcContextData data)
        {
            if (buffer == null)
            {
                buffer = new StringBuilder(102400);
            }
            buffer.Length = 0;

            //(x.1)route
            buffer.Append("{\"route\":\"").Append(data.route);

            //(x.2)caller
            buffer.Append("\",\"caller\":{\"rid\":\"").Append(data.caller.rid);

            if (data.caller.callStack != null)
                buffer.Append("\",\"callStack\":").Append(Serialization_Newtonsoft.Instance.SerializeToString(data.caller.callStack));

            buffer.Append("\",\"source\":").Append(data.caller.source);

            //(x.3)http
            buffer.Append("\"},\"http\":{\"url\":\"").Append(data.http.url)
                .Append("\",\"method\":\"").Append(data.http.method);

            if (data.http.statusCode != null)
                buffer.Append("\",\"statusCode\":").Append(data.http.statusCode.Value);
            if (data.http.protocol != null)
                buffer.Append("\",\"protocol\":").Append(data.http.protocol);
            if (data.http.headers != null)
                buffer.Append("\",\"headers\":").Append(Serialization_Newtonsoft.Instance.SerializeToString(data.http.headers));

            buffer.Append("\"}");

            if (data.error != null)
                buffer.Append(",\"error\":").Append(Serialization_Newtonsoft.Instance.SerializeToString(data.error));

            if (data.user != null)
                buffer.Append(",\"user\":").Append(Serialization_Newtonsoft.Instance.SerializeToString(data.user));



            //end
            buffer.Append("}");


            return buffer.ToString();
        }



       


    }
}
