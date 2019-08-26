using Newtonsoft.Json;
using System;
using System.Text;

namespace Sers.Core.Extensions
{
    public static partial class EncodingExtensions
    {
        public static string GetCharset(this Encoding value)
        {
            if(value== Encoding.UTF8)
                return "UTF-8";
             
            return value.ToString();
        }

    }
}
