using System;
using System.Runtime.CompilerServices;

using Newtonsoft.Json;

using Sers.Core.Module.Api.ApiDesc;

using Vit.Extensions;

namespace Sers.Core.Module.Api.LocalApi
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ApiNode_Original : IApiNode
    {
        [JsonProperty]
        public SsApiDesc apiDesc { get; set; }



        [JsonIgnore]
        public Func<ArraySegment<byte>, byte[]> onInvoke { get; set; }



        public ApiNode_Original(Func<ArraySegment<byte>, byte[]> onInvoke = null, SsApiDesc apiDesc = null)
        {
            this.apiDesc = apiDesc;
            this.onInvoke = onInvoke;
        }

        public ApiNode_Original(Func<ArraySegment<byte>, byte[]> onInvoke, string route, string httpMethod = null)
        {
            this.apiDesc = new SsApiDesc { route = route };
            if (httpMethod != null)
                apiDesc.HttpMethodSet(httpMethod);

            this.onInvoke = onInvoke;
        }





        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] Invoke(ArraySegment<byte> arg_OriData)
        {
            return onInvoke(arg_OriData);
        }





    }
}
