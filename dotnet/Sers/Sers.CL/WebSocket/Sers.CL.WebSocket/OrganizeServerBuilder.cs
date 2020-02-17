using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Sers.Core.CL.MessageOrganize;
using Sers.Core.CL.MessageOrganize.DefaultOrganize;
using Vit.Extensions;

namespace Sers.CL.WebSocket
{
    public class OrganizeServerBuilder : IOrganizeServerBuilder
    {
        public void Build(List<IOrganizeServer> organizeList, JObject config)
        {
            var delivery = new DeliveryServer();

            delivery.host = config["host"].ConvertToString() ?? delivery.host;

            organizeList.Add(new OrganizeServer(delivery, config));
        }
       
    }
}
