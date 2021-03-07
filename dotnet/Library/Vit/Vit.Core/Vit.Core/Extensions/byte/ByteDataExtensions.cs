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
            int count = 0;
            foreach (var item in byteData)
            {
                count += item.Count;
            }

            var bytes = new byte[count];

            int curIndex = 0;
            foreach (var item in byteData)
            {
                if (null == item.Array || item.Count == 0) continue;

                //item.CopyTo(bytes, curIndex);
                //Buffer.BlockCopy(item.Array, item.Offset, bytes, curIndex, item.Count);

                unsafe
                {
                    fixed (byte* pSource = item.Array, pTarget = bytes)
                    {
                        Buffer.MemoryCopy(pSource + item.Offset, pTarget + curIndex, item.Count, item.Count);
                    }
                }

                curIndex += item.Count;
            }
            return bytes;
        }


        
        #endregion

       

    }
}
