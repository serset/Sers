using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sers.SersLoader;
using Vit.Extensions;

namespace Sers.Serslot
{
    public class ControllerHelp
    {

        static ControllerHelp()
        {
            //SsModelBuilder.Getter_Name.Add(GetAttribute =>
            //{
            //    return GetAttribute.GetAttribute<DisplayNameAttribute>()?.DisplayName;
            //});

            //SsModelBuilder.Getter_Description.Add(GetAttribute =>
            //{
            //    return GetAttribute.GetAttribute<DescriptionAttribute>()?.Description;
            //});

            //SsModelBuilder.Getter_Example.Add(GetAttribute =>
            //{
            //    return GetAttribute.GetAttribute<SsExampleAttribute>()?.Value;
            //});


            //SsModelBuilder.Getter_DefaultValue.Add(GetAttribute =>
            //{
            //    return GetAttribute.GetAttribute<DefaultValueAttribute>()?.Value;
            //});
        
            SsModelBuilder.Getter_Type.Add(GetAttribute =>
            {
                return GetAttribute.GetAttribute<ModelMetadataTypeAttribute>()?.MetadataType;
            });

        }

        #region Assembly_GetControllers

        public static IEnumerable<Type> Assembly_GetControllers(Assembly assembly)
        {
           return assembly.GetTypes().Where(type => 
           typeof(Microsoft.AspNetCore.Mvc.ControllerBase).IsAssignableFrom(type)
           && type.GetCustomAttribute<NonControllerAttribute>()==null
           && !type.IsAbstract
           && type.IsPublic);
        }
        #endregion


        #region Controller_GetRoutePrefixs
        /// <summary>
        /// demo:   ["/Auth/fold1/fold2","/api","/"]
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<string> Controller_GetRoutePrefixs(Type type)
        {
            #region controllerName            
            string controllerName = type.Name;
            if (controllerName.EndsWith("Controller"))
            {
                controllerName = controllerName.Substring(0, controllerName.Length - 10);
            }
            #endregion

            // [Route("api/[controller]")]
            var routePrefixs = type.GetCustomAttributes<Microsoft.AspNetCore.Mvc.RouteAttribute>().Select(routeAttribute => 
            {
                var routePrefix = routeAttribute.Template;
                if (routePrefix.StartsWith("/"))
                {
                    routePrefix = routePrefix.Substring(1);
                }              
                return routePrefix?.Replace("[controller]", controllerName);
            }).ToList();

            if (routePrefixs.Count == 0)
            { 
                routePrefixs.Add("");
            } 
            return routePrefixs;
        }
        #endregion


        #region Action_GetReturnType

        public static Type Action_GetReturnType(MethodInfo method)
        {
            var returnType = method.ReturnType;

            //Task
            if (returnType.IsGenericType && typeof(Task<>).IsAssignableFrom(returnType.GetGenericTypeDefinition()))
            {
                returnType = returnType.GetGenericArguments()[0];
            }

            //ActionResult
            if (returnType.IsGenericType && typeof(ActionResult<>).IsAssignableFrom(returnType.GetGenericTypeDefinition()))
            {
                returnType = returnType.GetGenericArguments()[0];
            }

            return returnType;
        }
        #endregion

        #region Action_GetRoutes
        /// <summary>
        /// 
        /// </summary>
        /// <param name="routePrefixs">demo:   ["/Auth/fold1/fold2","/api","/"]</param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static List<(string route, string httpMethod, string oriRoute)> Action_GetRoutes(List<String> routePrefixs, MethodInfo method)
        {
            var routes = new List<(string route, string httpMethod, string oriRoute)>();

            if (null!=method.GetCustomAttributes<NonActionAttribute>())
                return routes;

            var attrs = method.GetCustomAttributes<Microsoft.AspNetCore.Mvc.Routing.HttpMethodAttribute>();
            if (null == attrs || attrs.Count() == 0) return routes;
            
            var attrRoute = method.GetCustomAttribute<Microsoft.AspNetCore.Mvc.RouteAttribute>()?.Template ?? ""; 

            foreach (var attr in attrs)
            {
                var route = attr?.Template?.Trim() ?? attrRoute;

                if (route.StartsWith("/"))
                {
                    // (x.1)  绝对路径。  /Station1/fold1/fold2/api1
                    AddAbsRoute(route); 
                }
                else
                {
                    // (x.2)  相对路径。  fold1/fold2/api1  
                    routePrefixs.ForEach(routePrefix=> 
                    {
                        string absRoute = "/" + routePrefix;

                        if (!string.IsNullOrWhiteSpace(route))
                            absRoute += "/" + route;

                        AddAbsRoute(absRoute);
                    });                 
                }

                #region Method AddAbsRoute
                void AddAbsRoute(string absRoute)
                {
                    //absRoute 绝对路径。  /Station1/fold1/fold2/api1  

                    var oriRoute = absRoute;
                    var Route = oriRoute;
                    #region 处理特殊路由
                    {
                        //(x.x.1)   /api/{action}/a
                        Route = Route.Replace("[action]",method.Name);


                        //(x.x.2) /api/Value/a?i=1
                        int index = Route.IndexOf("?");
                        if (index > 0)
                        {
                            Route = Route.Substring(0, index);                            
                        }

                        //(x.x.3)   /api/Value/{id}
                        index = Route.IndexOf('{');
                        if (index > 0)
                        {
                            Route = Route.Substring(0, index);
                            index = Route.LastIndexOf('/');
                            Route = Route.Substring(0, index) + "/*";
                        }
                    }
                    #endregion
                
                    var HttpMethods = attr.HttpMethods;
                    if (HttpMethods != null || HttpMethods.Count() > 0)
                    {
                        foreach (var httpMethod in HttpMethods)
                        {
                            routes.Add((Route, httpMethod,oriRoute)); 
                        }
                    }
                }
                #endregion
            }

            return routes;
        }
        #endregion
    }
}
