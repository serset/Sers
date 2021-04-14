using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Vit.Extensions
{
    public static partial class ByteDataExtensions
    {

        #region ByteDataToBytes

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ByteDataToBytes(this List<ArraySegment<byte>> byteData)
        {
            //(x.1)get length
            int count = 0;
            foreach (var item in byteData)
            {
                count += item.Count;
            }


            //(x.2) copy data
            var bytes = new byte[count];

            int curIndex = 0;
            foreach (var item in byteData)
            {
                if (null == item.Array || item.Count == 0) continue; 

                item.CopyTo(bytes, curIndex);                

                curIndex += item.Count;
            }
            return bytes;
        }



        #endregion




        

    }
}
