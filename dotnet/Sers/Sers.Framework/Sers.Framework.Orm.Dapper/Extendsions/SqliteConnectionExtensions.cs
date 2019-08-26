
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;
using SqlConnection = System.Data.SQLite.SQLiteConnection;
using SqlCommand = System.Data.SQLite.SQLiteCommand;
using SqlDataAdapter = System.Data.SQLite.SQLiteDataAdapter;
using SqlDataReader = System.Data.SQLite.SQLiteDataReader;
using System.Data.SQLite;
using Dapper;

namespace Sers.Core.Extensions
{
    public static partial class SqliteConnectionExtensions
    {

        #region Execute 

        /// <summary>
        /// (Lith Framework)
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(this SqlConnection conn, string sql, IDictionary<string, object> param = null)
        {
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = sql;

                #region param
                if (param != null)
                {
                    foreach (var entry in param)
                    {
                        cmd.Parameters.AddWithValue((string)entry.Key, entry.Value);
                    }
                }
                #endregion

                DataSet ds = new DataSet();
                using (var Adapter = new SqlDataAdapter(cmd))
                {
                    Adapter.Fill(ds);
                }
                return ds;
            }

        }

 
        #region  do not need. use Dapper instand. 

        /*
       

        /// <summary>
        /// (Lith Framework)
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(this SqlConnection conn, string sql, IDictionary<string, object> param = null)
        {
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = sql;

                #region param
                if (param != null)
                {
                    foreach (var entry in param)
                    {
                        cmd.Parameters.AddWithValue((string)entry.Key, entry.Value);
                    }
                }
                #endregion

                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// (Lith Framework)
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static object ExecuteScalar(this SqlConnection conn, string sql, IDictionary<string, object> param = null)
        {
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = sql;

                #region param
                if (param != null)
                {
                    foreach (var entry in param)
                    {
                        cmd.Parameters.AddWithValue((string)entry.Key, entry.Value);
                    }
                }
                #endregion

                return cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// (Lith Framework)
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static SqlDataReader ExecuteReader(this SqlConnection conn, string sql, IDictionary<string, object> param = null)
        {
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = sql;

                #region param
                if (param != null)
                {
                    foreach (var entry in param)
                    {
                        cmd.Parameters.AddWithValue((string)entry.Key, entry.Value);
                    }
                }
                #endregion
                return cmd.ExecuteReader();
            }
        }

        //*/
        #endregion

        #endregion



        #region Vacuum
        /// <summary>
        /// SQLite 的自带命令 VACUUM。用来重新整理整个数据库达到紧凑之用，比如把删除的彻底删掉等等。
        /// </summary>
        /// <param name="conn"></param>
        public static void Vacuum(this SqlConnection conn)
        {
            conn.Execute("VACUUM");
        }
        #endregion

        #region CreateTable        
        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="dt"></param>
        public static void CreateTable(this SqlConnection conn, DataTable dt)
        {

            if (null == dt || dt.Columns.Count == 0) return;

            //创建表结构的SQL语句

            // CREATE TABEL IF NOT EXISTS ，一般情况下用这句比较好，如果原来就有同名的表，没有这句就会出错
            StringBuilder sql = new StringBuilder("Create Table IF NOT EXISTS [").Append(dt.TableName).Append("] (");


            foreach (DataColumn dc in dt.Columns)
            {
                string columnName = dc.ColumnName;
                Type type = dc.DataType;

                sql.Append("[").Append(columnName).Append("] ").Append(GetDbType(type)).Append(",");
            }
            sql.Length--;
            sql.Append(")");
            conn.Execute(sql.ToString());         
        }


        #region GetDbType
        static string GetDbType(Type type)
        {
            if (type == typeof(int))
            {
                return "INT";
            }
            if (type == typeof(long))
            {
                return "INT64";
            }

            if (type == typeof(float))
            {
                return "FLOAT";
            }
            if (type == typeof(double))
            {
                return "DOUBLE";
            }

            if (type == typeof(DateTime))
            {
                return "DATETIME";
            }
            if (type == typeof(bool))
            {
                return "BOOL";
            }

            if (type == typeof(string))
            {
                return "TEXT";
            }

            return "TEXT";
        }
        #endregion
        #endregion


        #region Import


