using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sers.SersLoader;
using Sers.Core.Module.Api.ApiDesc;
using Sers.Core.Module.Api.LocalApi;
using Sers.Serslot;
using Vit.Extensions;
using Vit.Ioc;
using Sers.Core.Module.Api.LocalApi.Event;

namespace Sers.NetcoreLoader
{
    public class ApiLoader : SersLoader.ApiLoader
    {
        static ApiLoader()
        {
            #region use ioc            
            LocalApiEventMng.Instance.UseIoc();            
            #endregion
        }


 

        public ApiLoader() :base()
        {             
        }

        protected override IEnumerable<Type> LoadControllers(ApiLoaderConfig config)
        {
            var types = ControllerHelp.Assembly_GetControllers(config.assembly);

            foreach (var t in types)
            {
                IocHelp.AddScoped(t);
            }
            IocHelp.Update();

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
        protected override List<IApiNode> LoadApiNodes(List<String> routePrefixs, MethodInfo method, Func<SsApiDesc> CreateApiDesc)
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

                IApiNode apiNode = new LocalApiNode(apiDesc, method);
                return apiNode;
            }).ToList();             
        }

        #endregion







    }
}
