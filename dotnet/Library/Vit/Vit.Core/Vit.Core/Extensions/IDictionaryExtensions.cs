using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Vit.Extensions
{
    public static partial class IDictionaryExtensions
    {

        #region IDictionaryTryAdd

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="data"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IDictionaryTryAdd<TKey, TValue>(this IDictionary<TKey, TValue> data, TKey key, TValue value)
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
