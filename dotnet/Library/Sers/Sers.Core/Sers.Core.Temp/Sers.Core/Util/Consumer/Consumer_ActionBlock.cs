using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks.Dataflow;

namespace Sers.Core.Util.Consumer
{
    /// <summary>
    ///  数据流（任务并行库 TPL） 参考 https://blog.csdn.net/weixin_33697898/article/details/89700239
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Consumer_ActionBlock<T>: IConsumer<T>
    {

        public int workThreadCount { get; set; } = 16;

        public string name { get; set; }

        public Action<T> processor { get; set; }


        ActionBlock<T> abAsync=null;

        public bool IsRunning { get; private set; } = false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Publish(T t) 
        {
            abAsync.SendAsync(t);
        }


        public void Start()
        {
            if (IsRunning) return;

            IsRunning = true;

            abAsync = new ActionBlock<T>(processor,
         new ExecutionDataflowBlockOptions() {
             MaxDegreeOfParallelism = workThreadCount 
             //,BoundedCapacity=2<<18
             ,
         });


        }

        public void Stop() 
        {
            if (!IsRunning) return;
            IsRunning = false;

            abAsync = null;
        }


      

    }
}
