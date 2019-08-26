using Sers.Core.Extensions;
using System;
using System.Text;

namespace Sers.Core.Module.Serialization
{
    public class Serialization
    {

        public static Serialization Instance { get; } = new Serialization();


        public Encoding encoding { get; private set; } = System.Text.Encoding.UTF8;
        public string charset => encoding.GetCharset();

        public void SetEncoding(EEncoding? type)
        {
            if (null == type) return;

            switch (type.Value)
            {
                case EEncoding.ASCII: encoding = Encoding.ASCII; return;
                case EEncoding.UTF32: encoding = Encoding.UTF32; return;
                case EEncoding.UTF7: encoding = Encoding.UTF7; return;
                case EEncoding.UTF8: encoding = Encoding.UTF8; return;
                case EEncoding.Unicode: encoding = Encoding.Unicode; return;
            }
        }

         



        public string SerializeToString(object obj)
        {
            return obj.Serialize();
        }

       

        /// <summary>
        /// obj 可以为   byte[]、string、 object       
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public byte[] Serialize(object obj)
        {
            if (null == obj) return new byte[0];

            var type = obj.GetType();
            if (type == typeof(byte[]))
            {
                return (byte[])obj;
            }

            string strValue;
            if (type == typeof(string))
            {
                strValue = (string)obj;
            }
            else
            {
                strValue = obj.Serialize();
            }
            return strValue.StringToBytes();
        }
        public T Deserialize<T>(ArraySegment<byte> bytes)
        {
            return (T)Deserialize(bytes, typeof(T));
        }
        public object Deserialize(ArraySegment<byte> bytes, Type type)
        {
            if (type == typeof(byte[]))
            {
                return bytes.ArraySegmentByteToBytes();
            }
            if (type == typeof(ArraySegment<byte>))
            {
                return bytes;
            }
            string strValue = bytes.ArraySegmentByteToString();
            if (type == typeof(string))
            {
                return strValue;
            }
            return strValue.Deserialize(type);
        }

    }
}
