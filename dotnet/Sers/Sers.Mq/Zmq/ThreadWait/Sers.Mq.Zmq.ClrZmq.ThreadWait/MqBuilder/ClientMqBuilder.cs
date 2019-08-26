﻿using Newtonsoft.Json.Linq;
using Sers.Core.Module.Mq.Mq;
using Sers.Core.Module.Mq.MqManager;
using System.Collections.Generic;
using Sers.Core.Extensions;

namespace Sers.Mq.Zmq.ClrZmq.ThreadWait.MqBuilder
{
    public class ClientMqBuilder : IClientMqBuilder
    {
        public void BuildMq(List<IClientMq> mqList, JObject config)
        {
            var mq = new ClientMq();

            mq.endpoint = config["endpoint"].ConvertToString();        
            mq.secretKey = config["secretKey"].ConvertToString();

            mqList.Add(mq); 
        }
    }
}
