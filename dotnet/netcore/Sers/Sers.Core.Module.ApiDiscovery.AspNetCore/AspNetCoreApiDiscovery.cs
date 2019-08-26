using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sers.Core.Module.Api.LocalApi;
using Sers.Core.Module.SersDiscovery;

namespace Sers.Core.Module.ApiDiscovery.AspNetCore
{
    public class AspNetCoreApiDiscovery : Sers.Core.Module.SsApiDiscovery.SsApiDiscovery
    {

        public class Config
        {
            /// <summary>
            /// 是否把请求方式（GET POST PUT 等）添加为 route的后缀。
            /// 例如    true:   /StationDemo/flold1/api1/GET
            ///        false:   /StationDemo/flold1/api1
            /// </summary>
            public bool MethodAsRouteSuffix = false;

        }

        public Config config = new Config();

        public AspNetCoreApiDiscovery(IDictionary<string, IApiNode> apiMap):base(apiMap)
        {
             
        }

        protected override IEnumerable<Type> DiscoveryControllers(DiscoveryConfig config)
        {
            var types = config.assembly.GetTypes().Where(type => typeof(Microsoft.AspNetCore.Mvc.Controller).IsAssignableFrom(type) && !type.IsAbstract && type.IsPublic);
            return types;
        }

        #region GetRoutePrefix

        protected override string GetRoutePrefix(DiscoveryConfig config, Type type)
        {
            // [Route("api/[controller]")]
            string routePrefix = type.GetCustomAttribute<Microsoft.AspNetCore.Mvc.RouteAttribute>()?.Template??"";
            if (routePrefix.StartsWith('/'))
            {
                routePrefix = routePrefix.Substring(1);
            }
            string controllerName = type.Name;
            if (controllerName.EndsWith("Controller"))
            {
                controllerName = controllerName.Substring(0, controllerName.Length-10);
            }
            return routePrefix?.Replace("[controller]", controllerName);
        }
        #endregion


    


        #region DiscoveryApiRoutes
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stationNames">demo:   ["AuthCenter","DemoStation"]</param>
        /// <param name="routePrefix">demo: "fold1/fold2"</param>
        /// <param name="method"></param>
        /// <param name="routes"></param>
        protected override void DiscoveryApiRoutes(List<string> stationNames, string routePrefix, MethodInfo method, List<string> routes)
        {
            var attrs = method.GetCustomAttributes<Microsoft.AspNetCore.Mvc.Routing.HttpMethodAttribute>();
            if (null == attrs || attrs.Count() == 0) return;

            foreach (var attr in attrs)
            {
                var route = attr?.Template?.Trim()??"";

                if (route.StartsWith('/'))
                {
                    // (x.1)  绝对路径。  /Station1/fold1/fold2/api1
                    AddAbsRoute(route);
                }
                else
                {
                    // (x.2)  相对路径。  fold1/fold2/api1  
                    string absRoute = "/" + routePrefix;

                    if (!string.IsNullOrWhiteSpace(route))
                        absRoute += "/"+ route;

                    AddAbsRoute(absRoute);

                    //foreach (var stationName in stationNames)
                    //{
                    //    //  /{stationName}/{routePrefix}/route

                    //    var absRoute = "/" + stationName;
                    //    if (!string.IsNullOrWhiteSpace(routePrefix))
                    //    {
                    //        absRoute += "/" + routePrefix;
                    //    }
                    //    absRoute += "/" + route;
                    //    routes.Add(absRoute);
                    //}
                }

                #region Method AddAbsRoute
                void AddAbsRoute(string absRoute)
                {
                    if (!config.MethodAsRouteSuffix)
                    {
                        routes.Add(absRoute);
                        return;
                    }

                    //absRoute 绝对路径。  /Station1/fold1/fold2/api1  
                    var HttpMethods = attr.HttpMethods;
                    if (HttpMethods.Count() == 0)
                    {
                        routes.Add(absRoute);
                    }
                    else
                    {
                        foreach (var item in HttpMethods)
                        {
                            routes.Add(absRoute + "/" + item);
                        }
                    }
                }
                #endregion
            }
        }

        #endregion







    }
}