        public static string Import_GetSqlInsert(SqlConnection conn, DataTable dt, SQLiteParameterCollection sqlParam)
        {

            StringBuilder sql = new StringBuilder("insert into [").Append(dt.TableName).Append("] (");

            foreach (DataColumn dc in dt.Columns)
            {
                string columnName = dc.ColumnName;

                sql.Append("[").Append(columnName).Append("],");
            }
            sql.Length--;
            sql.Append(") values(");


            var item = dt.Rows[0];
            foreach (DataColumn dc in dt.Columns)
            {
                string columnName = dc.ColumnName;

                //sql.Append("@").Append(columnName).Append(",");
                sql.Append("?,");

                sqlParam.AddWithValue(columnName, item[dc]);
            }
            sql.Length--;
            sql.Append(")");

            return sql.ToString();
        }


        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="dt"></param>
        /// <param name="useTransaction">是否使用事务</param>
        public static void Import(this SqlConnection conn, DataTable dt, bool useTransaction = true)
        {

            if (null == dt || dt.Columns.Count == 0 || dt.Rows.Count == 0) return;           


            using (var cmd = new SqlCommand())
            {                

                cmd.Connection = conn;
                var param = cmd.Parameters;
                cmd.CommandText = Import_GetSqlInsert(conn,dt, param);

                if (useTransaction)
                {
                    using (var trans = conn.BeginTransaction())
                    {
                        try
                        {
                            foreach (DataRow row in dt.Rows)
                            {
                                for (int t = 0; t < param.Count; t++)
                                {
                                    param[t].Value = row[t];
                                }
                                cmd.ExecuteNonQuery();
                            }
                            trans.Commit();
                        }
                        catch (Exception)
                        {
                            trans.Rollback();
                            throw;
                        }
                    }
                }
                else
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        for (int t = 0; t < param.Count; t++)
                        {
                            param[t].Value = row[t];
                        }
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        #endregion

        #region Import dr

        public static string Import_GetSqlInsert(SqlConnection conn, string tableName,IDataReader dr, SQLiteParameterCollection sqlParam)
        {

            StringBuilder sql = new StringBuilder("insert into [").Append(tableName).Append("] (");

            for (int i = 0; i < dr.FieldCount; i++)
            {
                string columnName = dr.GetName(i).Trim();
                sql.Append("[").Append(columnName).Append("],");
            }
           
            sql.Length--;
            sql.Append(") values(");

            for (int i = 0; i < dr.FieldCount; i++)
            {
                sql.Append("?,");
                sqlParam.AddWithValue("param"+i, dr[i]);
            }
            sql.Length--;
            sql.Append(")");

            return sql.ToString();
        }
       
        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="tableName"></param>
        /// <param name="dr"></param>
        /// <param name="useTransaction">是否使用事务</param>
        public static void Import(this SqlConnection conn,string tableName, IDataReader dr, bool useTransaction = true)
        {
            if (!dr.Read()) return;

            using (var cmd = new SqlCommand())
            {
                cmd.Connection = conn;              

                var param = cmd.Parameters;
                cmd.CommandText = Import_GetSqlInsert(conn, tableName,dr, param);

                if (useTransaction)
                {
                    using (var trans = conn.BeginTransaction())
                    {
                        try
                        {
                            do
                            {
                                for (int t = 0; t < param.Count; t++)
                                {
                                    param[t].Value = dr[t];
                                }
                                cmd.ExecuteNonQuery();
                            } while (dr.Read());
                            trans.Commit();
                        }
                        catch (Exception)
                        {
                            trans.Rollback();
                            throw;
                        }
                    }
                }
                else
                {
                    do
                    {
                        for (int t = 0; t < param.Count; t++)
                        {
                            param[t].Value = dr[t];
                        }
                        cmd.ExecuteNonQuery();
                    } while (dr.Read());
                }
            }
        }
        #endregion


        #region GetAllTableName
        /// <summary>
        /// 获取所有的表名
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static List<string> GetAllTableName(this SqlConnection conn)
        {
            var rows = conn.ExecuteDataTable("SELECT name FROM sqlite_master WHERE type = 'table'").Rows;
            var tables = new List<string>();
            foreach (DataRow row in rows)
            {
                tables.Add(row[0] as string);
            }
            return tables;
             
        }
        #endregion

        #region GetAllData
        /// <summary>
        /// 获取所有的表数据
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static DataSet GetAllData(this SqlConnection conn)
        {             
            var tableNames = conn.GetAllTableName();
            var sql = "select * from " + String.Join(";select * from ", tableNames) +";";
            var ds = conn.ExecuteDataSet(sql);
            for (int t = 0; t < tableNames.Count; t++)
            {
                ds.Tables[t].TableName = tableNames[t];
            }
            return ds;
        }
        #endregion

    }
}
