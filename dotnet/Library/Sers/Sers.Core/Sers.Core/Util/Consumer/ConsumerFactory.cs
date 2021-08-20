using Newtonsoft.Json.Linq;

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
                case "BlockingCollection":
                    consumer = new LongTask<T>();  //16 16 440万          2  2  800万
                    break;
                case "ConsumerCache_BlockingCollection":
                    consumer = new ConsumerCascade<T, LongTask<T>>(); //16 16 4200-4500万
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
