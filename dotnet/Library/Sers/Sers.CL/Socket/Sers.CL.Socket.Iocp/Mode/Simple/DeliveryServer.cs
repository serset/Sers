using System;

using Sers.CL.Socket.Iocp.Base;

using Vit.Core.Module.Log;

namespace Sers.CL.Socket.Iocp.Mode.Simple
{
    public class DeliveryServer : DeliveryServer_Base<DeliveryConnection>
    {


        public override bool Start()
        {
            try
            {
                Logger.Info("[CL.DeliveryServer] Socket.Iocp,starting", new { host, port });

                if (!base.Start())
                {
                    return false;
                }

                Logger.Info("[CL.DeliveryServer] Socket.Iocp,started");
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
