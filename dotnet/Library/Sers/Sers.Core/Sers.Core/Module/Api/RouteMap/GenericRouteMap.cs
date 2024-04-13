using Vit.Extensions;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Sers.Core.Module.Api.RouteMap
{
    public class GenericRouteMap<T> : NomalRouteMap<T>
         where T : class
    {



        Tree<T> tree = new Tree<T>();



        /// <summary>
        /// path demo：  "/station1/fold2/*"
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        public override T Remove(string route)
        {
            base.Remove(route);

            route = route.Substring(0, route.LastIndexOf("/"));
            return tree.Remove(route);
        }

        /// <summary>
        ///  path demo：  "/station1/fold2/*"
        /// </summary>
        /// <param name="route"></param>
        /// <param name="apiService"></param>
        public override void Set(string route, T apiService)
        {
            base.Set(route, apiService);

            route = route.Substring(0, route.LastIndexOf("/"));
            var node = tree.BuildPath(route);
            if (null != node)
            {
                node.data = apiService;
            }
        }




        /// <summary>
        /// path demo：  "/station1/fold2/action2.html"
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override T Routing(string route)
        {
            route = route.Substring(0, route.LastIndexOf("/"));
            return tree.QueryByPath(route);
        }

    }
}
