using System.Runtime.CompilerServices;
using System.Text;

using Vit.Core.Module.Serialization;

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


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Encoding ToEncoding(this EEncoding type)
        {
            //if (null == type) return null;
            switch (type)
            {
                case EEncoding.ASCII: return Encoding.ASCII;
                case EEncoding.UTF32: return Encoding.UTF32;
                case EEncoding.UTF7: return Encoding.UTF7;
                case EEncoding.UTF8: return Encoding.UTF8;
                case EEncoding.Unicode: return Encoding.Unicode;
            }
            return null;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Encoding ToEncoding(this EEncoding? type)
        {
            if (null == type) return null;
            switch (type.Value)
            {
                case EEncoding.ASCII: return Encoding.ASCII;
                case EEncoding.UTF32: return Encoding.UTF32;
                case EEncoding.UTF7: return Encoding.UTF7;
                case EEncoding.UTF8: return Encoding.UTF8;
                case EEncoding.Unicode: return Encoding.Unicode;
            }
            return null;
        }


    }
}
