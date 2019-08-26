using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;


namespace Sers.Core.Module.Api.RouteMap
{
    public class NomalRouteMap<T> : IDataMap<T>
    {

        /// <summary>
        ///  映射表 route ->  ApiService 
        /// </summary>
        protected ConcurrentDictionary<string, T> apiRouteMap = new ConcurrentDictionary<string, T>();


        public int Count => apiRouteMap.Count;

        public IEnumerable<T> GetAll()
        {
            return (from kv in apiRouteMap select kv.Value);
        }

        public virtual T Remove(string route)
        { 
            apiRouteMap.TryRemove(route, out var apiService);
            return apiService;
        }


        public virtual T Get(string route)
        {
            if (apiRouteMap.TryGetValue(route, out var apiService))
            {
                return apiService;
            }
            return default(T);
        }
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
