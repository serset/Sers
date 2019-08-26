using System.Data;
using Sers.Core.Util.ConfigurationManager;

namespace App.AuthCenter.Logical.Database
{
    public class Database
    {
        static string ConnectionString = ConfigurationManager.Instance.GetByPath<string>("ConnectionStrings.AuthCenter");
        public static IDbConnection GetConnection()
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
