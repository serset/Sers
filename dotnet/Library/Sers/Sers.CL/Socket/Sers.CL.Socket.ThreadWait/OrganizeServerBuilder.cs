﻿using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using Sers.Core.CL.MessageOrganize;
using Sers.Core.CL.MessageOrganize.DefaultOrganize;

using Vit.Extensions.Newtonsoft_Extensions;
using Vit.Extensions.Serialize_Extensions;

namespace Sers.CL.Socket.ThreadWait
{
    public class OrganizeServerBuilder : IOrganizeServerBuilder
    {
        public void Build(List<IOrganizeServer> organizeList, JObject config)
        {
            var delivery = new DeliveryServer();

            #region security        
            if (config["security"] is JArray securityConfigs)
            {
                var securityManager = Sers.Core.Util.StreamSecurity.SecurityManager.BuildSecurityManager(securityConfigs);
                delivery.securityManager = securityManager;
            }
            #endregion

            delivery.host = config["host"].ConvertToString();
            delivery.port = config["port"].Convert<int>();

            organizeList.Add(new OrganizeServer(delivery, config));
        }
    }
}
