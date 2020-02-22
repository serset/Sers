using System;
using System.Collections.Generic;

namespace Vit.Extensions.IEnumerable
{
    /// <summary>
    ///  
    /// </summary>
    public static partial class IEnumerableExtensions
    {
 
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this IEnumerable<T> data, Action<T> action)
        {
            foreach (var item in data)
            {
                action(item);
            }
        }

    }
}
