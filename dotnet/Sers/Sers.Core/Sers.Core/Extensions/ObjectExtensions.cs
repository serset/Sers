using Newtonsoft.Json;
using System;

namespace Sers.Core.Extensions
{
    public static partial class ObjectExtensions
    {

        #region IsValueTypeOrStringType
        /// <summary>
        /// Gets a value indicating whether the data is a value type or string type
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool IsValueTypeOrStringType(this object data)
        {
            if (data == null)
            {
                return false;
                //throw new ArgumentNullException(nameof(type));
            }
            return data.GetType().TypeIsValueTypeOrStringType();
        }
        #endregion

    }
}
