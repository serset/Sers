using System;
using System.Collections.Concurrent;


namespace Sers.Core.Util.Pool
{
    public class ObjectPoolGenerator<T>       
    {
        private ConcurrentBag<T> _objects;
        private Func<T> objectGenerator;

        public ObjectPoolGenerator(Func<T> objectGenerator)
        {
            this.objectGenerator = objectGenerator ?? throw new ArgumentNullException("objectGenerator");
            _objects = new ConcurrentBag<T>();
        }

        public T Pop()
        {
            return _objects.TryTake(out var item) ? item : objectGenerator();
        }

        public void Push(T item)
        { 
            _objects.Add(item);
        }
    }
 
}