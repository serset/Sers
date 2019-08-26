using Dapper;
using Sers.Core.Extensions;
using Sers.Core.Module.Log;
using System;
using System.Data;
using Sers.Framework.Orm.Dapper.SqlHelp;

namespace Sers.MysqlTool.Logical
{
    public class BackupMySqlHelp
    {

        public static void BackupMysqlToSqlite(string mysqlConnectionString ,string sqliteConnectionString)
        {
            try
            {
                using (var connMysql = MySqlHelp.GetOpenConnection(mysqlConnectionString))
                using (var connSqlite = SqliteHelp.GetOpenConnection(sqliteConnectionString))
                {
                    var taNames = connMysql.GetAllTableName();
                    foreach (var tableName in taNames)
                    {
                        try
                        {
                            Logger.Info("    [x]start backup table " + tableName);
                            Logger.Info("        [x.1]create table " + tableName);
                            var dt = connMysql.ExecuteDataTable($"select * from `{tableName}` limit 1;");
                            connSqlite.CreateTable(dt);

                            Logger.Info("        [x.2]import table " + tableName+ " start...");

                            using (IDataReader dr = connMysql.ExecuteReader($"select * from `{tableName}`;"))
                            {
                                connSqlite.Import(tableName, dr);
                            }
                            Logger.Info("             import table " + tableName + " success");
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

        }

    }
}
