using Newtonsoft.Json.Linq;
using Sers.Core.Module.Mq.Mq;
using Sers.Core.Module.Mq.MqManager;
using System.Collections.Generic;
using Sers.Core.Extensions;

namespace Sers.Mq.Zmq.ClrZmq.ThreadWait.MqBuilder
{
    public class ServerMqBuilder : IServerMqBuilder
    {
        public void BuildMq(List<IServerMq> mqList, JObject config)
        {
            var mq = new ServerMq();

            mq.endpoint = config["endpoint"].ConvertToString();

            mqList.Add(mq); 
        }
    }
}
