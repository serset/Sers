
using Sers.Core.Module.Api.ApiDesc;
using Sers.Core.Module.Api.LocalApi;
using System;
using System.Collections.Generic;
using System.Reflection;
using Sers.SersLoader;
using Sers.Serslot;
using ApiLoader = Sers.Serslot.ApiLoader;
using LocalApiNode = Sers.Serslot.LocalApiNode;

namespace Vit.Extensions
{
    public static partial class LocalApiServiceExtensions
    {
        /// <summary>
        /// 调用Serslot加载器加载api
        /// </summary>
        /// <param name="data"></param>
        /// <param name="assembly"></param>
        /// <param name="server"></param>
        public static void LoadSerslotApi(this LocalApiService data, Assembly assembly, SerslotServer server)
        {
            if (null == data)
            {
                return;
            }
            #region (x.1) api from host
            var config = new ApiLoaderConfig { assembly= assembly };
            data.apiNodeMng.AddApiNode(new  ApiLoader(server).LoadApi(config));
            #endregion


            #region (x.2) api from appsettings.json::serslot.extApi
            Vit.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetByPath<List<SsApiDesc>>("serslot.extApi")?.ForEach(apiDesc =>
            {
                IApiNode apiNode;

                var reply = apiDesc.extendConfig?["reply"];

                if (reply == null)
                {
                    //(x.x.1)由host处理
                    apiNode = new LocalApiNode(apiDesc, server);
                }
                else
                {
                    //(x.x.2)直接返回 指定的数据
                    var replyBytes = reply.SerializeToBytes();
                    #region onInvoke
                    Func<ArraySegment<byte>, byte[]> onInvoke = (ArraySegment<byte> arg_OriData) => {
                        return replyBytes;
                    };
                    #endregion

                    apiNode = new ApiNode_Original(onInvoke, apiDesc);
                }

                data.apiNodeMng.AddApiNode(apiNode);
            });            
            #endregion

        }



    }
}
