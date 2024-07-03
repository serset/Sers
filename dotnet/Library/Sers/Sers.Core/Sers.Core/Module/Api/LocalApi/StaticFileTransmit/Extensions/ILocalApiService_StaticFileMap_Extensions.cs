using System;

using Sers.Core.Module.Api.LocalApi;
using Sers.Core.Module.Api.LocalApi.StaticFileTransmit;
using Sers.Core.Module.Api.LocalApi.StaticFileTransmit.Extensions;

using Vit.Core.Module.Log;


namespace Vit.Extensions
{
    public static partial class ILocalApiService_StaticFileMap_Extensions
    {


        #region LoadApi_StaticFileMap

        /// <summary>
        /// 从配置文件（appsettings.json::Sers.LocalApiService.staticFiles）加载静态文件映射器
        /// </summary>

        public static void LoadApi_StaticFiles(this ILocalApiService data)
        {
            if (data == null)
            {
                return;
            }

            var configs = Vit.Core.Util.ConfigurationManager.Appsettings.json.GetByPath<StaticFilesApiNodeConfig[]>("Sers.LocalApiService.staticFiles");

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


        static void LoadApi_StaticFileMap(ILocalApiService data, StaticFilesApiNodeConfig config)
        {

            #region (x.1)创建并初始化StaticFileMap
            StaticFileMap staticFileMap = new StaticFileMap(config);
            #endregion

            #region (x.2)onInvoke
            byte[] onInvoke(ArraySegment<byte> arg_OriData)
            {
                return staticFileMap.TransmitFile();
            }
            #endregion

            var apiNode = new ApiNode_Original(onInvoke, config.route);

            apiNode.apiDesc.description = config.apiName;
            apiNode.apiDesc.name = config.apiName;
            data.ApiNodeMng.AddApiNode(apiNode);

            var msg = "[LocalApiService] 已加载静态文件映射器";
            msg += "," + Environment.NewLine + "apiName:  " + config.apiName;
            msg += "," + "     route:  " + config.route;
            msg += "," + Environment.NewLine + "staticFileDirectory:  " + staticFileMap.fileBasePath;
            if (!string.IsNullOrEmpty(config.contentTypeMapFile))
            {
                msg += "," + Environment.NewLine + "contentTypeMapFile:  " + config.contentTypeMapFile;
            }
            Logger.Info(msg);
        }


        #endregion


    }
}
