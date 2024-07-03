using System;

using Newtonsoft.Json.Linq;

using Vit.Core.Util.Threading.Worker;

namespace Sers.Core.Util.Consumer
{
    public interface IConsumer<T>
    {

        bool isRunning { get; }

        int threadCount { get; set; }

        string threadName { get; set; }

        Action<T> Processor { get; set; }

        Action<ETaskFinishStatus, T> OnFinish { get; set; }

        void Init(JObject config);


        void Publish(T t);


        void Start();

        void Stop();
    }
}
