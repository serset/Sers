using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Vit.Core.Module.Serialization;
using Vit.Core.Util.Pool;

namespace Vit.Extensions
{
    public static partial class ArraySegmentByteExtensions
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReturnToPool(this ArraySegment<byte> data)
        {
            DataPool.BytesReturn(data.Array);
        }




    }
}
