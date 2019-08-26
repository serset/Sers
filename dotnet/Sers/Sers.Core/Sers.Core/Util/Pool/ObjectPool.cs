using System;
using System.Collections.Concurrent;


namespace Sers.Core.Util.Pool
{
    public class ObjectPool<T>
        where T:new()
    {

        public static ObjectPool<T> Shared = new ObjectPool<T>();



        private ConcurrentBag<T> _objects = new ConcurrentBag<T>();

        public int capi = 10000;
        

        public T Pop()
        {
            //return new T();
            return _objects.TryTake(out var item) ? item : new T();
        }

        public void Push(T item)
        {
            //return;
            if (_objects.Count > capi) return;
            _objects.Add(item);
        }
    }
 
}