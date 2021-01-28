using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace Vit.Extensions
{
    public static class IFormFileEntensions_ReadBytes
    {
        public static async Task<byte[]> ReadBytesAsync(this IFormFile data)
        {
            using (var stream = new MemoryStream())
            {
                await data.CopyToAsync(stream);
                return stream.ToArray();
            }
        }
    }
}
