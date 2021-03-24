using System;
using Disruptor;
using System.Threading.Tasks;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Sers.Core.Util.Consumer
{
    /// <summary>
    /// qps : 400万   producer:16    consumer:16
    /// 
    /// qps 1200万 - 2000万   1个对象，producer:2    consumer:2
    /// qps 8000万 - 10000万 16个对象，producer:2    consumer:2
    /// 参考 https://www.cnblogs.com/hda37210/p/5242185.html
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Consumer_WorkerPool<T> : IConsumer<T>
    {

        /// <summary>
        /// the size of the ring buffer, must be power of 2
        /// </summary>
        public static int defaultBufferSize = 2 << 18;

        public string name { get; set; }


        public int workThreadCount { get; set; } = 16;

        public Action<T> processor { get; set; }



        public Action<T>[] processorList { get; set; }


        /// <summary>
        /// the size of the ring buffer, must be power of 2
        /// </summary>
        public int ringBufferSize { get; set; } = defaultBufferSize;



        /// <summary>
        /// 产品
        /// </summary>
        public class Entry
        {
            public T data;
        }

        /// <summary>
        /// 消费处理对象
        /// </summary>
        public class WorkHandler : IWorkHandler<Entry>
        {
            Action<T> processor;
            public WorkHandler(Action<T> processor)
            {
                this.processor = processor;
            }


            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void OnEvent(Entry entry)
            {
                processor(entry.data);
            }
        }



        private RingBuffer<Entry> _ringBuffer;

        private Disruptor.WorkerPool<Entry> _workerPool;

        // static IWaitStrategy waitStrategy => new YieldingWaitStrategy();  //qps 1线程：2100万 2线程：1200万 4线程：500万
        static IWaitStrategy waitStrategy => new SpinWaitWaitStrategy();    //qps 1线程：2400万 2线程：1200万 4线程：500万


        public bool IsRunning { get; private set; } = false;

        public void Start()
        {
            if (IsRunning) return;
            IsRunning = true;


            IWorkHandler<Entry>[] handers;

            if (processorList != null)
            {
                handers = processorList.Select(processorItem => new WorkHandler(processorItem)).ToArray();
            }
            else 
            {
                handers = System.Linq.Enumerable.Range(0, workThreadCount).Select(i => new WorkHandler(processor)).ToArray();                
            }            


            if (handers.Length == 1)
            {
                _ringBuffer = RingBuffer<Entry>.CreateSingleProducer(() => new Entry(), ringBufferSize, waitStrategy);
            }
            else
            {
                _ringBuffer = RingBuffer<Entry>.CreateMultiProducer(() => new Entry(), ringBufferSize, waitStrategy);
            }

            _workerPool = new WorkerPool<Entry>(_ringBuffer
                , _ringBuffer.NewBarrier()
                , new FatalExceptionHandler()
                , handers);

            _ringBuffer.AddGatingSequences(_workerPool.GetWorkerSequences());


            _workerPool.Start(new Disruptor.Dsl.BasicExecutor(TaskScheduler.Default));
        }


        public void Stop()
        {
            if (!IsRunning) return;
            IsRunning = false;

            _workerPool.DrainAndHalt();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Publish(T data)
        {
            long sequence = _ringBuffer.Next();
            var product = _ringBuffer[sequence];

            product.data = data;

            _ringBuffer.Publish(sequence);
        }






    }
}
