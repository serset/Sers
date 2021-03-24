using System;

namespace Sers.Core.Util.Consumer
{
    public class ConsumerFactory
    {

        public static IConsumer<T> CreateConsumer<T>() 
        {
            //return new Consumer_BlockingCollection<T>();
            return new ConsumerCache<T, Consumer_BlockingCollection<T>>();
        }
    }
}
