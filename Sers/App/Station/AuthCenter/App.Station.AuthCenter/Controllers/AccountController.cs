using App.AuthCenter.Logical.Contract.Account;
using App.AuthCenter.Logical.Contract.Token;
using App.AuthCenter.Logical.Logical;
using Newtonsoft.Json.Linq;
using Sers.Core.Extensions;
using Sers.Core.Module.Api;
using Sers.Core.Module.Api.Data;
using Sers.Core.Module.ApiDesc.Attribute;
using Sers.Core.Module.Rpc;
using Sers.Core.Module.SsApiDiscovery;
using Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute;

namespace App.AuthCenter.Api.Controllers
{

    //站点名称，可多个。若不指定，则从 配置文件的节点"Sers.Station.Name"获取
    [SsStationName("AuthCenter")]
    public class AccountController : IApiController
    {
        /// <summary>
        /// 校验传入的at
        /// </summary>
        /// <param name="at"></param>
        /// <returns></returns>
        [SsRoute("account/verifyAt")]
        [SsName("校验at")]
        public ApiReturn<AuthToken> VerifyAt(string at)
        {
            var result = new ApiReturn<AuthToken>();
            AccountLogical.VerifyAt(at, result);
            return result;
        }


        /// <summary>
        /// 使用用户名密码登录
        /// </summary>
        /// <param name="arg"></param>
        /// <returns>登陆票据</returns>
        [SsRoute("account/login")]
        [SsName("账户登录")]      
        public ApiReturn<AuthToken> Login(LoginArg arg)
        {
            var result = new ApiReturn<AuthToken>();
            AccountLogical.Login(arg, result);
            return result;
        }

        /// <summary>
        /// 添加新的登录账户
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        [SsRoute("account/regist")]
        [SsName("添加账户")]       
        public ApiReturn Regist(RegistArg arg)
        {
            var result = new ApiReturn();
            AccountLogical.Regist(arg, result);
            return result;
        }

        /// <summary>
        /// 获取当前用户rpc信息
        /// </summary>
        /// <returns></returns>
        [SsRoute("account/getUserInfo")]
        [SsName("获取用户信息")]      
        public ApiReturn<JObject> Get()
        {
            var result = new ApiReturn<JObject>();
            result.data=RpcContext.RpcData.user_userInfo_Get();
            return result;
        }



    }
}
