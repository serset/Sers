using System;
using System.Collections.Generic;
using System.Text;
using Vit.Core.Module.Serialization;
using Vit.Core.Util.Pool;

namespace Vit.Extensions
{
    public static partial class BytesExtensions
    {

        public static void ReturnToPool(this byte[] data)
        {
            DataPool.BytesReturn(data);
        }




    }
}
