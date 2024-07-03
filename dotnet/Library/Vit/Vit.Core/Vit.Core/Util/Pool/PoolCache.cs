using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Vit.Core.Util.Pool
{
    public class PoolCache<T>
    {

        #region 使用队列 ConcurrentQueue       
        /*
        ConcurrentQueue<T> queue = new ConcurrentQueue<T>();


        public int Count => queue.Count;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryTake(out T result) 
        {
            return queue.TryDequeue(out result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T item) 
        {
            queue.Enqueue(item);
        }
        //*/
        #endregion


        #region 使用队列 ConcurrentBag       
        ///*
        ConcurrentBag<T> queue = new ConcurrentBag<T>();


        public int Count => queue.Count;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryTake(out T result)
        {
            return queue.TryTake(out result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T item)
        {
            queue.Add(item);
        }
        //*/
        #endregion


        #region 使用 RingBuffer
        /*

        const int power = 16;

        T[] queue = new T[2<< power];

        int head = 0;
        int tail = 0;

        int count = 0;

        //public int Count => queue.Count;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryTake(out T result)
        {


            return queue.TryTake(out result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T item)
        {
            queue.Add(item);
        }
        //*/
        #endregion






    }
}
