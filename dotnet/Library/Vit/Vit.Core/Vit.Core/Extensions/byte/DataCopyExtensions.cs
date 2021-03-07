using Vit.Core.Module.Serialization;
using System;

namespace Vit.Extensions
{
    public static partial class DataCopyExtensions
    {


 
        internal static void CopyTo<T>(this ArraySegment<T> seg, T[] bytes, int curIndex = 0)
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
