using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Sers.Core.Extensions
{
    public static partial class DictionaryExtensions
    {

        #region IsValueTypeOrStringType
       
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> data, TKey key, TValue value)
        {
            if (data == null || data.ContainsKey(key))
            {
                return false;               
            }
            data.Add(key, value);
            return true;
        }
        #endregion

    }
}
