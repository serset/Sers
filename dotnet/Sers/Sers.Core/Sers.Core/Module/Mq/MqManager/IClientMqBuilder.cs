using Newtonsoft.Json.Linq;
using Sers.Core.Module.Mq.Mq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sers.Core.Module.Mq.MqManager
{
    public interface IClientMqBuilder
    {
        void BuildMq(List<IClientMq> mqList,JObject config);
    }
}
