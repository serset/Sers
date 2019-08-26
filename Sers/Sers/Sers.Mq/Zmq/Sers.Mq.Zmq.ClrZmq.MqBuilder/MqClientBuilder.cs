using Sers.Core.Mq;
using Sers.Core.Mq.Mng;
using Sers.Core.Util.ConfigurationManager;
using Sers.Mq.Zmq.ClrZmq.RouteDealer;

namespace Sers.Mq.Zmq.ClrZmq.MqBuilder
{
    public class MqClientBuilder : IClientMqBuilder
    {
        public IClientMq BuildMq()
        {

            var config = ConfigurationManager.Instance.GetByPath<ClientMqConfig>("Sers.Mq.Zmq"); 
      

            if (string.IsNullOrEmpty(config?.host))
            {
                return null;
            }

            return new ClientMq(config);
        }
    }
}
