using MySql.Data.MySqlClient;

namespace Sers.Framework.Orm.Dapper.SqlHelp
{
    public class MySqlHelp
    {
        public static MySqlConnection GetConnection(string ConnectionString)
        {

            return new MySql.Data.MySqlClient.MySqlConnection(ConnectionString);
        }


        public static MySqlConnection GetOpenConnection(string ConnectionString)
        {
            var connection = GetConnection(ConnectionString);
            connection.Open();
            return connection;
        }


    }
}
