

using Dapper.Contrib.Extensions;
using System;
using System.Data;

namespace Sers.Core.Extensions
{
    /// <summary>
    /// Extension methods for RpcContextData
    /// </summary>
    public static class SqlMapperExtensions
    {

        /// <summary>
        /// 更新表，若更新失败则返回null
        /// </summary>
        /// <typeparam name="DbModel"></typeparam>
        /// <param name="conn"></param>
        /// <param name="keyValue"></param>
        /// <param name="howToChangeData"></param>
        /// <returns></returns>
        public static DbModel Update<DbModel>(this IDbConnection conn, object keyValue, Action<DbModel> howToChangeData) where DbModel : class
        {
            var dbData = conn.Get<DbModel>(keyValue);
            if (null == dbData) return null;

            howToChangeData(dbData);

            return conn.Update(dbData) ? dbData : null;
        }



        /// <summary>
        /// 更新表，若更新失败则返回null
        /// </summary>
        /// <typeparam name="DbModel"></typeparam>
        /// <typeparam name="UserModel"></typeparam>
        /// <param name="conn"></param>
        /// <param name="keyValue"></param>
        /// <param name="userData"></param>
        /// <param name="howToChangeData"></param>
        /// <returns></returns>
        public static DbModel Update<DbModel, UserModel>(this IDbConnection conn,object keyValue, UserModel userData, Action<DbModel, UserModel> howToChangeData) where DbModel : class
        {
            var dbData = conn.Get<DbModel>(keyValue);
            if (null == dbData) return null;

            howToChangeData(dbData, userData);

            return conn.Update(dbData) ? dbData : null;
        }


      







    }
}
