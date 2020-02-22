using System;

namespace Sers.Core.Module.PubSub
{
    public interface ISubscriber
    {
        string msgTitle { get; }

        void OnGetMessage(ArraySegment<byte> msgBody);
    }
}
