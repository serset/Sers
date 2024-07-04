using System;
using System.Runtime.CompilerServices;

namespace Vit.Extensions.Serialize_Extensions
{
    public static partial class HexStringExtensions
    {

        #region bytes <--> HexString

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string BytesToHexString(this byte[] data)
        {
            return BitConverter.ToString(data).Replace("-", string.Empty);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] HexStringToBytes(this string hex)
        {
            var bytes = new byte[hex.Length / 2];
            for (var x = 0; x < bytes.Length; x++)
            {
                var i = Convert.ToInt32(hex.Substring(x * 2, 2), 16);
                bytes[x] = (byte)i;
            }
            return bytes;
        }
        #endregion



        #region Int64 <--> HexString

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Int64ToHexString(this long data)
        {
            return Convert.ToString(data, 16);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long HexStringToInt64(this string hex)
        {
            //return Convert.ToInt64(hex, 16);
            return long.Parse(hex, System.Globalization.NumberStyles.AllowHexSpecifier);
        }
        #endregion

    }
}
