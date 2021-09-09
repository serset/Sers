using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Vit.Extensions
{
    /// <summary>
    ///  
    /// </summary>
    public static partial class IEnumeratorExtensions
    {

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<T> IEnumerator_ToList<T>(this IEnumerator data)
        {
            var list = new List<T>();
            data.Reset();
            while (data.MoveNext())
            {
                list.Add((T)data.Current);
            }
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<T> IEnumerator_ToList<T>(this IEnumerator<T> data)
        {
            var list = new List<T>();
            data.Reset();
            while (data.MoveNext())
            {
                list.Add(data.Current);
            }
            return list;
        }


    }
}
