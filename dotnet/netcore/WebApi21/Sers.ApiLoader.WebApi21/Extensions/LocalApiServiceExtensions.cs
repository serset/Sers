
using Sers.Core.Module.Api.LocalApi;
using Sers.Core.Module.ApiLoader;
using System.Reflection;

namespace Vit.Extensions
{
    public static partial class LocalApiServiceExtensions
    {
        /// <summary>
        /// 调用WebApi21加载器加载api
        /// </summary>
        /// <param name="localApiService"></param>
        /// <param name="assembly"></param>

        public static void LoadWebApi21(this LocalApiService localApiService, Assembly assembly)
        {
            if (null == localApiService)
            {
                return;
            }

            var config = new ApiLoaderConfig { assembly= assembly };
            localApiService.apiNodeMng.AddApiNode(new Sers.ApiLoader.WebApi21.ApiLoader().LoadApi(config));
 
        }



    }
}
