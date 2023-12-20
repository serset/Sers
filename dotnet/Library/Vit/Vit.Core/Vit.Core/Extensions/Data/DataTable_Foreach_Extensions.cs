using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;

namespace Vit.Extensions
{
    public static partial class DataTable_Foreach_Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Foreach(this DataTable dt,Action<DataRow> action)
        {
            foreach (DataRow dr in dt.Rows)
            {
                action(dr);
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> Select<T>(this DataTable dt, Func<DataRow, T> getField)
        {
            foreach (DataRow dr in dt.Rows)
            {
                yield return getField(dr);
            }
        }


    }
}
