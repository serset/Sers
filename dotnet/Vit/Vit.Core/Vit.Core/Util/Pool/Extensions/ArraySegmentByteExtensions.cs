using System;
using System.Collections.Generic;
using System.Text;
using Vit.Core.Module.Serialization;
using Vit.Core.Util.Pool;

namespace Vit.Extensions
{
    public static partial class ArraySegmentByteExtensions
    {

        public static void ReturnToPool(this ArraySegment<byte> data)
        {
            DataPool.BytesReturn(data.Array);
        }




    }
}
