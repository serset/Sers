using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

using Vit.Core.Module.Serialization;

namespace Vit.Extensions.Json_Extensions
{
    public static partial class BytesExtensions
    {

        #region bytes <--> String

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string BytesToString(this byte[] data, Encoding encoding = null)
        {
            return Serialization_Newtonsoft.Instance.BytesToString(data, encoding);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] StringToBytes(this string data, Encoding encoding = null)
        {
            return Serialization_Newtonsoft.Instance.StringToBytes(data, encoding);
        }
        #endregion

        #region bytes <--> Int32

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int BytesToInt32(this byte[] data, int startIndex = 0)
        {
            return BitConverter.ToInt32(data, startIndex);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] Int32ToBytes(this int data)
        {
            return BitConverter.GetBytes(data);
        }
        #endregion

        #region bytes <--> Int64

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long BytesToInt64(this byte[] data, int startIndex = 0)
        {
            return BitConverter.ToInt64(data, startIndex);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] Int64ToBytes(this long data)
        {
            return BitConverter.GetBytes(data);
        }
        #endregion

        #region bytes -> ArraySegmentByte
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ArraySegment<byte> BytesToArraySegmentByte(this byte[] bytes)
        {
            return null == bytes ? ArraySegmentByteExtensions.Null : new ArraySegment<byte>(bytes, 0, bytes.Length);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<ArraySegment<byte>> BytesToByteData(this byte[] bytes)
        {
            return null == bytes ? null : new List<ArraySegment<byte>> { bytes.BytesToArraySegmentByte() };
        }
        #endregion


    }
}
