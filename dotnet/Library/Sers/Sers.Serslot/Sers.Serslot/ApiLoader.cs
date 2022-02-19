using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sers.SersLoader;
using Sers.Core.Module.Api.ApiDesc;
using Sers.Core.Module.Api.LocalApi;
using Vit.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Sers.Serslot
{
    public class ApiLoader: SersLoader.ApiLoader
    {

        Func<SsApiDesc, ApiLoaderConfig,IApiNode> CreateApiNode;

        public ApiLoader(Func<SsApiDesc, ApiLoaderConfig, IApiNode> CreateApiNode) : base()
        {
            this.CreateApiNode = CreateApiNode;
        }

        protected override IEnumerable<Type> LoadControllers(ApiLoaderConfig config)
        {
            var types = ControllerHelp.Assembly_GetControllers(config.assembly);          

            return types;
        }


        #region GetRoutePrefix MethodInfoGetReturnType

        /// <summary>
        /// demo:   ["/Auth/fold1/fold2","/api","/"]
        /// </summary>
        /// <param name="config"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        protected override List<string> GetRoutePrefixs(ApiLoaderConfig config, Type type)
        {
            return ControllerHelp.Controller_GetRoutePrefixs(type);
        }

        protected override ParameterInfo[] MethodInfoGetArgInfos(MethodInfo method)
        {
            var argInfos = base.MethodInfoGetArgInfos(method);

            //剔除指定 FromServices 的函数参数
            if (argInfos != null) 
            {
                argInfos = argInfos.AsQueryable().Where(info => info.GetCustomAttribute<FromServicesAttribute>() == null).ToArray();
            }
            return argInfos;
        }

        protected override Type MethodInfoGetReturnType(MethodInfo method)
        {
            return ControllerHelp.Action_GetReturnType(method);
        }

        #endregion




        #region LoadApiNodes
        /// <summary>
        /// 
        /// </summary>
        /// <param name="routePrefixs">demo:   ["/Auth/fold1/fold2","/api"]</param>   
        /// <param name="method"></param>
        /// <param name="CreateApiDesc"></param>
        /// <param name="config"></param>
        protected override List<IApiNode> LoadApiNodes(List<String> routePrefixs, MethodInfo method, Func<SsApiDesc> CreateApiDesc, ApiLoaderConfig config)
        {
         
            var routes = ControllerHelp.Action_GetRoutes(routePrefixs, method);

            // routes -> apiNode
            return routes.Select(routeInfo => {

                (string route, string httpMethod, string oriRoute) = routeInfo;

                var apiDesc = CreateApiDesc();
                apiDesc.route = route;
                apiDesc.HttpMethodSet(httpMethod);

                apiDesc.OriRouteSet(oriRoute);
                apiDesc.SysDescAppend("oriRoute: " + oriRoute);


                IApiNode apiNode = CreateApiNode(apiDesc,config);
                return apiNode;
                
            }).ToList();                 
             
        }

        #endregion

    }
}
