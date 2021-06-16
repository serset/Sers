using Newtonsoft.Json;
using Sers.Core.Module.Api.LocalApi;
using Sers.Core.Module.Api.LocalApi.StaticFileTransmit;
using Sers.Core.Module.Api.LocalApi.StaticFileTransmit.Extensions;
using Sers.Core.Module.ApiLoader;
using Sers.SersLoader;
using System;
using System.Reflection;
using Vit.Core.Module.Log;


namespace Vit.Extensions
{
    public static partial class ILocalApiService_Extensions
    {

        #region LoadApi

        //public ApiLoaderMng apiLoaderMng { get; set; } = new ApiLoaderMng();

        /// <summary>
        /// 从配置文件(appsettings.json  Sers.LocalApiService.ApiLoaders ) 加载api加载器并加载api
        /// </summary>
        public static void LoadApi(this ILocalApiService data)
        {
            data.ApiNodeMng.AddApiNode(new ApiLoaderMng().LoadApi());
        }


        /// <summary>
        /// 调用SersApi加载器加载api
        /// </summary>
        /// <param name="data"></param>
        /// <param name="config"></param>
        public static void LoadSersApi(this ILocalApiService data,ApiLoaderConfig config)
        {
            data.ApiNodeMng.AddApiNode(new Sers.SersLoader.ApiLoader().LoadApi(config));
        }

        /// <summary>
        /// 调用SersApi加载器加载api
        /// </summary>
        /// <param name="data"></param>
        /// <param name="assembly"></param>
        public static void LoadSersApi(this ILocalApiService data, Assembly assembly)
        {
            data.ApiNodeMng.AddApiNode(new Sers.SersLoader.ApiLoader().LoadApi(new ApiLoaderConfig { assembly = assembly }));
        }

        #endregion



    }
}
