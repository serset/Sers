using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Vit.Extensions
{
    public static partial class StringExtensions
    {

        /// <summary>
        ///  Concatenates the members of a constructed System.Collections.Generic.IEnumerable`1 collection of type System.String, using the specified separator between each member.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string StringJoin(this IEnumerable<string> data, string separator)
        {
            if (data == null)
            {
                return null;
            }
            return string.Join(separator, data);
        }



    }
}
