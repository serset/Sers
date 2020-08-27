
using Sers.Core.Module.Api.LocalApi;
using System.Reflection;
using Sers.SersLoader;
using ApiLoader = Sers.NetcoreLoader.ApiLoader;

namespace Vit.Extensions
{
    public static partial class LocalApiServiceExtensions
    {
        /// <summary>
        /// 调用netcore加载器加载api
        /// </summary>
        /// <param name="localApiService"></param>
        /// <param name="assembly"></param>

        public static void LoadNetcoreApi(this LocalApiService localApiService, Assembly assembly)
        {
            if (null == localApiService)
            {
                return;
            }

            var config = new ApiLoaderConfig { assembly= assembly };
            localApiService.apiNodeMng.AddApiNode(new ApiLoader().LoadApi(config));
 
        }



    }
}
