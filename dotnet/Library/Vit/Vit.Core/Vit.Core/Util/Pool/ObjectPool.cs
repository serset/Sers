﻿
using System.Runtime.CompilerServices;

namespace Vit.Core.Util.Pool
{
    public class ObjectPool<T>
        where T : class, new()
    {

        public static readonly ObjectPool<T> Shared = new ObjectPool<T>();



        private PoolCache<T> _objects = new PoolCache<T>();

        /// <summary>
        /// Gets or sets the total number of elements the internal data structure can hold without resizing.(default:100000)
        /// </summary>
        public int Capacity = 2 << 16;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Pop()
        {
            //return new T();
            return _objects.TryTake(out var item) ? item : new T();
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public T PopOrNull()
        //{
        //    return _objects.TryTake(out var item) ? item : null;
        //}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Push(T item)
        {
            if (_objects.Count > Capacity) return;
            _objects.Add(item);
        }
    }

}