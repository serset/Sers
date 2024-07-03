using Newtonsoft.Json.Linq;

using Sers.Core.Util.Consumer.Mode;

namespace Sers.Core.Util.Consumer
{
    public class ConsumerFactory
    {
        public static IConsumer<T> CreateConsumer<T>(JObject config = null)
        {
            if (config == null) config = new JObject();

            IConsumer<T> consumer;
            switch (config["mode"]?.ToString())
            {
                case "LongThread":
                    consumer = new LongThread<T>();
                    break;
                case "LongThread_TimeLimit":
                    consumer = new LongThread_TimeLimit<T>();
                    break;
                case "ConsumerCascade":
                    consumer = new ConsumerCascade<T, LongThread<T>>();
                    break;
                case "ManagedThread":
                    consumer = new ManagedThread<T>();
                    break;
                default:
                    consumer = new LongThread<T>();
                    break;
            }

            consumer.Init(config);

            return consumer;
        }
    }
}
