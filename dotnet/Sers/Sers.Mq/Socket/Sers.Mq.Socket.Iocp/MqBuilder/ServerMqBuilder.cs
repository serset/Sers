using Newtonsoft.Json.Linq;
using Sers.Core.Module.Mq.Mq;
using Sers.Core.Module.Mq.MqManager;
using System.Collections.Generic;
using Sers.Core.Extensions;

namespace Sers.Mq.Socket.Iocp.MqBuilder
{
    public class ServerMqBuilder : IServerMqBuilder
    {
        public void BuildMq(List<IServerMq> mqList, JObject config)
        {
            var mq = new ServerMq();

            mq.host = config["host"].ConvertToString();
            mq.port = config["port"].Convert<int>(); 

            mqList.Add(mq); 
        }
    }
}
