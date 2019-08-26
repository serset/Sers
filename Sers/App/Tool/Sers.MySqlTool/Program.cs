
using Dapper;
using Sers.Core.Extensions;
using Sers.Core.Module.Log;
using Sers.Core.Util.Common;
using Sers.Framework.Orm.Dapper.SqlHelp;
using Sers.MysqlTool.Logical;
using System;
using System.Data;
using System.Linq;

namespace Main
{
    public class Program
    {


        /// <summary>
        /// dotnet Sers.MySqlTool.dll backup
        /// 
        /// dotnet Sers.MySqlTool.dll import "mysqlConnectionString" "sqliteFilePath" ["-createTable"] ["-truncate"]
        /// 
        ///    "-createTable": 若指定，则在导入数据前自动创建表结构
        ///    "-truncate": 若指定，则在导入数据前自动清空待操作表的原始数据
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {

            //args = new[] { "backup" };
            //args = new[] { "import", "Data Source=lj.sersms.com;Port=7006;Database=db_import;SslMode=none;User Id=root;Password=123456;CharSet=utf8;", "T:\\db_test.db" };

            Logger.OnLog = (level, msg) => { Console.WriteLine("[" + level + "]" + msg); };

            var commond = (args!=null && args.Length>0)? args[0] :null;
            try
            {
                switch (("" + commond).ToLower())
                {
                    case "backup": Backup(args); return;
                    case "import": Import(args); return;
                }
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex);
                Logger.log.Info(ex);
                return;
            }
          

            Logger.log.Info(" none valid commond.");
            return;
        }


        #region Backup
        class Backup_Config
        {
            // "dbName": "test",
            // "mySqlConnectionString": ""
            public string dbName { get; set; }
            public string mySqlConnectionString { get; set; }
        }
        static void Backup(string[] args)
        {
            #region backup database           
            try
            {
                Logger.log.Info("  backup database start...");

                foreach (var item in Sers.Core.Util.ConfigurationManager.ConfigurationManager.Instance.GetByPath<Backup_Config[]>("MysqlTool.BackupItems"))
                {
                    string sqliteFilePath = CommonHelp.GetAbsPathByRealativePath(new[] { "Data", "MysqlBackup",
                        item.dbName+"("+DateTime.Now.ToString("yyyy-MM-dd HHmmss")+")"+CommonHelp.NewGuidLong()+".db" });

                    BackupMySqlHelp.BackupMysqlToSqlite(item.mySqlConnectionString, sqliteFilePath);
                }
                Logger.log.Info("  backup database finish。");
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex);
            }
            #endregion
        }
        #endregion


        #region Import

        static void Import(string[] args)
        {
            #region (x.1)import data
            if (args.Length <= 2)
            {
                Logger.log.Info("Import commond - invalid args length .");
                return;
            }
            string mysqlConnectionString = args[1];
            string sqliteFilePath = args[2];
            if (string.IsNullOrWhiteSpace(mysqlConnectionString) && string.IsNullOrWhiteSpace(sqliteFilePath))
            {
                Logger.log.Info("Import commond - invalid args .");
                return;
            }


            DataSet ds;
            using (var conn = SqliteHelp.GetOpenConnection(sqliteFilePath))
            {
                ds = conn.GetAllData();
            }

            bool createTable = (Array.IndexOf(args, "-createTable") >= 0);
            bool truncate = (Array.IndexOf(args, "-truncate") >= 0);            


            using (var conn = MySqlHelp.GetOpenConnection(mysqlConnectionString))
            {
                Logger.log.Info("    [x]start import data ");
                if (createTable)
                {
                    Logger.log.Info("        [x.x]create table ");
                    foreach (DataTable dt in ds.Tables)
                    {
                        conn.CreateTable(dt);
                    }
                }

                if (truncate)
                {
                    Logger.log.Info("        [x.x]truncate table ");
                    foreach (DataTable dt in ds.Tables)
                    {
                        conn.Execute("truncate table "+dt.TableName+"");
                    }
                }
                Logger.log.Info("        [x.x]import data ");
                conn.Import(ds);
                Logger.log.Info("    import data success");
            }
        
            #endregion
        }
        #endregion

    }
}
