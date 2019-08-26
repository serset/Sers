using System;

namespace Sers.Core.Module.PubSub.ShareEndpoint
{
    public interface ISubscriber
    {
        string msgTitle { get; }

        void OnMessage(ArraySegment<byte> msgBody);
    }
}
