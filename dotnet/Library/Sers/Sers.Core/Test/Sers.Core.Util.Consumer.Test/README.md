
#--------------------------------
 
dotnet /root/app/publish/Sers.Core.Util.Consumer.Test.dll  8 16 BufferBlock

启动20个发布者
启动22个订阅者

//disruptor = new Disruptor<Entry>(() => new Entry(), 2 << 18, TaskScheduler.Default, ProducerType.Multi, new BlockingWaitStrategy());   //qps 350万
//disruptor = new Disruptor<Entry>(() => new Entry(), 2 << 18, TaskScheduler.Default, ProducerType.Multi, new SleepingWaitStrategy());   //qps 580万
disruptor = new Disruptor<Entry>(() => new Entry(), 2 << 18, TaskScheduler.Default, ProducerType.Multi, new YieldingWaitStrategy());     //qps 590万
 
Worker_BlockingCollection  //qps 260万

 
---------------------------------------------------------------------------------

        public static IConsumer<T> CreateConsumer<T>() 
        {

            //return new ConsumerCache<T, Consumer_BlockingCollection<T>>();
            IConsumer<T> consumer;
            switch (ConsumerMode)
            {
                //case "ActionBlock":
                //    consumer = new Consumer_ActionBlock<T>();  // 16 16 700万     24 24 900-1000万
                //    break;
                //case "BufferBlock":
                //    consumer = new Consumer_BufferBlock<T>();   //2 36 800-1000万
                //    break;
                case "BlockingCollection":
                    consumer = new Consumer_BlockingCollection<T>();  //16 16 440万          2  2  800万
                    break;


                //case "Disruptor":
                //    consumer = new Consumer_Disruptor<T>(); // 16 16 800万
                //    break;
                //case "WorkerPool":
                //    consumer = new Consumer_WorkerPool<T>(); //16 16 800-900万
                //    break;

                //case "ConsumerCache_ActionBlock":
                //    consumer = new ConsumerCache<T, Consumer_ActionBlock<T>>(); // 16 16  4000-4200万
                //    break;
                //case "ConsumerCache_BufferBlock":
                //    consumer = new ConsumerCache<T, Consumer_BufferBlock<T>>(); // 16 16  1500-1600万
                //    break;
                case "ConsumerCache_BlockingCollection":
                    consumer = new ConsumerCache<T, Consumer_BlockingCollection<T>>(); //16 16 4200-4500万
                    break;



                //case "ConsumerCache_WorkerPool":
                //    consumer = new ConsumerCache<T, Consumer_WorkerPool<T>>(); //750万  异常
                //    break;
                //case "WorkerPoolCache":
                //    consumer = new Consumer_WorkerPoolCache<T>();        //940万 异常
                //    break;
                //case "WorkerPoolCascade":
                //    consumer = new Consumer_WorkerPoolCascade<T>();   //1400万 异常
                //    break;

                default:
                    consumer = new Consumer_BlockingCollection<T>();
                    break;
            }

            return consumer;
        }