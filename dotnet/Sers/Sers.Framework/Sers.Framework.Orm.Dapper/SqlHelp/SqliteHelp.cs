using System.Data.SQLite;
using SqlConnection = System.Data.SQLite.SQLiteConnection;

namespace Sers.Framework.Orm.Dapper.SqlHelp
{
    public class SqliteHelp
    {
        public static SqlConnection GetConnection(string filePath)
        {    
           var connectionStringBuilder = new SQLiteConnectionStringBuilder();
            connectionStringBuilder.DataSource = filePath;
            return new SqlConnection(connectionStringBuilder.ConnectionString);
        }


        public static SqlConnection GetOpenConnection(string filePath)
        {
            var connection = GetConnection(filePath);
            connection.Open();
            return connection;
        }
    }
}
