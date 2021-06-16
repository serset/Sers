using System;

namespace Sers.Core.Util.Consumer
{
    public interface IConsumer<T>
    {

        bool IsRunning { get; }

        int workThreadCount { get; set; }

        string name { get; set; }

        Action<T> processor { get; set; }



        void Publish(T t);


        void Start();

        void Stop();
    }
}
