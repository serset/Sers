using Disruptor;
using Disruptor.Dsl;
using System;
using System.Threading.Tasks;

namespace Sers.Core.Util.Consumer
{


    // https://www.jianshu.com/p/6232d81581ff

    // https://www.cnblogs.com/duanxz/archive/2013/01/23/2872513.html

    // WaitStrategy消费者等待策略
    // https://www.imooc.com/article/259253


    /// <summary>
    /// qps 基本同 Consumer_WorkerPool
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Consumer_Disruptor<T> : IConsumer<T>
        where T: class,new()
    {
        public int workThreadCount { get; set; } = 16;

        public string name { get; set; }

        public Action<T> processor { get; set; }


        /// <summary>
        /// the size of the ring buffer, must be power of 2
        /// </summary>
        public int ringBufferSize { get; set; } = 2 << 22;



        public void Publish(T data)
        {
            long index = _ringBuffer.Next();
            var entity = _ringBuffer[index];
            entity.data = data;
            _ringBuffer.Publish(index);  
        }

        class Entry
        {
            public T data;
        }
        class WorkHandler : IWorkHandler<Entry>
        {
             Action<T> processor;
            public WorkHandler(Action<T> processor) 
            {
                this.processor = processor;
            }

            public void OnEvent(Entry entry)
            {
                processor(entry.data);       
            }
        }

        // static IWaitStrategy waitStrategy => new BlockingWaitStrategy();      //qps 1线程： 380万 2线程： 420万
        // static IWaitStrategy waitStrategy => new SleepingWaitStrategy();      //qps 1线程：1100万 2线程：1200万
        //static IWaitStrategy waitStrategy => new YieldingWaitStrategy();      //qps 1线程：1100万 2线程：1100万
        static IWaitStrategy waitStrategy => new SpinWaitWaitStrategy();        //qps 1线程：1500万 2线程：1200万



        public void Start()
        {           
            disruptor = new Disruptor<Entry>(() => new Entry(), ringBufferSize, TaskScheduler.Default, ProducerType.Multi, waitStrategy);

            IWorkHandler<Entry>[] workerPool = new IWorkHandler<Entry>[workThreadCount];
            for (var t = 0; t < workThreadCount; t++)
            {
                workerPool[t]=new WorkHandler(processor);
            } 
            disruptor.HandleEventsWithWorkerPool(workerPool);

            _ringBuffer = disruptor.Start();
        }

        Disruptor<Entry> disruptor;
        RingBuffer<Entry> _ringBuffer;

        public void Stop()
        {
            disruptor?.Shutdown();
            disruptor = null;
            _ringBuffer = null;
        }





    }
}
