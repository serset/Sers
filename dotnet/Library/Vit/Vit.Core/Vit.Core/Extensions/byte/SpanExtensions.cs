﻿using System;
using System.Runtime.CompilerServices;
using System.Text;

using Vit.Core.Module.Serialization;

namespace Vit.Extensions
{
    public static partial class SpanExtensions
    {


        #region SpanToString

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string SpanToString(this ReadOnlySpan<byte> data, Encoding encoding = null)
        {
            return Serialization.Instance.SpanToString(data, encoding);
        }

        #endregion


 

 


    }
}