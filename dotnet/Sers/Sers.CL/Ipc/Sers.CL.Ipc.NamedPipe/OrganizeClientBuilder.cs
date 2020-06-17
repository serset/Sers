using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Sers.Core.CL.MessageOrganize;
using Sers.Core.CL.MessageOrganize.DefaultOrganize;
using Vit.Extensions;

namespace Sers.CL.Ipc.NamedPipe
{
    public class OrganizeClientBuilder : IOrganizeClientBuilder
    {
        public void Build(List<IOrganizeClient> organizeList, JObject config)
        {
            var delivery = new DeliveryClient();

            delivery.serverName = config["serverName"].ConvertToString();
            delivery.pipeName = config["pipeName"].ConvertToString();      

            organizeList.Add(new OrganizeClient(delivery, config)); 
        }
    }
}
