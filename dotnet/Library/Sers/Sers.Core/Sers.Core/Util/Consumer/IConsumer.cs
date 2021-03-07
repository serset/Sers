using System;
using System.Collections.Generic;
using System.Text;

namespace Sers.Core.Util.Consumer
{
    public interface IConsumer<T>
    {
        int workThreadCount { get; set; }

        string name { get; set; }

        Action<T> processor { get; set; }



        void Publish(T t);


        void Start();

        void Stop();
    }
}
