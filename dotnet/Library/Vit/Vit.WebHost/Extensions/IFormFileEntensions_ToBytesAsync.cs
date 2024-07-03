using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace Vit.Extensions
{
    public static partial class IFormFileEntensions_ToBytesAsync
    {

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static async Task<byte[]> ToBytesAsync(this IFormFile data)
        {
            using var stream = new MemoryStream();
            await data.CopyToAsync(stream);
            return stream.ToArray();
        }
    }
}
