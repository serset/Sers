using System;
using Vit.Core.Module.Serialization;

namespace Vit.Extensions
{
    public static partial class SpanByteExtensions
    {

        #region SpanByte <--> String
 
        public static string SpanByteToString(this Span<byte> data)
        {             
            return Serialization.Instance.encoding.GetString(data); 
        }


        public static Span<byte> StringToSpanByte(this string data)
        {
            return Serialization.Instance.encoding.GetBytes(data);
        }
        #endregion


        #region SpanByte <--> Int32

        public static Int32 SpanByteToInt32(this Span<byte> data,int startIndex=0)
        {             
            return  BitConverter.ToInt32(data.ToArray(), startIndex);
        }


        public static Span<byte> Int32ToSpanByte(this Int32 data)
        {
            return BitConverter.GetBytes(data);
        }
        #endregion




    }
}
