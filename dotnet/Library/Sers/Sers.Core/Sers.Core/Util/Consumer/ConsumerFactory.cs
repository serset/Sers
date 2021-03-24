using Vit.Core.Util.ConfigurationManager;

namespace Sers.Core.Util.Consumer
{
    public class ConsumerFactory
    {

        /// <summary>
        /// 队列类型。可为 BlockingCollection(默认)、 ConsumerCache_BlockingCollection(高性能) 、ActionBlock、BufferBlock 
        /// </summary>
        public static string consumerType = ConfigurationManager.Instance.GetByPath<string>("Vit.ConsumerType");

        public static IConsumer<T> CreateConsumer<T>() 
        {

            //return new ConsumerCache<T, Consumer_BlockingCollection<T>>();
            IConsumer<T> consumer;
            switch (consumerType)
            {
                case "ActionBlock":
                    consumer = new Consumer_ActionBlock<T>();  // 16 16 700万     24 24 900-1000万
                    break;
                case "BufferBlock":
                    consumer = new Consumer_BufferBlock<T>();   //2 36 800-1000万
                    break;
                case "BlockingCollection":
                    consumer = new Consumer_BlockingCollection<T>();  //16 16 440万          2  2  800万
                    break;


                case "Disruptor":
                    consumer = new Consumer_Disruptor<T>(); // 16 16 800万
                    break;
                case "WorkerPool":
                    consumer = new Consumer_WorkerPool<T>(); //16 16 800-900万
                    break;

                case "ConsumerCache_ActionBlock":
                    consumer = new ConsumerCache<T, Consumer_ActionBlock<T>>(); // 16 16  4000-4200万
                    break;
                case "ConsumerCache_BufferBlock":
                    consumer = new ConsumerCache<T, Consumer_BufferBlock<T>>(); // 16 16  1500-1600万
                    break;
                case "ConsumerCache_BlockingCollection":
                    consumer = new ConsumerCache<T, Consumer_BlockingCollection<T>>(); //16 16 4200-4500万
                    break;



                case "ConsumerCache_WorkerPool":
                    consumer = new ConsumerCache<T, Consumer_WorkerPool<T>>(); //750万  异常
                    break;
                case "WorkerPoolCache":
                    consumer = new Consumer_WorkerPoolCache<T>();        //940万 异常
                    break;
                case "WorkerPoolCascade":
                    consumer = new Consumer_WorkerPoolCascade<T>();   //1400万 异常
                    break;

                default:
                    consumer = new Consumer_BlockingCollection<T>();
                    break;
            }

            return consumer;
        }
    }
}
