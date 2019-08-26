using System;
using Sers.Core.Module.Serialization;

namespace Sers.Core.Extensions
{
    public static partial class ReadOnlySpanByteExtensions
    {

        #region ReadOnlySpanByte <--> String
  
        public static string ReadOnlySpanByteToString(this ReadOnlySpan<byte> data)
        {             
            return Serialization.Instance.encoding.GetString(data); 
        }


        public static ReadOnlySpan<byte> StringToReadOnlySpanByte(this string data)
        {
            return Serialization.Instance.encoding.GetBytes(data);
        }
        #endregion


        #region ReadOnlySpanByte <--> Int32 

        public static Int32 ReadOnlySpanByteToInt32(this ReadOnlySpan<byte> data,int startIndex=0)
        {             
            return  BitConverter.ToInt32(data.ToArray(), startIndex);
        }


        public static ReadOnlySpan<byte> Int32ToReadOnlySpanByte(this Int32 data)
        {
            return BitConverter.GetBytes(data);
        }
        #endregion




    }
}
