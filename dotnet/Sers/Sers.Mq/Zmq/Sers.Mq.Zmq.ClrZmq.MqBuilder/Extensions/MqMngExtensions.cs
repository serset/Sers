using Sers.Core.Mq.Mng;
using Sers.Mq.Zmq.ClrZmq.MqBuilder;

namespace Sers.Core.Extensions
{
    public static partial class MqMngExtensions
    {

        public static void UseZmq(this ClientMqMng mqMng)
        {
            if (null == mqMng) return;

            mqMng.AddMqBuilder(new MqClientBuilder());

        }


        public static void UseZmq(this ServerMqMng mqMng)
        {
            if (null == mqMng) return;

            mqMng.AddMqBuilder(new MqServerBuilder());

        }


    }
}
