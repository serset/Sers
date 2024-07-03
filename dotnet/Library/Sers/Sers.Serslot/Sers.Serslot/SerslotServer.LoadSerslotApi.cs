using System;
using System.Reflection;

using Sers.Core.Module.Api.ApiDesc;
using Sers.Core.Module.Api.LocalApi;
using Sers.SersLoader;

using Vit.Extensions;


namespace Sers.Serslot
{
    public partial class SerslotServer
    {

        /// <summary>
        ///  load apis by Serslot ApiLoader
        /// </summary>
        /// <param name="data"></param>
        /// <param name="assembly"></param>
        public void LoadSerslotApi(ILocalApiService data, Assembly assembly)
        {
            if (null == data)
            {
                return;
            }

            #region #1 api from host
            var config = new SersLoader.ApiLoaderConfig { assembly = assembly };

            Func<SsApiDesc, ApiLoaderConfig, IApiNode> CreateApiNode =
                (apiDesc, _) =>
                {
                    return new LocalApiNode(apiDesc, this);
                };

            var apiLoader = new Sers.Serslot.ApiLoader(CreateApiNode);

            data.ApiNodeMng.AddApiNode(apiLoader.LoadApi(config));
            #endregion


            // #2 load api from appsettings.json::serslot.extApi
            data.LoadSerslotExtApi(apiDesc => new LocalApiNode(apiDesc, this));

        }
    }
}
