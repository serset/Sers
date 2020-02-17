using System;
using System.Collections.Generic;
using Sers.Core.CL.MessageDelivery;

namespace Sers.Core.CL.MessageOrganize.DefaultOrganize
{
    public class OrganizeConnection:IOrganizeConnection
    {
        public string connTag { get; set; }

        internal IDeliveryConnection deliveryConn { get; private set; }

        private RequestAdaptor requestAdaptor;
        public OrganizeConnection(IDeliveryConnection deliveryConn, RequestAdaptor requestAdaptor)
        {
            this.deliveryConn = deliveryConn;
            this.requestAdaptor = requestAdaptor;
        }


        public void SendMessageAsync(List<ArraySegment<byte>> message)
        {
            requestAdaptor.SendMessageAsync(this, message);
        }


        public void SendRequestAsync(Object sender, List<ArraySegment<byte>> requestData, Action<object, List<ArraySegment<byte>>> callback)
        {
            requestAdaptor.SendRequestAsync(this, sender, requestData, callback);
        }
        public bool SendRequest(List<ArraySegment<byte>> requestData, out List<ArraySegment<byte>> replyData)
        {
           return requestAdaptor.SendRequest(this,requestData, out replyData);
        }
    }


}
