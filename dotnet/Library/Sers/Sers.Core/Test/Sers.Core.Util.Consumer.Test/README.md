
#--------------------------------
cd Sers.Core.Util.PubSub.Test
dotnet Sers.Core.Util.PubSub.Test.dll 

启动20个发布者
启动22个订阅者

//disruptor = new Disruptor<Entry>(() => new Entry(), 2 << 18, TaskScheduler.Default, ProducerType.Multi, new BlockingWaitStrategy());   //qps 350万
//disruptor = new Disruptor<Entry>(() => new Entry(), 2 << 18, TaskScheduler.Default, ProducerType.Multi, new SleepingWaitStrategy());   //qps 580万
disruptor = new Disruptor<Entry>(() => new Entry(), 2 << 18, TaskScheduler.Default, ProducerType.Multi, new YieldingWaitStrategy());     //qps 590万
 
Worker_BlockingCollection  //qps 260万

 