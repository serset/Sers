using Newtonsoft.Json.Linq;
using Sers.Core.Module.Mq.Mq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sers.Core.Module.Mq.MqManager
{
    public interface IServerMqBuilder
    {
        void BuildMq(List<IServerMq> mqList, JObject config);
    }
}
