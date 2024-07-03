using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Sers.Core.Module.Api.RouteMap
{
    public class NomalRouteMap<T> : IDataMap<T>
        where T : class
    {

        /// <summary>
        ///  映射表 route ->  ApiService 
        /// </summary>
        //protected ConcurrentDictionary<string, T> apiRouteMap = new ConcurrentDictionary<string, T>();


        protected SortedDictionary<string, T> apiRouteMap = new SortedDictionary<string, T>();

        public int Count => apiRouteMap.Count;

        public IEnumerable<T> GetAll()
        {
            return (from kv in apiRouteMap select kv.Value);
        }

        public virtual T Remove(string route)
        {
            //apiRouteMap.TryRemove(route, out var apiService);
            //return apiService;

            T t = apiRouteMap[route];
            if (t != null) apiRouteMap.Remove(route);
            return t;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual T Get(string route)
        {
            if (apiRouteMap.TryGetValue(route, out var apiService))
            {
                return apiService;
            }
            return default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public virtual T Routing(string route)
        {
            return Get(route);
        }

        public virtual void Set(string route, T apiService)
        {
            apiRouteMap[route] = apiService;
        }
    }
}
