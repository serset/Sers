using System;
using System.Buffers;

using ByteData = System.Collections.Generic.List<System.ArraySegment<byte>>;

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
 
        public static ByteData ByteDataGet()
        {
            //return new ByteData();
            return ObjectPool<ByteData>.Shared.Pop();
        }

        internal static void ByteDataReturn(ByteData data)
        {
            //return;
            ObjectPool<ByteData>.Shared.Push(data);
        }
        #endregion


    }
}
