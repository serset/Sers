using System;
using Sers.CL.Socket.Iocp.Base;
using Vit.Core.Module.Log;

namespace Sers.CL.Socket.Iocp.Mode.Simple
{
    public class DeliveryClient : DeliveryClient_Base<DeliveryConnection>
    {         

         
        public override bool Connect()
        {
            try
            {
                Logger.Info("[CL.DeliveryClient] Socket.Iocp,connecting", new { host = host, port = port });


                if (!base.Connect()) 
                {
                    return false;
                }


                Logger.Info("[CL.DeliveryClient] Socket.Iocp,connected");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return false;
        }

    }
}
