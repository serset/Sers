using Sers.Core.Module.Api.LocalApi;
using Sers.Core.Module.Api.LocalApi.StaticFileTransmit;
using System;
using Vit.Core.Module.Log;


namespace Vit.Extensions
{
    public static partial class LocalApiService_StaticFileMap_Extensions
    {

        #region LoadApi_StaticFileMap

        /// <summary>
        /// 从配置文件（appsettings.json::Sers.LocalApiService.StaticFileMap）加载静态文件映射器
        /// </summary>

        public static void LoadApi_StaticFileMap(this LocalApiService data)
        {
            if (data == null)
            {
                return;
            }

            var configs = Vit.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetByPath<StaticFileMapConfig[]>("Sers.LocalApiService.StaticFileMap");

            if (configs == null || configs.Length == 0) return;

            foreach (var config in configs)
            {
                try
                {
                    LoadApi_StaticFileMap(data, config);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }

        }


        static void LoadApi_StaticFileMap(LocalApiService data, StaticFileMapConfig config)
        {

            #region (x.1)创建并初始化StaticFileMap
            StaticFileMap staticFileMap = new StaticFileMap(config.staticFileDirectory);
            var loadedContentTypeMap = staticFileMap.LoadContentTypeFromFile(config.ContentTypeMapFile);
            #endregion

            #region (x.2)onInvoke
            Func<ArraySegment<byte>, byte[]> onInvoke = (ArraySegment<byte> arg_OriData) => {
                return staticFileMap.TransmitFile();
            };
            #endregion

            var apiNode = new ApiNode_Original(onInvoke, config.route);

            apiNode.apiDesc.description = config.apiName;
            apiNode.apiDesc.name = config.apiName;
            data.apiNodeMng.AddApiNode(apiNode);

            var msg = "[LocalApiService] 已加载静态文件映射器";
            msg += "," + Environment.NewLine + "apiName:  " + config.apiName;
            msg += ","+ "     route:  " + config.route;
            msg += "," + Environment.NewLine + "staticFileDirectory:  " + staticFileMap.fileBasePath;
            if (loadedContentTypeMap)
            {
                msg += ","+Environment.NewLine+"ContentTypeMapFile:  "+ config.ContentTypeMapFile;
            }
            Logger.Info(msg);
        }

        class StaticFileMapConfig
        {
            /// <summary>
            /// api路由前缀，例如 "/demo/ui/*"
            /// </summary>
            public string route;

            /// <summary>
            /// api描述，静态文件描述
            /// </summary>
            public string apiName;
            /// <summary>
            /// 额外静态文件类型映射配置的文件路径（mappings.json），例如"wwwroot/mappings.json"。若不指定（或指定的文件不存在）则不添加额外文件类型映射配置
            /// </summary>
            public string ContentTypeMapFile;
            /// <summary>
            /// 静态文件的路径，如 "wwwroot/demo"
            /// </summary>
            public string staticFileDirectory;          
        }


        #endregion


    }
}
