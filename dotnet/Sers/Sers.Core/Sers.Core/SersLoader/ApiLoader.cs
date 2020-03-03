using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Sers.Core.Module.Api.ApiDesc;
using Sers.Core.Module.Api.LocalApi;
using Sers.Core.Module.ApiLoader;
using Sers.SersLoader.RpcVerify2;
using Vit.Core.Module.Log;
using Vit.Core.Util.Common;
using Vit.Core.Util.ComponentModel.Api;
using Vit.Core.Util.ComponentModel.Model;
using Vit.Core.Util.ConfigurationManager;
using Vit.Core.Util.XmlComment;
using Vit.Extensions;

namespace Sers.SersLoader
{
    public class ApiLoader : IApiLoader
    {

        public ApiLoader()
        {
        }


        SsModelBuilder ssModelBuilder = new SsModelBuilder();

        XmlMng xmlMng;

        /// <summary>
        /// 调用SsApi加载器加载api
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public IEnumerable<IApiNode> LoadApi(JObject config)
        {
            return LoadApi(config.ConvertBySerialize<ApiLoaderConfig>());
        }

        /// <summary>
        /// 调用SsApi加载器加载api
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public IEnumerable<IApiNode> LoadApi(ApiLoaderConfig config)
        {

            #region (x.1)get assembly

            //(x.x.1) load from dll file
            if (config.assembly == null && !String.IsNullOrEmpty(config.assemblyFile))
            {
                try
                {
                    config.assembly = Assembly.LoadFile(CommonHelp.GetAbsPathByRealativePath(config.assemblyFile));
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }

            //(x.x.2) load by assemblyName
            if (config.assembly == null && !String.IsNullOrEmpty(config.assemblyName))
            {
                try
                {
                    config.assembly = Assembly.Load(config.assemblyName);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }

            //if (config.assembly == null) return null;
            if (config.assembly == null) yield break;

            #endregion


            #region (x.2)LoadSubscriber            
            Core.Module.PubSub.Controller.SubscriberLoader.LoadSubscriber(config.assembly);
            #endregion


            #region (x.3)LoadApi
            Logger.Info("[ApiLoader] LoadApi,assembly:[" + config.assembly.FullName + "]");

            //List<IApiNode> apiNodes = new List<IApiNode>();

            using (xmlMng = new XmlMng())
            {

                xmlMng.AddBin();

                ssModelBuilder.xmlMng = xmlMng;


                #region LoadFromAssembly

                //(x.1) 获取Controller实体类
                var types = LoadControllers(config);


                //(x.2)遍历Controller                
                foreach (var type in types)
                {
                    //(x.x.1) 获取 routePrefix
                    List<String> routePrefixs = GetRoutePrefixs(config, type);


                    #region (x.x.3) 遍历method 构建apiNodes             
                    var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);
                    foreach (var method in methods)
                    {
                        //(x.x.x.1) sampleApiDesc
                        SsApiDesc sampleApiDesc = null;

                        //(x.x.x.2) 构建apiNodes
                        var nodes = LoadApiNodes(routePrefixs, method, () => {
                            if(sampleApiDesc==null) return sampleApiDesc = GetApiDesc(method);
                            return sampleApiDesc.Clone();
                        });
                        if (nodes != null && nodes.Count > 0)
                        {
                            //apiNodes.AddRange(nodes);
                            foreach (var n in nodes)
                            {
                                yield return n;
                            }
                        }
                    }
                    #endregion
                }
                #endregion
            }


            //return apiNodes;
            #endregion

        }




        #region GetStationName

        protected virtual List<String> GetStationNames(ApiLoaderConfig config, Type type)
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
            stationNames = ConfigurationManager.Instance.GetByPath<List<String>>("Sers.LocalApiService.apiStationNames");
            return stationNames;

        }

        #endregion

        #region GetRoutePrefix

