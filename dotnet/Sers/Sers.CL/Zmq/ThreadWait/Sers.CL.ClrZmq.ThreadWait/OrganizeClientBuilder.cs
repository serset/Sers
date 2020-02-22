using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Sers.Core.CL.MessageOrganize;
using Sers.Core.CL.MessageOrganize.DefaultOrganize;
using Vit.Extensions;

namespace Sers.CL.ClrZmq.ThreadWait
{
    public class OrganizeClientBuilder : IOrganizeClientBuilder
    {
        public void Build(List<IOrganizeClient> organizeList, JObject config)
        {
            var delivery = new DeliveryClient();

            delivery.endpoint = config["endpoint"].ConvertToString();

            organizeList.Add(new OrganizeClient(delivery, config));
        }
    }
}
