using System.Linq;
using App.AuthCenter.Logical.Contract.Account;
using App.AuthCenter.Logical.Contract.Token;
using App.AuthCenter.Logical.Entity;
using Dapper.Contrib.Extensions;
using Sers.Core.Extensions;
using Sers.Core.Module.Api;
using Sers.Core.Module.Api.Data;
using Sers.Core.Util.SsError;

namespace App.AuthCenter.Logical.Logical
{
    public class AccountLogical
    {
       


        public static void Login(LoginArg arg,ApiReturn<AuthToken> result)
        {

            AccountData user= FindUser(arg.account,""+arg.pwd);           

            if (null == user)
            {
                result.success = false;
                result.error = new SsError
                {                              
                    errorMessage="用户名或密码不正确",                    
                    errorTag="lith_181225_01"
                };
                return;
            }



            #region Create Token
            var token=TokenLogical.GenerateToken(user);
            #endregion

            result.data = token;
            return;
        }



        public static void VerifyAt(string at, ApiReturn<AuthToken> result)
        {

            var token=TokenLogical.Redis_TokenGetByAt(at);
            if (null == token)
            {
                result.success = false;
                result.error = new SsError
                {                
                    errorMessage = "无效的access token",
                    errorTag = "lith_181225_02"
                };
                return;
            }
            result.data = token;
            return;
        }


        public static void Regist(RegistArg arg, ApiReturn  result)
        {
            AccountData user;
            #region 数据库            
            using (var conn = Database.Database.GetOpenConnection())
            {

                var list = conn.GetAll<AccountData>();
                user = list.FirstOrDefault((m) => m.account == arg.account);
                if (null != user)
                {
                    result.success = false;
                    result.error = new SsError
                    {
                        errorMessage = "用户名已经存在",
                        errorTag = "lith_181225_03"
                    };
                    return;
                }
                
                user = arg.ConvertBySerialize<AccountData>();

                conn.Insert(user);

            }
            #endregion
                                  
 
            return;
        }


        static AccountData FindUser(string account, string pwd = null)
        {
            #region 查数据库            
            using (var conn = Database.Database.GetOpenConnection())
            {
                var list = conn.GetAll<AccountData>();
                if (null == pwd)
                {
                    return list.FirstOrDefault((m) => m.account == account);
                }
                else
                {
                    return list.FirstOrDefault((m) => m.account == account && m.pwd == pwd);
                }
            }
            #endregion
        }
    }
}
