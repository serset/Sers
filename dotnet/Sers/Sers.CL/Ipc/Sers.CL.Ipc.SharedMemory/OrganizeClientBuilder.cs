using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Sers.Core.CL.MessageOrganize;
using Sers.Core.CL.MessageOrganize.DefaultOrganize;
using Vit.Extensions;

namespace Sers.CL.Ipc.SharedMemory
{
    public class OrganizeClientBuilder : IOrganizeClientBuilder
    {
        public void Build(List<IOrganizeClient> organizeList, JObject config)
        {

            var connConfig = config.ConvertBySerialize<ConnConfig>();

            var delivery = new DeliveryClient();

            delivery.name = connConfig.name;

            organizeList.Add(new OrganizeClient(delivery, config));
        }


        
    }
}
