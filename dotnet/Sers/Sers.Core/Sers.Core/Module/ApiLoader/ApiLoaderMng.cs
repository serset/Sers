using Newtonsoft.Json.Linq;
using Vit.Extensions;
using Sers.Core.Module.Api.LocalApi;
using Vit.Core.Module.Log;
using Vit.Core.Util.Common;
using System;
using System.Collections.Generic;
using System.Reflection;
using Sers.Core.Module.Reflection;

namespace Sers.Core.Module.ApiLoader
{
    public class ApiLoaderMng
    {

        /// <summary>
        /// 从配置文件(appsettings.json  Sers.LocalApiService.ApiLoaders ) 加载api加载器并加载api
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IApiNode> LoadApi()
        {
      
            var configs = Vit.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetByPath<JObject[]>("Sers.LocalApiService.ApiLoaders");
            if (configs == null || configs.Length == 0) return null;

            List<IApiNode> apiNodes = new List<IApiNode>();

            foreach (var config in configs)
            {
                try
                {
                    var apiLoader = GetApiLoader(config);

                    if (apiLoader != null)
                    {

                        var nodes = apiLoader.LoadApi(config);
                        if (nodes != null)
                            apiNodes.AddRange(nodes);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }
            return apiNodes;
        }

        #region GetApiLoader
        IApiLoader GetApiLoader(JObject config)
        {
            //(x.1) get className    
            var className = config["loader_className"].ConvertToString();
            if (string.IsNullOrEmpty(className) || className == "Sers.ApiLoader.Sers.ApiLoader")
                return new Sers.ApiLoader.Sers.ApiLoader();

            var assemblyFile = config["loader_assemblyFile"].ConvertToString();
             
            #region (x.2) CreateInstance
            var apiLoader = ObjectLoader.CreateInstance(assemblyFile, className) as IApiLoader;
            #endregion

            //(x.3) return
            return apiLoader;
        }
        #endregion


    }
}
