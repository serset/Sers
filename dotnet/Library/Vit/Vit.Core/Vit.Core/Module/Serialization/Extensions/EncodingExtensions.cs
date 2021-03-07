using Newtonsoft.Json;
using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Vit.Extensions
{
    public static partial class EncodingExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetCharset(this Encoding value)
        {
            if(value== Encoding.UTF8)
                return "UTF-8";
             
            return value.ToString();
        }

    }
}
