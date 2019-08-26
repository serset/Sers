using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Sers.Mq.Zmq.ClrZmq.RouteDealer
{
    class RequestInfo
    {
        public ArraySegment<byte> replyData;
        public AutoResetEvent mEvent;
    };
}
