using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Sers.Core.Module.Api.RouteMap
{
    public class RouteMap<T> : IDataMap<T>
        where T : class
    {



        IDataMap<T> normalRouteMap = new NomalRouteMap<T>();
        IDataMap<T> genericRouteMap = new GenericRouteMap<T>();




        /// <summary>
        /// 是否为普通接口（不为泛接口）
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool RouteIsNormalApi(string route)
        {
            return !route.EndsWith("*");
        }

        public int Count => normalRouteMap.Count + genericRouteMap.Count;


        /// <summary>
        /// path demo：  1."/station1/fold2/api1"    2."/station1/fold2/*"
        /// </summary>
        /// <param name="route"></param>
        /// <param name="apiService"></param>
        public void Set(string route, T apiService)
        {
            lock (this)
            {
                var apiMap = RouteIsNormalApi(route) ? normalRouteMap : genericRouteMap;

                apiMap.Set(route, apiService);
            }
        }

        /// <summary>
        /// route demo：  1."/station1/fold2/api1"    2."/station1/fold2/*"
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        public T Remove(string route)
        {
            lock (this)
            {
                var apiMap = RouteIsNormalApi(route) ? normalRouteMap : genericRouteMap;

                return apiMap.Remove(route);
            }
        }


        /// <summary>
        /// route demo：  1."/station1/fold2/api1"    2."/station1/fold2/*"
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Get(string route)
        {
            return normalRouteMap.Get(route) ?? genericRouteMap.Get(route);
        }


        /// <summary>
        /// route demo：  1."/station1/fold2/api1"    2."/station1/fold2/*"   3."/station1/fold2/index.html"
        /// </summary>
        /// <param name="route"></param>
        /// <param name="routeType"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Routing(string route, out ERouteType routeType)
        {
            var t = normalRouteMap.Routing(route);
            if (null != t)
            {
                routeType = ERouteType.nomalRoute;
                return t;
            }
            routeType = ERouteType.genericRoute;
            return genericRouteMap.Routing(route);
        }

        /// <summary>
        /// route demo：  1."/station1/fold2/api1"    2."/station1/fold2/*"   3."/station1/fold2/index.html"
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Routing(string route)
        {
            return Routing(route, out _);
        }

        public IEnumerable<T> GetAll()
        {
            return normalRouteMap.GetAll().Concat(genericRouteMap.GetAll());
        }




    }
}
