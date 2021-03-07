using System;
using System.Collections.Generic;
using Sers.Core.CL.MessageDelivery;

namespace Sers.Core.CL.MessageOrganize.DefaultOrganize
{
    public class OrganizeConnection : IOrganizeConnection
    {
        public string connTag { get; set; }

        internal IDeliveryConnection deliveryConn { get; private set; }

        private RequestAdaptor requestAdaptor;
        public OrganizeConnection(IDeliveryConnection deliveryConn, RequestAdaptor requestAdaptor)
        {
            this.deliveryConn = deliveryConn;
            this.requestAdaptor = requestAdaptor;
        }


        public void SendMessageAsync(Vit.Core.Util.Pipelines.ByteData message)
        {
            requestAdaptor.SendMessageAsync(this, message);
        }


        public void SendRequestAsync(Object sender, Vit.Core.Util.Pipelines.ByteData requestData, Action<object, Vit.Core.Util.Pipelines.ByteData> callback)
        {
            requestAdaptor.SendRequestAsync(this, sender, requestData, callback);
        }
        public bool SendRequest(Vit.Core.Util.Pipelines.ByteData requestData, out Vit.Core.Util.Pipelines.ByteData replyData)
        {
            return requestAdaptor.SendRequest(this, requestData, out replyData);
        }

        public void Close() 
        {
            deliveryConn?.Close();
            deliveryConn = null;
        }

    }


}
