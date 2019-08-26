using System;
using System.Collections.Generic;
using System.Text;
using Sers.Core.Module.Serialization;
using Sers.Core.Util.Pool;

namespace Sers.Core.Extensions
{
    public static partial class ArraySegmentByteExtensions
    {

        public static void ReturnToPool(this ArraySegment<byte> data)
        {
            DataPool.BytesReturn(data.Array);
        }




    }
}
