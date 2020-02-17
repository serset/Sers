using System;
using System.Collections.Generic;
using System.Text;
using Vit.Core.Module.Serialization;

namespace Vit.Extensions
{
    public static partial class Base64StringExtensions
    {

        #region bytes <-->  Base64String

        public static string BytesToBase64String(this byte[] data)
        {
            return Convert.ToBase64String(data);
        }


        public static byte[] Base64StringToBytes(this string data)
        {
            return Convert.FromBase64String(data);
        }
        #endregion


     


         


    }
}
