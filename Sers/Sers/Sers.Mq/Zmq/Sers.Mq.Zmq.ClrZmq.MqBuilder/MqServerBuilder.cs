using Sers.Core.Mq;
using Sers.Core.Mq.Mng;
using Sers.Core.Util.ConfigurationManager;
using Sers.Mq.Zmq.ClrZmq.RouteDealer;

namespace Sers.Mq.Zmq.ClrZmq.MqBuilder
{
    public class MqServerBuilder : IServerMqBuilder
    {
        public IServerMq BuildMq()
        {

            var config = ConfigurationManager.Instance.GetByPath<ServerMqConfig>("Sers.Mq.Zmq");
            if (null== config)
            {
                return null;
            }

            return new ServerMq(config);
        }
    }
}