        protected virtual List<String> GetRoutePrefixs(ApiLoaderConfig config, Type type)
        {
            //（优先级从高到低：  routePrefix_Force、在代码上的SsRoutePrefixAttribute特性指定 、 routePrefix）

            // (x.1) 获取stationNames
            List<String> stationNames = GetStationNames(config, type);


            //(x.2) 构建RoutePrefix
            var prefix = GetRoutePrefix();
            return stationNames.Select(stationName =>
            {
                //  /{stationName}/{routePrefix}/route               
                if (String.IsNullOrWhiteSpace(prefix))
                {
                    return "/" + stationName;
                }
                else
                {
                    return "/" + stationName + "/" + prefix;
                }
            }).ToList();

            string GetRoutePrefix()
            {
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


        }
        #endregion

        #region DiscoveryControllers

        protected virtual IEnumerable<Type> LoadControllers(ApiLoaderConfig config)
        {
            var types = config.assembly.GetTypes().Where(type => typeof(IApiController).IsAssignableFrom(type) && !type.IsAbstract && type.IsPublic);
            return types;
        }
        #endregion


        protected virtual Type MethodInfoGetReturnType(MethodInfo method)
        {
            return method.ReturnType;
        }


        #region GetApiDesc
        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        protected virtual SsApiDesc GetApiDesc(MethodInfo method)
        {
            var apiDesc = new SsApiDesc();
            var xmlComment = xmlMng.GetXmlHelp(method)?.Method_GetComment(method);

            //(x.1) name from code
            apiDesc.name = method.GetCustomAttribute<SsNameAttribute>()?.Value;

            //(x.2) description from code
            apiDesc.description = method.GetCustomAttribute<SsDescriptionAttribute>()?.Value;

            //(x.3) rpcValidations from code
            //apiDesc.rpcValidations = SersValidMng.GetRpcValidationsFromMethod(method);    
            apiDesc.rpcVerify2 = RpcVerify2Loader.GetRpcVerify2FromMethod(method);

            //(x.4)ArgType        
            apiDesc.argType = ssModelBuilder.BuildSsModel_Arg(method, xmlComment);

            //(x.5)ReturnType
            apiDesc.returnType = ssModelBuilder.BuildSsModel_Return(method, MethodInfoGetReturnType(method));
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


        #region LoadApiNodes
        /// <summary>
        /// 
        /// </summary>
        /// <param name="routePrefixs">demo:   ["/Auth/fold1/fold2","/api","/"]</param>   
        /// <param name="method"></param>
        /// <param name="CreateApiDesc"></param> 
        protected virtual List<IApiNode> LoadApiNodes(List<String> routePrefixs, MethodInfo method, Func<SsApiDesc> CreateApiDesc)
        {
            if (null == routePrefixs || routePrefixs.Count == 0) return null;

            var attrs = method.GetCustomAttributes<SsRouteAttribute>();
            if (null == attrs || attrs.Count() == 0) return null;

            #region (x.1)获取 apiDescs
            
            List<SsApiDesc> apiDescs = new List<SsApiDesc>();

            foreach (var attr in attrs)
            {
                var route = attr?.Value?.Trim();

                if (String.IsNullOrEmpty(route)) continue;


                if (route.StartsWith("/"))
                {
                    // (x.1)  绝对路径。  /Station1/fold1/fold2/api1  
                    var apiDesc = CreateApiDesc();
                    apiDesc.route = route;

                    if (!string.IsNullOrEmpty(attr.HttpMethod))
                    {
                        apiDesc.HttpMethodSet(attr.HttpMethod);
                    }
                    apiDescs.Add(apiDesc);
                }
                else
                {
                    // (x.2)  相对路径。  fold1/fold2/api1  
                    foreach (var routePrefix in routePrefixs)
                    {
                        //  /{stationName}/{routePrefix}/route
                        var absRoute = routePrefix + "/" + route;                        

                        var apiDesc = CreateApiDesc();
                        apiDesc.route = absRoute;
                        if (!string.IsNullOrEmpty(attr.HttpMethod))
                        {
                            apiDesc.HttpMethodSet(attr.HttpMethod);
                        }
                        apiDescs.Add(apiDesc);
                    }
                }
            }
            #endregion

            var apiController_Obj = Activator.CreateInstance(method.DeclaringType);

            //(x.2) apiDescs -> apiNode
            return apiDescs.Select(apiDesc =>
            {
                IApiNode apiNode = new LocalApiNode(apiDesc, method, apiController_Obj);
                return apiNode;
            }).ToList();
        }

        #endregion






    }
}
