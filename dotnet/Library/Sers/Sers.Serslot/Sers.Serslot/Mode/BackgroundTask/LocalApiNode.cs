using System;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Sers.Core.Module.Api.ApiDesc;
using Sers.Core.Module.Api.LocalApi;
using Vit.Extensions;

namespace Sers.Serslot
{
    [JsonObject(MemberSerialization.OptIn)]
    public class LocalApiNode<TServer> : IApiNode
    {
        [JsonProperty]
        public SsApiDesc apiDesc { get; set; }

 
        public LocalApiNode(SsApiDesc apiDesc)
        {
            this.apiDesc = apiDesc;        
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] Invoke(ArraySegment<byte> arg_OriData)
        {
            throw new NotImplementedException();
        }       


    }
}
