using Newtonsoft.Json.Linq;
using System;

namespace Sers.Core.Util.StreamSecurity
{
    public interface ISecurity
    {
        ISecurity Clone();
        void Init(JObject config);

        void Encryption(ArraySegment<byte> data);  

        void Decryption(ArraySegment<byte> data);     
    }
}
