using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Vit.Core.Util.Pool
{
    public class PoolCache<T>
    {

        #region 使用队列 ConcurrentQueue       
        ///*
        ConcurrentQueue<T> queue = new ConcurrentQueue<T>();


        public int Count => queue.Count;

        public bool TryTake(out T result) 
        {
            return queue.TryDequeue(out result);
        }

        public void Add(T item) 
        {
            queue.Enqueue(item);
        }
        //*/
        #endregion


        #region 使用队列 ConcurrentBag       
        /*
        ConcurrentBag<T> queue = new ConcurrentBag<T>();


        public int Count => queue.Count;

        public bool TryTake(out T result) 
        {
            return queue.TryTake(out result);
        }

        public void Add(T item) 
        {
            queue.Add(item);
        }
        //*/
        #endregion








    }
}
