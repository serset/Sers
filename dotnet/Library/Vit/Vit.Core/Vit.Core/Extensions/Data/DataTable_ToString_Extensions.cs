using System;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;

namespace Vit.Extensions
{
    public static partial class DataTable_ToString_Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToString(this DataTable dt, string fieldSeparator = null, string rowSeparator = null)
        {
            var build = new StringBuilder();
            var columnCount = dt.Columns.Count;
            foreach (DataRow dr in dt.Rows)
            {
                for (var t = 0; t < columnCount; t++)
                {
                    if (t != 0) build.Append(fieldSeparator);

                    build.Append(dr[t]);
                }
                build.Append(rowSeparator);
            }

            return build.ToString();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToString(this DataTable dt, Func<object, string> getValue, string fieldSeparator = null, string rowSeparator = null)
        {
            var build = new StringBuilder();
            var columnCount = dt.Columns.Count;
            foreach (DataRow dr in dt.Rows)
            {
                for (var t = 0; t < columnCount; t++)
                {
                    if (t != 0) build.Append(fieldSeparator);

                    build.Append(getValue(dr[t]));
                }
                build.Append(rowSeparator);
            }

            return build.ToString();
        }

    }
}
