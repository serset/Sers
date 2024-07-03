using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Vit.Extensions;

namespace Sers.Core.Module.Api.RouteMap
{
    public class Tree<T>
        where T : class
    {
        string key;
        public T data;


        SortedDictionary<string, Tree<T>> children = new SortedDictionary<string, Tree<T>>();

        public Tree<T> parent { get; private set; }


        /// <summary>
        /// path demo：  "/station1/fold2/api1"
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Tree<T> BuildPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return this;
            return BuildPath(path.Split('/'), 1);

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Tree<T> BuildPath(string[] path, int index)
        {
            if (path.Length <= index)
            {
                return this;
            }

            if (!children.TryGetValue(path[index], out var tree))
            {
                tree = new Tree<T> { key = path[index], parent = this };

                children.IDictionaryTryAdd(tree.key, tree);
            }
            return tree.BuildPath(path, index + 1);
        }

        /// <summary>
        /// path demo：  "/station1/fold2/api1/action2"
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T QueryByPath(string path)
        {
            return QueryByPath(path.Split('/'), 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        T QueryByPath(string[] path, int index)
        {
            if (path.Length <= index) return data;

            if (children.TryGetValue(path[index], out var value))
            {
                return value.QueryByPath(path, index + 1) ?? data;
            }
            return data;
        }

        /// <summary>
        /// path demo：  "/station1/fold2/api1/action2"
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Tree<T> GetChildren(string path)
        {
            return GetChildren(path.Split('/'), 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Tree<T> GetChildren(string[] path, int index)
        {
            if (path.Length <= index) return this;

            if (children.TryGetValue(path[index], out var value))
            {
                return value.GetChildren(path, index + 1);
            }
            return null;
        }


        void TryRemove()
        {
            if (data == null)
            {
                if (children.Count == 0 && parent != null)
                {
                    parent.children.Remove(key);
                    parent.TryRemove();
                }
            }
        }

        /// <summary>
        ///  path demo：  "/station1/fold2/api1/action2"
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public T Remove(string path)
        {
            T data = null;

            var cur = GetChildren(path);
            if (null != cur)
            {
                data = cur.data;
                cur.data = null;
                cur.TryRemove();
            }
            return data;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetAllData(List<T> list)
        {
            if (data != null)
            {
                list.Add(data);
            }
            foreach (var kv in children)
            {
                kv.Value.GetAllData(list);
            }
        }
    }

}
