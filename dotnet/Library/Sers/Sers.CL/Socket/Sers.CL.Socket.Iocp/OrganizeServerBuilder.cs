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

            #region security     
            Sers.Core.Util.StreamSecurity.SecurityManager securityManager = null;
            if (config["security"] is JArray securityConfigs)
            {
                securityManager = Sers.Core.Util.StreamSecurity.SecurityManager.BuildSecurityManager(securityConfigs);
            }
            #endregion


            string mode = config["mode"]?.ToString();

            switch (mode)
            {
                case "Simple":
                    {
                        var delivery = new Mode.Simple.DeliveryServer();

                        delivery.securityManager = securityManager;

                        delivery.host = config["host"].ConvertToString();
                        delivery.port = config["port"].Convert<int>();

                        organizeList.Add(new OrganizeServer(delivery, config));
                    }
                    break;
                case "Timer":          
                    {
                        var delivery = new Mode.Timer.DeliveryServer();

                        delivery.securityManager = securityManager;

                        delivery.host = config["host"].ConvertToString();
                        delivery.port = config["port"].Convert<int>();

                        organizeList.Add(new OrganizeServer(delivery, config));
                    }
                    break;
                //case "SpinWait":
                default:
                    {
                        var delivery = new Mode.SpinWait.DeliveryServer();

                        delivery.securityManager = securityManager;

                        delivery.host = config["host"].ConvertToString();
                        delivery.port = config["port"].Convert<int>();

                        organizeList.Add(new OrganizeServer(delivery, config));
                    }
                    break;
            }


        }
       
    }
}
