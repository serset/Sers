using System;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Sers.Core.Module.Api.ApiDesc;
using Sers.Core.Module.Api.LocalApi;
using Vit.Extensions;

namespace Sers.Serslot
{
    [JsonObject(MemberSerialization.OptIn)]
    public class LocalApiNode: IApiNode
    {
        [JsonProperty]
        public SsApiDesc apiDesc { get; set; }


        SerslotServer server;
        public LocalApiNode(SsApiDesc apiDesc, SerslotServer server)
        {
            this.apiDesc = apiDesc;
            this.server = server;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] Invoke(ArraySegment<byte> arg_OriData)
        {
            return server.ProcessRequestByRpc(arg_OriData);
        }       


    }
}
