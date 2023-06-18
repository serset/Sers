using Newtonsoft.Json.Linq;
using Sers.Core.Module.Rpc;
using System;
using Sers.Core.Module.Message;
using Vit.Core.Module.Log;
using Vit.Extensions;
using Vit.Extensions.Json_Extensions;
using Vit.Extensions.Newtonsoft_Extensions;
using Vit.SSO.Service.RS256;

namespace Sers.Core.Module.Api.ApiEvent.BeforeCallApi.JsonWebToken
{
    /// <summary>
    /// 在调用接口前，会获取 rpcData.http.headers.Authorization(格式为 "Bearer xxxxxx")，或cookie中的token， 并把jwt中的Claims信息放到 rpcData.user.userInfo
    /// </summary>
    public class JsonWebToken : IBeforeCallApi
    {

        // if true,when token is invalid,will block the request and return 401
        //"BlockIfInvalid": false,
        //bool? BlockIfInvalid;

  
        string CallerSource;
        JwtValidateService_RS256Base jwtService;

        public void Init(JObject config)
        { 
            CallerSource = config["CallerSource"]?.Value<String>(); 
            jwtService = config.Deserialize<JwtValidateService_RS256Base>();
        }




        /// <summary>
        /// Bearer。 转换at为对应的用户
        /// </summary>
        /// <param name="rpcData"></param>
        /// <param name="requestMessage"></param>
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public void BeforeCallApi(RpcContextData rpcData, ApiMessage requestMessage)
        {
            try
            {
                if (null != rpcData.user) return;

                // #1 get token
                var token = rpcData.Authorization_Get();
                if (string.IsNullOrWhiteSpace(token))
                    token = rpcData.Cookie_Get("Authorization");
                if (string.IsNullOrWhiteSpace(token))
                    return;

                // #2 ValidateToken
                var userInfo = jwtService.ValidateToken(token);

                // #3 set rpcData.user
                if (null != userInfo)
                {
                    if (!string.IsNullOrWhiteSpace(CallerSource))
                    {
                        rpcData.caller.source = CallerSource;
                    }
                    rpcData.user = new { userInfo = userInfo.Claims };
                    requestMessage.rpcContextData_OriData = ArraySegmentByteExtensions.Null;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}
