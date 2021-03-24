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

        static StatisticsQpsInfo qpsPub = new StatisticsQpsInfo();

        static StatisticsQpsInfo qpsSub = new StatisticsQpsInfo();

        static void Main(string[] args)
        {             

            Logger.OnLog = (level, msg) => { Console.Write("[" + level + "]" + msg); };

            qpsPub.Start(" Pub");
            qpsSub.Start(" Sub");

            for (var t = 0; t < 1; t++)
            {
                new ProgramQps() { name = "" + t }.Start();
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
        string name;
        public void Start() 
        {
            // consumer = new Consumer_BlockingCollection<Product>();  //260万
            //consumer = new ConsumerCache<Product,Consumer_BlockingCollection<Product>>(); //300万

            consumer = new Consumer_Disruptor<Product>(); //600-650万
            //consumer = new ConsumerCache<Product, Consumer_Disruptor<Product>>(); //370-400万

            //consumer = new Consumer_WorkerPool<Product>(); //640-680万
            //consumer = new ConsumerCache<Product, Consumer_WorkerPool<Product>>(); //750万  异常

            //consumer = new Consumer_WorkerPoolCache<Product>();        //940万 异常

           // consumer = new Consumer_WorkerPoolCascade<Product>();   //1400万



            consumer.processor = Processor;
            consumer.workThreadCount = consumerThreadCount;
            consumer.Start();


            for (int i = producerThreadCount; i > 0; i--)
            {
                StartThreadPublish();
            }
        }


        static int producerThreadCount = 16;
        static int consumerThreadCount = 16;

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
                    for (var t = 1; t <10000; t++)
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
