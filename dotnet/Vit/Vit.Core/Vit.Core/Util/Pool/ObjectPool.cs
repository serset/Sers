using System.Collections.Concurrent;

namespace Vit.Core.Util.Pool
{
    public class ObjectPool<T>
        where T:new()
    {

        public static ObjectPool<T> Shared = new ObjectPool<T>();


        private ConcurrentBag<T> _objects = new ConcurrentBag<T>();

        /// <summary>
        /// Gets or sets the total number of elements the internal data structure can hold without resizing.(default:100000)
        /// </summary>
        public int Capacity = 100000;
        

        public T Pop()
        {
            //return new T();
            return _objects.TryTake(out var item) ? item : new T();
        }

        public T PopOrNull()
        {
            return _objects.TryTake(out var item) ? item : default(T);
        }

        public void Push(T item)
        {
            if (_objects.Count > Capacity) return;
            _objects.Add(item);
        }
    }
 
}