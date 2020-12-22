using System;

namespace Vit.Extensions
{
    public static partial class HexStringExtensions
    {

        #region bytes <--> HexString

        public static string BytesToHexString(this byte[] data)
        {
            return BitConverter.ToString(data).Replace("-", string.Empty);
        }


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


    }
}
