using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Vit.Extensions
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IEnumerable_ForEach<T>(this IEnumerable<T> data, Action<T> action)
        {
            foreach (var item in data)
            {
                action(item);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="action"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async Task IEnumerable_ForEachAsync<T>(this IEnumerable<T> data, Func<T, Task> action)
        {
            foreach (var item in data)
            {
                await action(item);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<T> IEnumerable_ToList<T>(this IEnumerable data) //ICollection data
        {
            return data?.GetEnumerator().IEnumerator_ToList<T>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<T> IEnumerable_ToList<T>(this IEnumerable<T> data) //ICollection data
        {
            return data?.GetEnumerator().IEnumerator_ToList();
        }

       
    }
}
