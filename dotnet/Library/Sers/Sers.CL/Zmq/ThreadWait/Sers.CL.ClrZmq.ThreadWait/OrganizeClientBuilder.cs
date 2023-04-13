using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using Sers.Core.CL.MessageOrganize;
using Sers.Core.CL.MessageOrganize.DefaultOrganize;

using Vit.Extensions.Newtonsoft_Extensions;

namespace Sers.CL.ClrZmq.ThreadWait
{
    public class OrganizeClientBuilder : IOrganizeClientBuilder
    {
        public void Build(List<IOrganizeClient> organizeList, JObject config)
        {
            var delivery = new DeliveryClient();

            #region security        
            if (config["security"] is JArray securityConfigs)
            {
                var securityManager = Sers.Core.Util.StreamSecurity.SecurityManager.BuildSecurityManager(securityConfigs);
                ((DeliveryConnection)delivery.conn).securityManager = securityManager;
            }
            #endregion

            delivery.endpoint = config["endpoint"].ConvertToString();

            organizeList.Add(new OrganizeClient(delivery, config));
        }
    }
}
