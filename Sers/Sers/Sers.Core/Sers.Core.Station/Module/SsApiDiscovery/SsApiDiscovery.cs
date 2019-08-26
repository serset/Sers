using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sers.Core.Extensions;
using Sers.Core.Module.Api.ApiDesc;
using Sers.Core.Module.Api.LocalApi;
using Sers.Core.Module.Log;
using Sers.Core.Module.SersDiscovery;
using Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute;
using Sers.Core.Module.SsApiDiscovery.SersValid;
using Sers.Core.Util.ConfigurationManager;
using Sers.Core.Util.XmlComment;

namespace Sers.Core.Module.SsApiDiscovery
{
    public class SsApiDiscovery : ISersDiscovery
    {
        IDictionary<string, IApiNode> apiMap;
        public SsApiDiscovery(IDictionary<string, IApiNode> apiMap)
        {
            this.apiMap = apiMap;
        }

        SsModelBuilder ssModelBuilder = new SsModelBuilder();

        XmlMng xmlMng;
 
        public void Discovery(DiscoveryConfig config)
        {
      
            using (xmlMng = new XmlMng())
            {

               
                xmlMng.AddBin();

                ssModelBuilder.xmlMng = xmlMng;

                #region DiscoveryFromAssembly

                //(x.1)


                //(x.2) 获取Controller实体类
                var types = DiscoveryControllers(config);


                //(x.3)遍历types
                var routes = new List<string>();
                foreach (var type in types)
                {

                    // (x.x.1) 获取stationNames
                    List<string> stationNames = GetStationName(config,type);                    
                


                    //(x.x.2) 获取 routePrefix
                    string routePrefix = GetRoutePrefix(config,type);



                    #region (x.x.3) 遍历method
                    var obj = Activator.CreateInstance(type);
                    var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);
                    foreach (var method in methods)
                    {

                        //(x.x.x.1)  route
                        routes.Clear();
                        DiscoveryApiRoutes(stationNames, routePrefix, method, routes);
                        if (routes.Count == 0) continue;


                        //(x.x.x.2) sampleApiDesc
                        var sampleApiDesc = DiscoveryApiDesc(method);
                        var apiDesc = sampleApiDesc;

                        //(x.x.x.3) 构建ApiNode
                        foreach (string route in routes)
                        {
                            if (apiDesc == null) apiDesc = sampleApiDesc.ConvertBySerialize<SsApiDesc>();

                            apiDesc.route = route;
                            IApiNode apiNode = new LocalApiNode(apiDesc, method, obj);
                            apiMap[route] = apiNode;

                            apiDesc = null;
                        }

                    }
                    #endregion
                }
                #endregion
            }


        }

        #region GetStationName

        protected virtual List<string> GetStationName(DiscoveryConfig config, Type type)
        {
            List<string> stationNames;
            string stationName;

            //(x.1)
            stationName = config.apiStationName_Force;
            if (!string.IsNullOrEmpty(stationName))
            {
                stationNames = new List<string> { stationName };
                return stationNames;
            }


            //(x.2)
            var attrs = type.GetCustomAttributes<SsStationNameAttribute>();
            if (null != attrs && attrs.Count() != 0)             
            {
                stationNames = attrs.Select((attr) => attr.Value).ToList();
                return stationNames;
            }



            //(x.3)
            stationName = config.apiStationName;
            if (!string.IsNullOrEmpty(stationName))
            {
                stationNames = new List<string> { stationName };
                return stationNames;
            }


            //(x.4)
            stationNames = ConfigurationManager.Instance.GetByPath<List<string>>("Sers.ApiStation.Name");
            return stationNames;
 
        }

        #endregion

        #region GetRoutePrefix

        protected virtual string GetRoutePrefix(DiscoveryConfig config,Type type)
        {
            string routePrefix;

            routePrefix = config.routePrefix_Force;
            if (!string.IsNullOrEmpty(routePrefix))
            {
                return routePrefix;
            }


            routePrefix = type.GetCustomAttribute<SsRoutePrefixAttribute>()?.Value;
            if (!string.IsNullOrEmpty(routePrefix))
            {
                return routePrefix;
            }

            routePrefix = config.routePrefix;
            if (!string.IsNullOrEmpty(routePrefix))
            {
                return routePrefix;
            }
             
            return null;
        }
        #endregion

        #region DiscoveryControllers

        protected virtual IEnumerable<Type> DiscoveryControllers(DiscoveryConfig config)
        {
            var types = config.assembly.GetTypes().Where(type => typeof(IApiController).IsAssignableFrom(type) && !type.IsAbstract && type.IsPublic);
            return types;
        }
        #endregion


        #region DiscoveryApiDesc
        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        protected virtual SsApiDesc DiscoveryApiDesc(MethodInfo method)
        {
            var apiDesc = new SsApiDesc();
            //catagory
            apiDesc.catagory = method.GetCustomAttribute<SsCategoryAttribute>()?.Value;

            //name
            apiDesc.name = method.GetCustomAttribute<SsNameAttribute>()?.Value;

            //description
            apiDesc.description = method.GetCustomAttribute<SsDescriptionAttribute>()?.Value;

            //rpcValidations
            apiDesc.rpcValidations = SersValidMng.GetRpcValidationsFromMethod(method);

            var xmlComment = xmlMng.GetXmlHelp(method)?.Method_GetComment(method);


            // ArgType        
            apiDesc.argType = ssModelBuilder.BuildSsModel_Arg(method.GetParameters(), xmlComment);

            
            // ReturnType
            apiDesc.returnType = ssModelBuilder.BuildSsModel_Return(method);
            if (string.IsNullOrWhiteSpace(apiDesc.returnType.description))
            {
                apiDesc.returnType.description = xmlComment?.returns;
            }


            if (string.IsNullOrEmpty(apiDesc.description))
            {
                apiDesc.description = xmlComment?.summary?.Trim();
            }


            if (string.IsNullOrWhiteSpace(apiDesc.name))
            {
                apiDesc.name = method.Name;
            }

            return apiDesc;
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
        protected virtual  void DiscoveryApiRoutes(List<string> stationNames, string routePrefix, MethodInfo method, List<string> routes)
        {
            if (null == stationNames || stationNames.Count == 0) return;

            var attrs = method.GetCustomAttributes<SsRouteAttribute>();
            if (null == attrs || attrs.Count() == 0) return;

            foreach (var attr in attrs)
            {
                var route = attr?.Value?.Trim();

                if (string.IsNullOrWhiteSpace(route)) continue;



                if (route.StartsWith("/"))
                {
                    // (x.1)  绝对路径。  /Station1/fold1/fold2/api1  
                    routes.Add(route);
                }
                else
                {
                    // (x.2)  相对路径。  fold1/fold2/api1  
                    foreach (var stationName in stationNames)
                    {
                        //  /{stationName}/{routePrefix}/route

                        var absRoute = "/" + stationName;
                        if (!string.IsNullOrWhiteSpace(routePrefix))
                        {
                            absRoute += "/" + routePrefix;
                        }
                        absRoute += "/" + route;
                        routes.Add(absRoute);
                    }
                }
            }

        }

        #endregion





 
    }
}
