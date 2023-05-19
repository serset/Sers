using System;
using System.Runtime.CompilerServices;

namespace Vit.Extensions.Json_Extensions
{
    public static partial class DataCopyExtensions
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo(this ArraySegment<byte> source, byte[] dest, int destOffset = 0, int? count = null)
        {
            unsafe
            {
                fixed (byte* pSource = source.Array, pTarget = dest)
                {
                    Buffer.MemoryCopy(pSource + source.Offset, pTarget + destOffset, count ?? source.Count, count ?? source.Count);
                }
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void CopyTo(this byte[] source, int sourceOffset, int count, byte[] dest, int destOffset = 0)
        {
            unsafe
            {
                fixed (byte* pSource = source, pTarget = dest)
                {
                    Buffer.MemoryCopy(pSource + sourceOffset, pTarget + destOffset, count, count);
                }
            }
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe byte[] Clone(this byte[] source, int sourceOffset, int count)
        {
            byte[] dest = new byte[count];

            fixed (byte* pSource = source, pTarget = dest)
            {
                Buffer.MemoryCopy(pSource + sourceOffset, pTarget, count, count);
            }
            return dest;
        }






        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CopyTo<T>(this ArraySegment<T> seg, T[] bytes, int curIndex = 0)
        {
            //Array.Copy(seg.Array, seg.Offset, bytes, curIndex, seg.Count);
            Buffer.BlockCopy(seg.Array, seg.Offset, bytes, curIndex, seg.Count);


            //data.CopyTo(bytes);

            //unsafe
            //{
            //    fixed (byte* pSource = seg.Array, pTarget = bytes)
            //    {
            //        Buffer.MemoryCopy(pSource + data.Offset, pTarget, data.Count, data.Count);
            //    }
            //}
        }


    }
}
