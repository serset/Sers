using System;
using System.Runtime.CompilerServices;

namespace Vit.Core.Util.Pool
{
    public class ObjectPoolGenerator<T>
    {
        public static readonly ObjectPoolGenerator<T> Shared = new ObjectPoolGenerator<T>();


        private PoolCache<T> _objects = new PoolCache<T>();

        public Func<T> objectGenerator;



        /// <summary>
        /// Gets or sets the total number of elements the internal data structure can hold without resizing.(default:100000)
        /// </summary>
        public int Capacity = 2 << 16;


        public ObjectPoolGenerator(Func<T> objectGenerator = null)
        {
            this.objectGenerator = objectGenerator;
            //this.objectGenerator = objectGenerator ?? throw new ArgumentNullException("objectGenerator");          
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Pop()
        {
            //return objectGenerator.Invoke();

            if (_objects.TryTake(out var item)) return item;
            if (objectGenerator != null) return objectGenerator.Invoke();
            return default;
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public T PopOrNull()
        //{
        //    return _objects.TryTake(out var item) ? item : default(T);
        //}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Push(T item)
        {
            if (_objects.Count > Capacity) return;
            _objects.Add(item);
        }
    }

}