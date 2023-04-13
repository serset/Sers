using System;
using System.Runtime.CompilerServices;

using Vit.Core.Util.Pool;

namespace Vit.Extensions.Json_Extensions
{
    public static partial class ArraySegmentBytePool_Extensions
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReturnToPool(this ArraySegment<byte> data)
        {
            DataPool.BytesReturn(data.Array);
        }




    }
}
