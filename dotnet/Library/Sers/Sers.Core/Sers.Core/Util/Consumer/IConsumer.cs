using Newtonsoft.Json.Linq;
using System;

namespace Sers.Core.Util.Consumer
{
    public interface IConsumer<T>
    {

        bool isRunning { get; }

        int threadCount { get; set; }

        string threadName { get; set; }

        Action<T> processor { get; set; }

        Action<T> OnFinish { get; set; }
        Action<T> OnTimeout { get; set; }

        void Init(JObject config);


        void Publish(T t);


        void Start();

        void Stop();
    }
}
