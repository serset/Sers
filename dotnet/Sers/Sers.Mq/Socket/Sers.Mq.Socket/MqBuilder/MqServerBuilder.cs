using Sers.Core.Mq;
using Sers.Core.Mq.Mng;
using Sers.Core.Util.ConfigurationManager;

namespace Sers.Mq.Socket.MqBuilder
{
    public class MqServerBuilder : IServerMqBuilder
    {
        public IServerMq BuildMq()
        {
            var config = ConfigurationManager.Instance.GetByPath<ServerMqConfig>("Sers.Mq.Socket");
            if (null == config)
            {
                return null;
            }

            return new ServerMq(config);
        }
    }
}
