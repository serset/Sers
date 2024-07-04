using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using Sers.Core.Module.Message;
using Sers.Core.Module.Rpc;

using Vit.Core.Module.Log;
using Vit.Extensions;
using Vit.Extensions.Serialize_Extensions;
using Vit.Extensions.Newtonsoft_Extensions;

namespace Sers.Core.Module.Api.ApiEvent.BeforeCallApi.AccountInCookie
{
    /// <summary>
    /// 在调用接口前，会获取 rpcData.http.headers.Cookie(格式为 "user=xxx;c=7")中的user，在账号列表中比对userToken，回写 CallerSource(rpcData.caller.source) 和 userInfo(rpcData.user.userInfo)
    /// </summary>
    public class AccountInCookie : IBeforeCallApi
    {
        class Account
        {
            public string userToken;
            public string CallerSource;
            public object userInfo;
        }

        SortedDictionary<string, Account> userMap = new SortedDictionary<string, Account>();


        /// <summary>
        ///  Sers.AccountInCookie.account:            [ {"userToken":"admin_123","CallerSource":"Internal","userInfo":{}  }      ]  
        /// </summary>
        /// <param name="config"></param>
        public void Init(JObject config)
        {
            userMap.Clear();

            var acounts = config["account"].Deserialize<Account[]>();

            if (acounts == null || acounts.Length == 0) return;

            #region 构建 userMap
            foreach (var account in acounts)
            {
                try
                {
                    if (!string.IsNullOrEmpty(account.userToken))
                        userMap[account.userToken] = account;
                }
                catch (Exception)
                {
                }
            }
            #endregion

        }

        /// <summary>
        /// 转换Cookie为对应的用户
        /// </summary>
        /// <param name="rpcData"></param>
        /// <param name="requestMessage"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void BeforeCallApi(RpcContextData rpcData, ApiMessage requestMessage)
        {
            try
            {
                #region (x.x.x.1)获取cookie 中的用户令牌
                string authUserFromCookie = rpcData.Cookie_Get("user");
                if (string.IsNullOrEmpty(authUserFromCookie)) return;
                #endregion

                #region (x.x.x.2) 转换用户身份并写入 rpcData
                if (userMap.TryGetValue(authUserFromCookie, out var account))
                {
                    if (account.CallerSource != null)
                    {
                        rpcData.caller.source = account.CallerSource;
                    }
                    if (account.userInfo != null)
                    {
                        rpcData.user = new { userInfo = account.userInfo };
                    }
                    requestMessage.rpcContextData_OriData = ArraySegmentByteExtensions.Null;
                }
                #endregion
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}
