using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Sers.Core.CL.MessageOrganize;
using Sers.Core.CL.MessageOrganize.DefaultOrganize;
using Vit.Extensions;

namespace Sers.CL.Socket.Iocp
{
    public class OrganizeServerBuilder : IOrganizeServerBuilder
    {
        public void Build(List<IOrganizeServer> organizeList, JObject config)
        {
            var delivery = new DeliveryServer();

            delivery.host = config["host"].ConvertToString();
            delivery.port = config["port"].Convert<int>();

            organizeList.Add(new OrganizeServer(delivery, config));
        }
       
    }
}
