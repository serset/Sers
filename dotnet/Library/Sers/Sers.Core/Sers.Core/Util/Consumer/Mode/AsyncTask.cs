using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Vit.Core.Util.Threading;

namespace Sers.Core.Util.Consumer.Mode
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Async<T>: IConsumer<T>
    {

        BlockingCollection<T> queue = new BlockingCollection<T>();
 


        public string name { get; set; }


        public int workThreadCount { get; set; }


        /// <summary>
        ///  超时时间。脉冲间隔。（主动关闭超过此时间的任务,实际任务强制关闭的时间会在1倍超时时间到2倍超时时间内)。单位：ms。(默认300000)
        /// </summary>
        public int timeout_ms { get; set; }

        public Action<T> processor { get; set; }
        public Action<T> OnFinish { get; set; }
        public Action<T> OnTimeout { get; set; }



        public bool IsRunning { get; set; }

        public void Init(JObject config)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Publish(T t)
        {        
            Task.Run(() =>
            {
                try
                {
                    processor(t);
                }
                catch (Exception ex)
                {
                    //throw;
                    OnTimeout?.Invoke(t);
                    return;
                }

                OnFinish?.Invoke(t);

            }, new CancellationTokenSource(timeout_ms).Token);
        }


        public void Start()
        {             
        }

        public void Stop() 
        {   
        }
         

    }
}
