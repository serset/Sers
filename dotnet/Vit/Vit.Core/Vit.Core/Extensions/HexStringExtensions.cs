using System;

namespace Vit.Extensions
{
    public static partial class HexStringExtensions
    {

        #region bytes <--> HexString

        public static string Int64ToHexString(this Int64 data)
        {
            return Convert.ToString(data, 16);
        }


        public static Int64 HexStringToInt64(this string hex)
        {
            //return Convert.ToInt64(hex, 16);
            return Int64.Parse(hex, System.Globalization.NumberStyles.AllowHexSpecifier); 
        }
        #endregion



    }
}
