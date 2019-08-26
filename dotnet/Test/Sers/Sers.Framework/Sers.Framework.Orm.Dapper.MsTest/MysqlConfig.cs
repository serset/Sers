using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Framework.Orm.Dapper.MsTest
{
    public class MysqlConfig
    {
        const string ConnectionString = "Database=his;Data Source=localhost;SslMode = none;User Id=root;Password=Mysql123#;CharSet=utf8;";
        public static  IDbConnection GetConnection()
        {
          
            return new MySql.Data.MySqlClient.MySqlConnection(ConnectionString);
        }


        public static IDbConnection GetOpenConnection()
        {
            var connection = GetConnection();
            connection.Open();
            return connection;
        }
    }
}
