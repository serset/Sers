using System;
using System.Reflection;
using Sers.Core.Module.Api.ApiDesc;
using Sers.Core.Module.Api.LocalApi;
using Vit.Extensions;


namespace Sers.Serslot.Mode.BackgroundTask
{
    public partial class SerslotServer 
    {

        /// <summary>
        /// 调用Serslot加载器加载api
        /// </summary>
        /// <param name="data"></param>
        /// <param name="assembly"></param>
        public void LoadSerslotApi(ILocalApiService data, Assembly assembly)
        {
            if (null == data)
            {
                return;
            }

            #region (x.1) api from host
            var config = new SersLoader.ApiLoaderConfig { assembly = assembly };

            Func<SsApiDesc, IApiNode> CreateApiNode =
                (apiDesc) =>
                {
                    return new LocalApiNode( apiDesc,this);
                };

            var apiLoader = new Sers.Serslot.ApiLoader(CreateApiNode);

            data.ApiNodeMng.AddApiNode(apiLoader.LoadApi(config));
            #endregion


            //(x.2)load api from appsettings.json::serslot.extApi
            data.LoadSerslotExtApi(apiDesc=> new LocalApiNode(apiDesc, this));

        }
    }
}
