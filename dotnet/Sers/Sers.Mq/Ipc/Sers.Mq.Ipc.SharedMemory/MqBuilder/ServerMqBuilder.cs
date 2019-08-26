using Newtonsoft.Json.Linq;
using Sers.Core.Module.Mq.Mq;
using Sers.Core.Module.Mq.MqManager;
using System.Collections.Generic;
using Sers.Core.Extensions;
using Sers.Mq.Ipc.SharedMemory.Mq;

namespace Sers.Mq.Ipc.SharedMemory.MqBuilder
{
    public class ServerMqBuilder : IServerMqBuilder
    {
        public void BuildMq(List<IServerMq> mqList, JObject config)
        {
            var mqConfig = config.ConvertBySerialize<MqConfig>();

            var mq = new ServerMq();

            mq.name = mqConfig.name;
            mq.nodeCount = mqConfig.nodeCount;
            mq.nodeBufferSize = mqConfig.nodeBufferSize;

            mqList.Add(mq);
        }
    }
}
