using System;
using System.Collections.Generic;
using Sers.Core.Module.Serialization;

namespace Sers.Core.Extensions
{
    public static partial class BytesExtensions
    {

        #region bytes <--> String
    
        public static string BytesToString(this byte[] data)
        {             
            return Serialization.Instance.encoding.GetString(data); 
        }


        public static byte[] StringToBytes(this string data)
        {
            return Serialization.Instance.encoding.GetBytes(data);
        }
        #endregion


        #region bytes <--> Int32

        public static Int32 BytesToInt32(this byte[] data,int startIndex=0)
        {
            return  BitConverter.ToInt32(data, startIndex);
        }


        public static byte[] Int32ToBytes(this Int32 data)
        {
            return BitConverter.GetBytes(data);
        }
        #endregion

        #region bytes <--> Int64

        public static Int64 BytesToInt64(this byte[] data, int startIndex = 0)
        {
            return BitConverter.ToInt64(data, startIndex);
        }


        public static byte[] Int64ToBytes(this Int64 data)
        {
            return BitConverter.GetBytes(data);
        }
        #endregion

        #region bytes -> ArraySegmentByte
        public static ArraySegment<byte> BytesToArraySegmentByte(this byte[] bytes)
        {
            return null == bytes? ArraySegmentByteExtensions.Null: new ArraySegment<byte>(bytes, 0,bytes.Length);
        }


        public static List<ArraySegment<byte>> BytesToByteData(this byte[] bytes)
        {
            return null == bytes?null: new List<ArraySegment<byte>> { bytes.BytesToArraySegmentByte() };
        }
        #endregion


    }
}
