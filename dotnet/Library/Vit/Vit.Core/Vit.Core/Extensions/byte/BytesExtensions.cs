using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Vit.Core.Module.Serialization;

namespace Vit.Extensions
{
    public static partial class BytesExtensions
    {

        #region bytes <--> String

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string BytesToString(this byte[] data, Encoding encoding = null)
        {             
            return Serialization.Instance.BytesToString(data, encoding); 
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] StringToBytes(this string data, Encoding encoding = null)
        {
            return Serialization.Instance.StringToBytes(data, encoding);
        }
        #endregion

        #region bytes <--> Int32

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int32 BytesToInt32(this byte[] data,int startIndex=0)
        {
            return  BitConverter.ToInt32(data, startIndex);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] Int32ToBytes(this Int32 data)
        {
            return BitConverter.GetBytes(data);
        }
        #endregion

        #region bytes <--> Int64

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Int64 BytesToInt64(this byte[] data, int startIndex = 0)
        {
            return BitConverter.ToInt64(data, startIndex);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] Int64ToBytes(this Int64 data)
        {
            return BitConverter.GetBytes(data);
        }
        #endregion

        #region bytes -> ArraySegmentByte
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ArraySegment<byte> BytesToArraySegmentByte(this byte[] bytes)
        {
            return null == bytes? ArraySegmentByteExtensions.Null: new ArraySegment<byte>(bytes, 0,bytes.Length);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<ArraySegment<byte>> BytesToByteData(this byte[] bytes)
        {
            return null == bytes?null: new List<ArraySegment<byte>> { bytes.BytesToArraySegmentByte() };
        }
        #endregion


    }
}
