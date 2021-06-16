using System;
using System.Threading;
using System.Threading.Tasks;
using CLClient.Statistics;
using Sers.Core.Util.Consumer;
using Vit.Core.Module.Log;

namespace CLClient
{
    class ProgramQps
    {

        static int producerThreadCount = 8;
        static int consumerThreadCount = 16;
        public static string consumerType = "ActionBlock";


        static StatisticsQpsInfo qpsPub = new StatisticsQpsInfo();

        static StatisticsQpsInfo qpsSub = new StatisticsQpsInfo();

        static void Main(string[] args)
        {             

            Logger.OnLog = (level, msg) => { Console.Write("[" + level + "]" + msg); };

            qpsPub.Start(" Pub");
            qpsSub.Start("                                                    Sub");


            if (args != null)
            {
                if (args.Length >= 1)
                {
                    int.TryParse(args[0], out producerThreadCount);
                }

                if (args.Length >= 2)
                {
                    int.TryParse(args[1], out consumerThreadCount);
                }
                if (args.Length >= 3)
                {
                    consumerType = args[2];
                }
            }


            for (var t = 0; t < 1; t++)
            {
                new ProgramQps() {  }.Start();
            }

            while (true)
            {
                Thread.Sleep(5000);
            }
        }



        class Product 
        {     

            public int ms = 0;
            public int IncrementCount = 0;
        }


        IConsumer<Product> consumer;
    
        public void Start()
        {
            switch (consumerType) 
            {
                case "ActionBlock":
                    consumer = new Consumer_ActionBlock<Product>();  // 16 16 700万     24 24 900-1000万
                    break;
                case "BufferBlock":
                    consumer = new Consumer_BufferBlock<Product>();   //2 36 800-1000万
                    break;
                case "BlockingCollection":
                    consumer = new Consumer_BlockingCollection<Product>();  //16 16 440万          2  2  800万
                    break;
              
  
                //case "Disruptor":
                //    consumer = new Consumer_Disruptor<Product>(); // 16 16 800万
                //    break;
                //case "WorkerPool":
                //    consumer = new Consumer_WorkerPool<Product>(); //16 16 800-900万
                //    break;

                case "ConsumerCache_ActionBlock":
                    consumer = new ConsumerCache<Product, Consumer_ActionBlock<Product>>(); // 16 16  4000-4200万
                    break;
                case "ConsumerCache_BufferBlock":
                    consumer = new ConsumerCache<Product, Consumer_BufferBlock<Product>>(); // 16 16  1500-1600万
                    break;
                case "ConsumerCache_BlockingCollection":
                    consumer = new ConsumerCache<Product, Consumer_BlockingCollection<Product>>(); //16 16 4200-4500万
                    break;



                //case "ConsumerCache_WorkerPool":
                //    consumer = new ConsumerCache<Product, Consumer_WorkerPool<Product>>(); //750万  异常
                //    break;
                //case "WorkerPoolCache":
                //    consumer = new Consumer_WorkerPoolCache<Product>();        //940万 异常
                //    break;
                //case "WorkerPoolCascade":
                //    consumer = new Consumer_WorkerPoolCascade<Product>();   //1400万 异常
                //    break;

                default:
                    consumer= ConsumerFactory.CreateConsumer<Product>(); 
                    break;
            }


            Console.WriteLine("consumer: "+ consumer.GetType().FullName);       



            consumer.processor = Processor;
            consumer.workThreadCount = consumerThreadCount;
            consumer.Start();


            for (int i = producerThreadCount; i > 0; i--)
            {
                StartThreadPublish();
            }
        }


  

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        void Processor(Product obj) 
        {
            if (obj.ms > 0)
                Thread.Sleep(obj.ms);


            if (obj.IncrementCount > 0)
                qpsSub.IncrementRequest(); 
        }


        /// <summary>
        ///  
        /// </summary>
        void StartThreadPublish()
        {
            Task.Run(() =>
            {
                Product product;

                for (int i = 0; i < int.MaxValue; i++)
                {
                    for (var t = 1; t <1000; t++)
                    {
                        //new Product { ms = t == 0 ? 1 : 0 };
                        //if (t % 100 == 0) qpsPub.IncrementRequest(100);

                        product = new Product();
                        product.ms = 0;
                        product.IncrementCount = 0;
                        consumer.Publish(product);

                    }
                    product = new Product();
                    product.ms = 1;
                    product.IncrementCount = 1;
                    consumer.Publish(product);
                    qpsPub.IncrementRequest();
                    //qpsPub.IncrementRequest(1);

                    Thread.Sleep(1);

                }
            });
        }
    }
}
