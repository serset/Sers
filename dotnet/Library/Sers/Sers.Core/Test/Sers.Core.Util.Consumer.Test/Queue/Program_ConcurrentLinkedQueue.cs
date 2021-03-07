using System;
using System.Threading;
using System.Threading.Tasks;
using CLClient.Statistics;
using Sers.Core.Util.PubSub;
using Sers.Core.Util.PubSub.Test.Queue;
using Vit.Core.Module.Log;
using Vit.Core.Util.Pool;

namespace CLClient1
{
    // qps   1,4  600万
    class ConcurrentLinkedQueue
    {

        static StatisticsQpsInfo qpsPub = new StatisticsQpsInfo();

        static StatisticsQpsInfo qpsSub = new StatisticsQpsInfo();

        static void Main11(string[] args)
        {
             

            Logger.OnLog = (level, msg) => { Console.Write("[" + level + "]" + msg); };

            qpsPub.Start(" Pub");
            //qpsSub.Start(" Sub");

            for (var t = 0; t < 1; t++)
            {
                new ConcurrentLinkedQueue() { name = "" + t }.Start();
            }

            while (true)
            {
                Thread.Sleep(5000);
            }
        }



        class Product 
        {
            public static Product Pop()
            {
                return new Product();
                //return ObjectPool<Product>.Shared.Pop();
            }

            /// <summary>
            /// 使用结束请手动调用
            /// </summary>
            public void Push()
            {
                //ObjectPool<Product>.Shared.Push(this);
            }

            public int ms = 0;
            public int IncrementCount = 0;
        }


        ConcurrentLinkedQueue<Product> worker;
        string name;
        public void Start() 
        {
    
           
            worker = new ConcurrentLinkedQueue<Product>();
 

 


            for (int i = pubThreadCount; i > 0; i--)
            {
                StartThreadPublish();
            }


            for (int i = subThreadCount; i > 0; i--)
            {
                StartThreadSubscribe();
            }
        }


        static int pubThreadCount =4;
        static int subThreadCount = 4;
      
        void StartThreadPublish()
        {
            Task.Run(() =>
            {
                Product product=new Product();

                for (int i = 0; i < int.MaxValue; i++)
                {
                    for (var t = 1; t <10000; t++)
                    {


                        //product = Product.Pop();
                        //product.ms = 0;
                        //product.IncrementCount = 0;
                        //worker.Publish(product);
                        worker.Enqueue(product);

                    }
                    //product = Product.Pop();
                    //product.ms = 1;
                    //product.IncrementCount = 10000;
                    //worker.Publish(product);         
                    worker.Enqueue(product);
                    qpsPub.IncrementRequest(10000);
                   
                    //Thread.Sleep(1);

                }
            });
        }


        void StartThreadSubscribe()
        {
            Task.Run(() =>
            {

                SpinWait spin = new SpinWait();
                for (int i = 0; i < int.MaxValue; i++)
                {
                    worker.TryDequeue(out var product);
     

                    if (product == null) 
                    {
                        //spin.SpinOnce();
                    }

                    //if (obj.ms > 0)
                    //    Thread.Sleep(obj.ms);


                    //if (obj.IncrementCount > 0)
                    //    qpsSub.IncrementRequest(obj.IncrementCount);

                    //obj.Push();
                }
            });
        }
    }
}
