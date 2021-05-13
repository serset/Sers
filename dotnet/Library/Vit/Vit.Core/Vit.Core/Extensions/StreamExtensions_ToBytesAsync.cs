using System.IO;
using System.Threading.Tasks;

namespace Vit.Extensions
{
    public static partial class StreamExtensions_ToBytesAsync
    {

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static async Task<byte[]> ToBytesAsync(this Stream data)
        {
            if (data is MemoryStream ms_) 
            {
                if (ms_.Length > 0)
                {
                    return ms_.ToArray();
                }
                return null;
            }

            using (MemoryStream ms = new MemoryStream())
            {
                await data.CopyToAsync(ms);
                if (ms.Length > 0)
                {
                    return ms.ToArray();
                }
                return null;
            }
           
        }



    }
}
