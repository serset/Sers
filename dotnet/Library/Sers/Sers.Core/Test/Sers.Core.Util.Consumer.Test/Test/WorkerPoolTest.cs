using System;
using System.Collections.Generic;
using Disruptor;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Sers.Core.Util.PubSub.Test.Test
{

    // 参考 https://www.cnblogs.com/hda37210/p/5242185.html



    /// <summary>
    /// 消费者管理器
    /// </summary>
    /// <typeparam name="TProduct">产品</typeparam>
    public class Workers<TProduct> where TProduct : Producer<TProduct>, new()
    {

       

        private Disruptor.WorkerPool<TProduct> _workerPool;

        public Workers(List<IWorkHandler<TProduct>> handers, IWaitStrategy waitStrategy = null, int bufferSize = /*Test.repeat **/ (2<<22)) //1024 * 64
        {

            if (handers == null || handers.Count == 0)
                throw new ArgumentNullException("消费事件处理数组为空!");

            if (handers.Count == 1)
                _ringBuffer = RingBuffer<TProduct>.CreateSingleProducer(() => new TProduct(), bufferSize,
                    waitStrategy ?? new SpinWaitWaitStrategy());
            else
            {
                _ringBuffer = RingBuffer<TProduct>.CreateMultiProducer(() => new TProduct(), bufferSize,
                    waitStrategy ?? new SpinWaitWaitStrategy());
            }

            _workerPool = new WorkerPool<TProduct>(_ringBuffer
                , _ringBuffer.NewBarrier()
                , new FatalExceptionHandler()
                , handers.ToArray());

            _ringBuffer.AddGatingSequences(_workerPool.GetWorkerSequences());
        }

        public void Start()
        {
            _workerPool.Start(new Disruptor.Dsl.BasicExecutor(TaskScheduler.Default));
        }

        public Producer<TProduct> CreateOneProducer()
        {
            return new Producer<TProduct>(this._ringBuffer);
        }
        public void DrainAndHalt()
        {
            _workerPool.DrainAndHalt();
        }

        private readonly RingBuffer<TProduct> _ringBuffer;
    }

    /// <summary>
    /// 生产者对象
    /// </summary>
    /// <typeparam name="TProduct">产品类型</typeparam>
    public class Producer<TProduct> where TProduct : Producer<TProduct>
    {

        long _sequence;
        private RingBuffer<TProduct> _ringBuffer;
        public Producer()
        {

        }
        public Producer(RingBuffer<TProduct> ringBuffer)
        {
            _ringBuffer = ringBuffer;
        }
        /// <summary>
        /// 获取可修改的产品
        /// </summary>
        /// <returns></returns>
        public Producer<TProduct> Enqueue()
        {
            long sequence = _ringBuffer.Next();
            Producer<TProduct> producer = _ringBuffer[sequence];
            producer._sequence = sequence;
            if (producer._ringBuffer == null)
                producer._ringBuffer = _ringBuffer;
            return producer;
        }
        /// <summary>
        /// 提交产品修改
        /// </summary>
        public void Commit()
        {
            _ringBuffer.Publish(_sequence);
        }
    }

    /// <summary>
    /// 产品/继承生产者
    /// </summary>
    public class Product : Producer<Product>
    {
        //产品包含的属下随便定义,无要求,只需要继承自生产者就行了
        public long Value { get; set; }
        public string Guid { get; set; }
    }


    /// <summary>
    /// 消费处理对象
    /// </summary>
    public class WorkHandler : IWorkHandler<Product>
    {
        public Test test;
        public void OnEvent(Product @event)
        {

            test.UpdateCacheByOut(@event.Guid);
            //收到产品,在这里写处理代码

        }

    }



    public class Test
    {
        public const int repeat = 1;



        public static  long PrePkgInCount = 0;
        public static  long PrePkgOutCount = 0;
        public static  long PkgInCount = 0;
        public static  long PkgOutCount = 0;
          ConcurrentDictionary<string, string> InCache = new ConcurrentDictionary<string, string>();
          ConcurrentDictionary<string, string> OutCache = new ConcurrentDictionary<string, string>();
        private static long Seconds;
        static bool qpsStarted = false;


        //static int threadCount = 16 * 4;
        static int threadCount = repeat * 2;

        public void Main1()
        {
            //Workers<Product> workers = new Workers<Product>(
            //new List<IWorkHandler<Product>>() { new WorkHandler() { test=this}, new WorkHandler() { test = this } });

            Workers<Product> workers = new Workers<Product>(
            Enumerable.Range(1, 2).Select(x =>(IWorkHandler<Product>) new WorkHandler() { test = this }).ToList());


            var producerWorkerList = Enumerable.Range(1, threadCount).Select(x => workers.CreateOneProducer()).ToArray(); 


            //Producer<Product> producerWorkers = workers.CreateOneProducer();
            //Producer<Product> producerWorkers1 = workers.CreateOneProducer();

            workers.Start();
            if (!qpsStarted)
            {
                qpsStarted = true;
                Task.Run(delegate
                {
                    while (true)
                    {
                        Thread.Sleep(1000);
                        Seconds++;
                        long intemp = PkgInCount;
                        long outemp = PkgOutCount;
                        Console.WriteLine(
                            $"In ops={intemp - PrePkgInCount},out ops={outemp - PrePkgOutCount},inCacheCount={InCache.Count},OutCacheCount={OutCache.Count},RunningTime={Seconds}");
                        PrePkgInCount = intemp;
                        PrePkgOutCount = outemp;
                    }

                });
            }

            producerWorkerList.ToList().ForEach(w=> {

                Task.Run(delegate { Run(w); });
            }) ;

            //Task.Run(delegate { Run(producerWorkers); });
            //Task.Run(delegate { Run(producerWorkers); });
            //Task.Run(delegate { Run(producerWorkers1); });
            Console.Read();

        }

        public   void Run(Producer<Product> producer)
        {
            for (int i = 0; i < int.MaxValue; i++)
            {

                var obj = producer.Enqueue();
                CheckRelease(obj as Product);
                obj.Commit();
            }
        }

        public   void CheckRelease(Product publisher)
        {
            //Interlocked.Increment(ref PkgInCount);
            return; //不检查正确性
            publisher.Guid = Guid.NewGuid().ToString();
            InCache.TryAdd(publisher.Guid, string.Empty);
        }

        long count = 0; 

        public   void UpdateCacheByOut(string guid)
        {
            if (Interlocked.Increment(ref count) % 10000 == 9999)
            {
                Interlocked.Add(ref PkgOutCount, 10000);
            }

            return;
            //Interlocked.Increment(ref PkgOutCount);
    
            if (guid != null)
                if (InCache.ContainsKey(guid))
                {
                    string str;
                    InCache.TryRemove(guid, out str);
                }
                else
                {
                    OutCache.TryAdd(guid, string.Empty);
                }

        }

    }
}
