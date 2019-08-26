using Newtonsoft.Json.Linq;
using Sers.Core.Module.Mq.Mq;
using Sers.Core.Module.Mq.MqManager;
using System.Collections.Generic;
using Sers.Core.Extensions;
using Sers.Mq.Ipc.SharedMemory.Mq;

namespace Sers.Mq.Ipc.SharedMemory.MqBuilder
{
    public class ClientMqBuilder : IClientMqBuilder
    {
        public void BuildMq(List<IClientMq> mqList, JObject config)
        {
            var mqConfig = config.ConvertBySerialize<MqConfig>();

            var mq = new ClientMq();

            mq.name = mqConfig.name;
            mq.secretKey = mqConfig.secretKey;

            mqList.Add(mq); 
        }
    }
}
