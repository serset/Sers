using System;
using App.AuthCenter.Logical.Contract.Token;
using App.AuthCenter.Logical.Entity;
using Sers.Core.Db.Redis;
using Sers.Core.Util.Common;
using Sers.Core.Util.ConfigurationManager;

namespace App.AuthCenter.Logical.Logical
{
    public class TokenLogical
    {

        /// <summary>
        /// 以分钟为单位的AccessToken过期时间
        /// </summary>
        protected static int At_ExpiresMinutes= ConfigurationManager.Instance.Get<int>("AuthConfig", "At_ExpiresMinutes");


        /// <summary>
        /// 以分钟为单位的RefreshToken过期时间
        /// </summary>
        protected static int Rt_ExpiresMinutes = ConfigurationManager.Instance.Get<int>("AuthConfig", "Rt_ExpiresMinutes");




        public static AuthToken GenerateToken(AccountData user)
        {

            var token = GeneraNullToken();
            token.userId = user.userId;
            token.name = user.name;
            Redis_TokenCache(token);
            return token;
        }




        #region Redis

        
        static void Redis_TokenCache(AuthToken token)
        {
            using (var db = new DbRedis())
            {
              
                string[] key = { "auth", "token" ,"at", token.at};
              
                db.Set(token, token.at_ExpiresTime, key);

                key[2] = "rt";
                key[3] = token.rt;
                db.Set(token, token.rt_ExpiresTime, key);
            }
        }

        public static AuthToken Redis_TokenGetByAt(string at)
        {
            using (var db = new DbRedis())
            {
                return db.Get<AuthToken>("auth", "token", "at", at);
            }
        }
        public static AuthToken Redis_TokenGetByRt(string rt)
        {
            using (var db = new DbRedis())
            {
                return db.Get<AuthToken>("auth", "token", "rt", rt);
            }
        }
        #endregion


        #region GeneraNullToken

        private static AuthToken GeneraNullToken()
        {
            var token = new AuthToken
            {
                addTime = DateTime.Now
            };

            token.at = "at" + CommonHelp.NewGuid();
            token.rt = "rt" + CommonHelp.NewGuid();

            token.at_ExpiresTime = token.addTime.AddMinutes(At_ExpiresMinutes);
            token.rt_ExpiresTime = token.addTime.AddMinutes(Rt_ExpiresMinutes);
            return token;
        }
        #endregion

    }
}
