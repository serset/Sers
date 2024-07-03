using System;
using System.Collections.Generic;

using Sers.Core.Module.Api.LocalApi.Event;
using Sers.Core.Module.Message;

namespace Sers.Core.Module.Api.LocalApi
{
    public interface ILocalApiService
    {
        void Init();

        bool Start();
        void Stop();

        LocalApiEventMng localApiEventMng { get; }
        ApiNodeMng ApiNodeMng { get; }

        IEnumerable<IApiNode> apiNodes { get; }


        void InvokeApiAsync(Object sender, ApiMessage apiRequest, Action<object, ApiMessage> callback);
    }
}
