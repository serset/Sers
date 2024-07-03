using System;

using Sers.Core.Module.Api.ApiDesc;

namespace Sers.Core.Module.Api.LocalApi
{
    public interface IApiNode
    {
        SsApiDesc apiDesc { get; }


        byte[] Invoke(ArraySegment<byte> request);
    }
}
