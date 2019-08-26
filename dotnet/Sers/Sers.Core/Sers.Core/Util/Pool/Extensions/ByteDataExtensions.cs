using Sers.Core.Util.Pool;
using System;
using System.Collections.Generic;

namespace Sers.Core.Extensions
{
    public static partial class ByteDataExtensions
    {
        
        public static void ReturnToPool(this List<ArraySegment<byte>> data)
        {
            foreach (var item in data)
            {
                DataPool.BytesReturn(item.Array);
            }
            data.Clear();

            DataPool.ByteDataReturn(data);
        }


      


    }
}
