using System;
using System.Buffers;
using System.Collections.Generic;

namespace Vit.Core.Util.Pool
{
    public class DataPool
    {
       
      
        #region Bytes       
        public static byte[] BytesGet(int minimumLength)
        {
            // return new byte[minimumLength];
            return ArrayPool<byte>.Shared.Rent(minimumLength);
        }

        public static void BytesReturn(byte[] data)
        {
             ArrayPool<byte>.Shared.Return(data);
        }
        #endregion

        #region ArraySegmentByte       
        public static ArraySegment<byte> ArraySegmentByteGet(int length)
        {
            return new ArraySegment<byte>(BytesGet(length), 0, length);           
        }

        //public static void ArraySegmentByteReturn(ArraySegment<byte> data)
        //{            
        //    BytesReturn(data.Array);
        //}
        #endregion



        #region ByteData
 
        public static List<System.ArraySegment<byte>> ByteDataGet()
        {
            //return new List<System.ArraySegment<byte>>();
            return ObjectPool<List<System.ArraySegment<byte>>>.Shared.Pop();
        }

        internal static void ByteDataReturn(List<System.ArraySegment<byte>> data)
        {
            //return;
            ObjectPool<List<System.ArraySegment<byte>>>.Shared.Push(data);
        }
        #endregion


    }
}
