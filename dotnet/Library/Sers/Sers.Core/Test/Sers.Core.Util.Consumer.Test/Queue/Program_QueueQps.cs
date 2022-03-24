using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using CLClient.Statistics;
using Sers.Core.Util.PubSub;
using Sers.Core.Util.PubSub.Test.Queue;
using Vit.Core.Module.Log;
using Vit.Core.Util.Pool;

namespace CLClient1
{

    // qps SersQueue    1 thread  1千万
    //                  4 thread  6百万

    // qps  RingBuffer  1 thread  2   千万
    //                  4 thread  2.5 千万
    //                 16 thread  4.7 千万


    //ConcurrentQueue
    //  thead      qps
    //   1,1     5 千万
    //   4,6     7 千万


    //ConcurrentBag
    //  thead      qps(万)
    //   1,1     1700
    //   2,2     180 
    //   8,8     180 

    class Program_QueueQps
    {

        static int pubThreadCount = 8;
        static int subThreadCount = 16;

        static SpinWait spin = new SpinWait();

        static StatisticsQpsInfo qpsPub = new StatisticsQpsInfo();

        static StatisticsQpsInfo qpsSub = new StatisticsQpsInfo();

        static void Mainwww(string[] args)
        {


            Logger.PrintToConsole = true;

            qpsPub.Start(" Pub");
            qpsSub.Start(" Sub");

            for (var t = 0; t < 1; t++)
            {
               Start();
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


        #region SersQueue      
        static void Start_SersQueue()
        {

            var worker = new Queue_Channel<Product>();
 
            int pubThreadCount = 4;
            int subThreadCount = 6;


            for (int i = pubThreadCount; i > 0; i--)
            {
                Task.Run(() =>
                {
                    Product product = new Product();

                    for (; ; )
                    {
                        for (var t = 1; t < 10000; t++)
                        {


                            //product = Product.Pop();
                            //product.ms = 0;
                            //product.IncrementCount = 0;
                             worker.Publish(product);
                   

                        }
                        //product = Product.Pop();
                        //product.ms = 1;
                        //product.IncrementCount = 10000;
                         worker.Publish(product);
               
                        qpsPub.IncrementRequest(10000);

                        //Thread.Sleep(1);

                    }
                });
            }


            for (int i = subThreadCount; i > 0; i--)
            {
                Task.Run(() =>
                {

                    SpinWait spin = new SpinWait();
                    for (; ; )
                    {

                        var p = worker.Take();
                        if (p == null)
                        {
                            spin.SpinOnce();
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

        #endregion

        #region RingBuffer      
        static void Start_RingBuffer()
        { 
            var worker = new RingBuffer<Product>();
    

            int pubThreadCount = 4;
            int subThreadCount = 6;




            for (int i = pubThreadCount; i > 0; i--)
            {
                Task.Run(() =>
                {
                    Product product = new Product();

                    for (; ; )
                    {
                        for (var t = 1; t < 10000; t++)
                        {
                            //product = Product.Pop();
                            //product.ms = 0;
                            //product.IncrementCount = 0;
                            worker.Publish(product);                  

                        }
                        //product = Product.Pop();
                        //product.ms = 1;
                        //product.IncrementCount = 10000;
                        worker.Publish(product);       
                        qpsPub.IncrementRequest(10000);
                        //Thread.Sleep(1);
                    }
                });
            }


            for (int i = subThreadCount; i > 0; i--)
            {
                Task.Run(() =>
                {

                    SpinWait spin = new SpinWait();
                    for (; ; )
                    {



                        var p = worker.Take();
                        if (p == null)
                        {
                            spin.SpinOnce();
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

        #endregion



        #region ConcurrentQueue      
        static void Start2() 
        { 
            var worker = new ConcurrentQueue<Product>();

            int pubThreadCount = 4;
            int subThreadCount = 6;


            for (int i = pubThreadCount; i > 0; i--)
            {
                Task.Run(() =>
                {
                    Product product = new Product();

                    for (; ; )
                    {
                        for (var t = 1; t < 10000; t++)
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


            for (int i = subThreadCount; i > 0; i--)
            {
                Task.Run(() =>
                {

                    SpinWait spin = new SpinWait();
                    for (; ; )
                    {
                        worker.TryDequeue(out var p);


                        //var p = worker.Take();
                        //if (p == null) 
                        //{
                        //    spin.SpinOnce();
                        //}

                        //if (obj.ms > 0)
                        //    Thread.Sleep(obj.ms);


                        //if (obj.IncrementCount > 0)
                        //    qpsSub.IncrementRequest(obj.IncrementCount);

                        //obj.Push();
                    }
                });
            }
        }

        #endregion


        #region ConcurrentBag      
        static void Start()
        {
            var worker = new ConcurrentBag<Product>();



            for (int i = pubThreadCount; i > 0; i--)
            {
                Task.Run(() =>
                {
                    Product product = new Product();

                    for (; ; )
                    {
                        for (var t = 1; t < 10000; t++)
                        { 
                            worker.Add(product);
                        }
             
                        worker.Add(product);
                        qpsPub.IncrementRequest(10000);

                        //spin.SpinOnce();
                        Thread.Sleep(10);

                    }
                });
            }


            for (int i = subThreadCount; i > 0; i--)
            {
                Task.Run(() =>
                {                   
                    for (; ; )
                    {

                        worker.TryTake(out var p);


                      
                        //if (p == null) 
                        //{
                        //    spin.SpinOnce();
                        //}
 
                    }
                });
            }
        }

        #endregion

    }
}
