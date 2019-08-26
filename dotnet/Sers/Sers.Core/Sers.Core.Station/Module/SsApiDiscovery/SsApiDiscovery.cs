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
        IDictionary<String, IApiNode> apiMap;
        public SsApiDiscovery(IDictionary<String, IApiNode> apiMap)
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


                //(x.1) 获取Controller实体类
                var types = DiscoveryControllers(config);


                //(x.2)遍历Controller                
                foreach (var type in types)
                {
                    try
                    {                        

                        // (x.x.1) 获取stationNames
                        List<String> stationNames = GetStationName(config, type);


                        //(x.x.2) 获取 routePrefix
                        String routePrefix = GetRoutePrefix(config, type);


                        #region (x.x.3) 遍历method     
                        var obj = Activator.CreateInstance(type);
                        var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);
                        foreach (var method in methods)
                        {

                            //(x.x.x.1)  route
                            var routes = new List<String>();
                            //routes.Clear();
                            DiscoveryApiRoutes(stationNames, routePrefix, method, routes);
                            if (routes.Count == 0) continue;


                            //(x.x.x.2) sampleApiDesc
                            var sampleApiDesc = DiscoveryApiDesc(method);
                            var apiDesc = sampleApiDesc;

                            //(x.x.x.3) 构建ApiNode
                            foreach (String route in routes)
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
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                        throw;
                    }
                }
                #endregion
            }


        }

        #region GetStationName

        protected virtual List<String> GetStationName(DiscoveryConfig config, Type type)
        {
            //（优先级从高到低：  apiStationName_Force 、 在代码上的SsStationNameAttribute特性指定 、 apiStationName 、 appsettings.json指定）

            List<String> stationNames;
            String stationName;

            //(x.1) apiStationName_Force
            stationName = config.apiStationName_Force;
            if (!String.IsNullOrEmpty(stationName))
            {
                stationNames = new List<String> { stationName };
                return stationNames;
            }


            //(x.2) 在代码上的SsStationNameAttribute特性指定
            var attrs = type.GetCustomAttributes<SsStationNameAttribute>();
            if (null != attrs && attrs.Count() != 0)             
            {
                stationNames = attrs.Select((attr) => attr.Value).ToList();
                return stationNames;
            }



            //(x.3) apiStationName
            stationName = config.apiStationName;
            if (!String.IsNullOrEmpty(stationName))
            {
                stationNames = new List<String> { stationName };
                return stationNames;
            }


            //(x.4)  appsettings.json指定
            stationNames = ConfigurationManager.Instance.GetByPath<List<String>>("Sers.ApiStation.apiStationName");
            return stationNames;
 
        }

        #endregion

        #region GetRoutePrefix

        protected virtual String GetRoutePrefix(DiscoveryConfig config,Type type)
        {
            //（优先级从高到低：  routePrefix_Force、在代码上的SsRoutePrefixAttribute特性指定 、 routePrefix）


            String routePrefix;


            //(x.1) routePrefix_Force
            routePrefix = config.routePrefix_Force;
            if (!String.IsNullOrEmpty(routePrefix))
            {
                return routePrefix;
            }

            //(x.2) 在代码上的SsRoutePrefixAttribute特性指定
            routePrefix = type.GetCustomAttribute<SsRoutePrefixAttribute>()?.Value;
            if (!String.IsNullOrEmpty(routePrefix))
            {
                return routePrefix;
            }

            //(x.3) routePrefix
            routePrefix = config.routePrefix;
            if (!String.IsNullOrEmpty(routePrefix))
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
            var xmlComment = xmlMng.GetXmlHelp(method)?.Method_GetComment(method);

            //(x.1) name from code
            apiDesc.name = method.GetCustomAttribute<SsNameAttribute>()?.Value;

            //(x.2) description from code
            apiDesc.description = method.GetCustomAttribute<SsDescriptionAttribute>()?.Value;

            //(x.3) rpcValidations from code
            apiDesc.rpcValidations = SersValidMng.GetRpcValidationsFromMethod(method);    

            //(x.4)ArgType        
            apiDesc.argType = ssModelBuilder.BuildSsModel_Arg(method, xmlComment);
            
            //(x.5)ReturnType
            apiDesc.returnType = ssModelBuilder.BuildSsModel_Return(method);
            if (String.IsNullOrWhiteSpace(apiDesc.returnType.description))
            {
                apiDesc.returnType.description = xmlComment?.returns;
            }

            //(x.6) description from xmlComment
            if (String.IsNullOrEmpty(apiDesc.description))
            {
                apiDesc.description = xmlComment?.summary?.Trim();
            }

            //(x.7) Name from xmlComment
            if (String.IsNullOrWhiteSpace(apiDesc.name))
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
        protected virtual  void DiscoveryApiRoutes(List<String> stationNames, String routePrefix, MethodInfo method, List<String> routes)
        {
            if (null == stationNames || stationNames.Count == 0) return;

            var attrs = method.GetCustomAttributes<SsRouteAttribute>();
            if (null == attrs || attrs.Count() == 0) return;

            foreach (var attr in attrs)
            {
                var route = attr?.Value?.Trim();

                if (String.IsNullOrEmpty(route)) continue;


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
                        if (!String.IsNullOrWhiteSpace(routePrefix))
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
