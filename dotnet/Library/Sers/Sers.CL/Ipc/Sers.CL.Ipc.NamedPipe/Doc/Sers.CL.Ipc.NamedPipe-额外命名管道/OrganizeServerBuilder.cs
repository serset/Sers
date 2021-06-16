using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Sers.Core.CL.MessageOrganize;
using Sers.Core.CL.MessageOrganize.DefaultOrganize;
using Vit.Extensions;

namespace Sers.CL.Ipc.NamedPipe
{
    public class OrganizeServerBuilder : IOrganizeServerBuilder
    {
        public void Build(List<IOrganizeServer> organizeList,JObject config)
        {
            var delivery = new DeliveryServer();
 
            delivery.pipeName = config["pipeName"].ConvertToString();

            organizeList.Add(new OrganizeServer(delivery, config)); 
        }
    }
}
