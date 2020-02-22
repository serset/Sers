using System;
using Newtonsoft.Json;
using Vit.Extensions;
using Sers.Core.Module.Api.ApiDesc;
using Sers.Core.Module.Api.LocalApi; 

namespace Sers.Serslot21
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


        public /*virtual*/ byte[] Invoke(ArraySegment<byte> arg_OriData)
        {
            return server.ProcessRequestByRpc(arg_OriData);
        }       


    }
}
