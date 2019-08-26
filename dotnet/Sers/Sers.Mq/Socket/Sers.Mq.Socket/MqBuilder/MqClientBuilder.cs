using Sers.Core.Mq;
using Sers.Core.Mq.Mng;
using Sers.Core.Util.ConfigurationManager;

namespace Sers.Mq.Socket.MqBuilder
{
    public class MqClientBuilder : IClientMqBuilder
    {
        public IClientMq BuildMq()
        {
            var config = ConfigurationManager.Instance.GetByPath<ClientMqConfig>("Sers.Mq.Socket");


            if (string.IsNullOrEmpty(config?.host))
            {
                return null;
            }
            return new ClientMq(config);

        }
    }
}
