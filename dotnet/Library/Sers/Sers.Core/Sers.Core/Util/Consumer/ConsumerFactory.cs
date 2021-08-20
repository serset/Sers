using Newtonsoft.Json.Linq;
using Sers.Core.Util.Consumer.Mode;

namespace Sers.Core.Util.Consumer
{
    public class ConsumerFactory
    {
        public static IConsumer<T> CreateConsumer<T>(JObject config) 
        {
            if (config == null) config = new JObject();

            IConsumer<T> consumer;
            switch (config["mode"].ToString())
            {
                case "AsyncTask":
                    consumer = new AsyncTask<T>();
                    break;
                case "ConsumerCascade":
                    consumer = new ConsumerCascade<T, LongTask<T>>();
                    break;
                case "LongTask":
                    consumer = new LongTask<T>();
                    break;
                case "LongTask_TimeLimit":
                    consumer = new LongTask_TimeLimit<T>();
                    break;
                default:
                    consumer = new LongTask<T>();
                    break;
            }

            consumer.Init(config);

            return consumer;
        }
    }
}
