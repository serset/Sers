
using Sers.Core.Module.Api.ApiDesc;
using Sers.Core.Module.Api.LocalApi;
using System;
using System.Collections.Generic;


namespace Vit.Extensions
{
    internal static partial class ILocalApiService_LoadSerslotExtApi_Extensions
    {

        /// <summary>
        /// load api from appsettings.json::serslot.extApi
        /// </summary>
        /// <param name="data"></param>
        /// <param name="CreateApiNode"></param>
        internal static void LoadSerslotExtApi(this ILocalApiService data, Func<SsApiDesc, IApiNode> CreateApiNode = null)
        {
            if (CreateApiNode == null) CreateApiNode = apiDesc => new ApiNode_Original(apiDesc: apiDesc);

            Vit.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetByPath<List<SsApiDesc>>("serslot.extApi")?.ForEach(apiDesc =>
            {
                IApiNode apiNode;

                var reply = apiDesc.extendConfig?["reply"];

                if (reply == null)
                {
                    //(x.x.1)由host处理
                    apiNode = CreateApiNode(apiDesc);
                }
                else
                {
                    //(x.x.2)直接返回 指定的数据
                    var replyBytes = reply.SerializeToBytes();
                    #region onInvoke
                    Func<ArraySegment<byte>, byte[]> onInvoke = (ArraySegment<byte> arg_OriData) =>
                    {
                        return replyBytes;
                    };
                    #endregion

                    apiNode = new ApiNode_Original(onInvoke, apiDesc);
                }

                data.ApiNodeMng.AddApiNode(apiNode);
            });

        }

    }
}
