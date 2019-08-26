using Sers.Core.Mq.Mng;
using Sers.Mq.Socket.MqBuilder;

namespace Sers.Core.Extensions
{
    public static partial class MqMngExtensions
    {

        public static void UseSocket(this ClientMqMng mqMng)
        {
            if (null == mqMng) return;

            mqMng.AddMqBuilder(new MqClientBuilder());

        }


        public static void UseSocket(this ServerMqMng mqMng)
        {
            if (null == mqMng) return;

            mqMng.AddMqBuilder(new MqServerBuilder());

        }


    }
}
