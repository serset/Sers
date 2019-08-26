using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Sers.Core.Module.Api;
using Sers.Core.Module.Api.Data;
using Sers.Core.Module.ApiDesc.Attribute;
using Sers.Core.Module.SsApiDiscovery;
using Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute;
using Dapper;
using Sers.Core.Extensions;
using System.Data;
using Newtonsoft.Json.Linq;
using Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute.Valid;
using Sers.Framework.Orm.Dapper.SqlHelp;

namespace App.DataPort.Station.Controllers.MySql
{
    public class ExecuteController : IApiController
    {
  
        /// <summary>
        /// 执行sql语句，返回多个结果集
        /// </summary>
        /// <param name="ConnectionString"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        [SsRoute("mysql/execute/tables")]
        [SsEqualNotNull(path = "caller.source", value = "GoverGateway", errorMessage = "无权限")]
        public ApiReturn<object> ExecuteDataSet(
            [SsExample("Data Source=c7.sersms.com;Database=db_authCenter;SslMode=none;User Id=root;Password=123456;CharSet=utf8;")]string ConnectionString,
            [SsExample("select 1 as s,@p2 as v2;select @p1 as p1;")]string sql,
            [SsExample("{\"p1\":\"v1\",\"p2\":\"v2\"}")]IDictionary<string,object> param=null,
            [SsExample("tb_order,tb_order_assign")]string[] tableName = null)
        {
            DataSet ds;
            using (var conn= MySqlHelp.GetOpenConnection(ConnectionString))
            {
                ds= conn.ExecuteDataSet(sql, param);
            }

            if (tableName != null && tableName.Length != 0)
            {
                for (int i = Math.Min(tableName.Length, ds.Tables.Count) - 1; i >= 0; i--)
                {
                    ds.Tables[i].TableName = tableName[i];
                }
            }
            return ds;
        }


        /// <summary>
        /// 执行sql语句，返回结果集
        /// </summary>
        /// <param name="ConnectionString"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        [SsRoute("mysql/execute/table")]
        [SsEqualNotNull(path = "caller.source", value = "GoverGateway", errorMessage = "无权限")]
        public ApiReturn<object> ExecuteDataTable(
            [SsExample("Data Source=c7.sersms.com;Database=db_authCenter;SslMode=none;User Id=root;Password=123456;CharSet=utf8;")]string ConnectionString,
            [SsExample("select 1 as s,@p2 as v2;select @p1 as p1;")]string sql,
            [SsExample("{\"p1\":\"v1\",\"p2\":\"v2\"}")]IDictionary<string, object> param = null)
        {

            using (var conn = MySqlHelp.GetOpenConnection(ConnectionString))
            {
                return conn.ExecuteDataTable(sql, param);
            }
        }


        /// <summary>
        /// 执行sql语句，返回影响行数
        /// </summary>
        /// <param name="ConnectionString"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        [SsRoute("mysql/execute")]
        [SsEqualNotNull(path = "caller.source", value = "GoverGateway", errorMessage = "无权限")]
        public ApiReturn<int> Execute(
            [SsExample("Data Source=c7.sersms.com;Database=db_authCenter;SslMode=none;User Id=root;Password=123456;CharSet=utf8;")]string ConnectionString,
            [SsExample("select 1 as s,@p2 as v2;select @p1 as p1;")]string sql,
            [SsExample("{\"p1\":\"v1\",\"p2\":\"v2\"}")]IDictionary<string, object> param = null)
        {

            using (var conn = MySqlHelp.GetOpenConnection(ConnectionString))
            {
                return conn.Execute(sql, param);
            }
        }

    }
}
