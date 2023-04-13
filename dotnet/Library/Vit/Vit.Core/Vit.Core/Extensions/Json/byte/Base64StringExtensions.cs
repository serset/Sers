using System;
using System.Runtime.CompilerServices;

namespace Vit.Extensions.Json_Extensions
{
    public static partial class Base64StringExtensions
    {

        #region bytes <-->  Base64String

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string BytesToBase64String(this byte[] data)
        {
            return Convert.ToBase64String(data);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] Base64StringToBytes(this string data)
        {
            return Convert.FromBase64String(data);
        }
        #endregion         


    }
}
