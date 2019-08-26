using Microsoft.Data.Sqlite;

namespace Sers.Framework.Orm.Dapper.SqlHelp
{
    public class MsSqliteHelp
    {
        public static SqliteConnection GetConnection(string filePath)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = filePath;

            return new SqliteConnection(connectionStringBuilder.ConnectionString);
        }


        public static SqliteConnection GetOpenConnection(string filePath)
        {
            var connection = GetConnection(filePath);
            connection.Open();
            return connection;
        }
    }
}
