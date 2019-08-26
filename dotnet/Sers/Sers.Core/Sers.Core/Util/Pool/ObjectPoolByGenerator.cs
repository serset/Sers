using System;
using System.Collections.Concurrent;


namespace Sers.Core.Util.Pool
{
    public class ObjectPoolGenerator<T>       
    {
        public static ObjectPoolGenerator<T> Shared = new ObjectPoolGenerator<T>();


        private ConcurrentBag<T> _objects;

        public Func<T> objectGenerator;

        public ObjectPoolGenerator(Func<T> objectGenerator=null)
        {
            this.objectGenerator = objectGenerator;
            //this.objectGenerator = objectGenerator ?? throw new ArgumentNullException("objectGenerator");
            _objects = new ConcurrentBag<T>();
        }

        public T Pop()
        {
            if (_objects.TryTake(out var item)) return item;
            if(objectGenerator!=null) return objectGenerator.Invoke();
            return default(T);
        }
        public T PopOrNull()
        {
            return _objects.TryTake(out var item) ? item : default(T);
        }

        public void Push(T item)
        { 
            _objects.Add(item);
        }
    }
 
}