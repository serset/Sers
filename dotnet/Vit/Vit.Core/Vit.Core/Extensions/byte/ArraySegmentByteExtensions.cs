using Vit.Core.Module.Serialization;
using System;

namespace Vit.Extensions
{
    public static partial class ArraySegmentByteExtensions
    {
        public static readonly ArraySegment<byte> Null = new ArraySegment<byte>(new byte[0],0,0);

        //public static ArraySegment<T> Null<T>()
        //{
        //    return new ArraySegment<T>(new T[0], 0,0);
        //}

        public static bool HasData<T>(this ArraySegment<T> seg)
        {
            return seg!=null && seg.Array!=null && seg.Count>0;
        }



        internal static void CopyTo<T>(this ArraySegment<T> seg,T[] bytes,int curIndex=0)
        {
            Array.Copy(seg.Array, seg.Offset, bytes, curIndex, seg.Count); 
        }

        public static ArraySegment<T> Slice<T>(this ArraySegment<T> seg,int Offset,int? count=null)
        {
            return new ArraySegment<T>(seg.Array,seg.Offset+ Offset, count?? (seg.Count-Offset) );
        }
     



        #region ArraySegmentByte <--> String

        public static string ArraySegmentByteToString(this ArraySegment<byte> data)
        {
            if (null == data || data.Array==null) return null;
            return data.Count==0?"":Serialization.Instance.encoding.GetString(data.Array,data.Offset,data.Count); 
        }


        public static ArraySegment<byte> StringToArraySegmentByte(this string data)
        {
            return (null == data) ? Null : Serialization.Instance.encoding.GetBytes(data).BytesToArraySegmentByte();
        }
        #endregion


        #region ArraySegmentByte <--> bytes
 
        public static byte[] ArraySegmentByteToBytes(this ArraySegment<byte> data)
        {
            if (null == data) return null;


            var bytes = new byte[data.Count];
            if (data.Count > 0)
            {
                data.CopyTo(bytes);
            }
            return bytes;
        }

 
        #endregion


        #region ArraySegmentByte <--> Int32

        public static Int32 ArraySegmentByteToInt32(this ArraySegment<byte> data,int startIndex=0)
        {             
            return  BitConverter.ToInt32(data.Array, data.Offset+startIndex);
        }


        public static ArraySegment<byte> Int32ToArraySegmentByte(this Int32 data)
        {
            return BitConverter.GetBytes(data).BytesToArraySegmentByte();
        }
        #endregion


        #region ArraySegmentByte <--> Int64

        public static Int64 ArraySegmentByteToInt64(this ArraySegment<byte> data, int startIndex = 0)
        {
            return BitConverter.ToInt64(data.Array, data.Offset + startIndex);
        }


        public static ArraySegment<byte> Int64ToArraySegmentByte(this Int64 data)
        {
            return BitConverter.GetBytes(data).BytesToArraySegmentByte();
        }
        #endregion

    }
}
