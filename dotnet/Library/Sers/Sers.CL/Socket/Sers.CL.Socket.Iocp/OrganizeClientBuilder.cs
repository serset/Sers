using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Sers.Core.CL.MessageOrganize;
using Sers.Core.CL.MessageOrganize.DefaultOrganize;
using Vit.Extensions;

namespace Sers.CL.Socket.Iocp
{
    public class OrganizeClientBuilder : IOrganizeClientBuilder
    {
        public void Build(List<IOrganizeClient> organizeList, JObject config)
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
                        var delivery = new Mode.Simple.DeliveryClient();

                        delivery.host = config["host"].ConvertToString();
                        delivery.port = config["port"].Convert<int>();

                        ((Mode.Simple.DeliveryConnection)delivery.conn).securityManager = securityManager;

                        organizeList.Add(new OrganizeClient(delivery, config));
                    }
                    break;
                case "Timer":
                    {
                        var delivery = new Mode.Timer.DeliveryClient();

                        delivery.host = config["host"].ConvertToString();
                        delivery.port = config["port"].Convert<int>();

                        ((Mode.Timer.DeliveryConnection)delivery.conn).securityManager = securityManager;

                        organizeList.Add(new OrganizeClient(delivery, config));
                    }
                    break;
                //case "SpinWait":
                default:
                    {
                        var delivery = new Mode.SpinWait.DeliveryClient();

                        delivery.host = config["host"].ConvertToString();
                        delivery.port = config["port"].Convert<int>();

                        ((Mode.Timer.DeliveryConnection)delivery.conn).securityManager = securityManager;

                        organizeList.Add(new OrganizeClient(delivery, config));
                    }
                    break;
            }
        }
    }
}
