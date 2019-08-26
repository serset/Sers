using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Dapper;

using SqlConnection = MySql.Data.MySqlClient.MySqlConnection;
using SqlCommand = MySql.Data.MySqlClient.MySqlCommand;
using SqlDataAdapter = MySql.Data.MySqlClient.MySqlDataAdapter;
using SqlDataReader= MySql.Data.MySqlClient.MySqlDataReader;


namespace Sers.Core.Extensions
{
    public static partial class MySqlConnectionExtensions
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
            using(var cmd = new SqlCommand())
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
            
            StringBuilder sql = new StringBuilder("Create Table ").Append(dt.TableName).Append(" (");


            foreach (DataColumn dc in dt.Columns)
            {
                string columnName = dc.ColumnName;
                Type type = dc.DataType;

                sql.Append(columnName).Append(" ").Append(GetDbType(type)).Append(",");
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
                return "int";
            }
            if (type == typeof(long))
            {
                return "bigint";
            }

            if (type == typeof(float))
            {
                return "float";
            }
            if (type == typeof(double))
            {
                return "double";
            }

            if (type == typeof(DateTime))
            {
                return "datetime";
            }
             
            if (type == typeof(string))
            {
                return "text";
            }

            return "text";
        }
        #endregion
        #endregion


        #region GetAllTableName
        /// <summary>
        /// 获取所有的表名
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static List<string> GetAllTableName(this SqlConnection conn)
        {
            var rows = conn.ExecuteDataTable("show tables;").Rows;
            var tables = new List<string>();
            foreach (DataRow row in rows)
            {
                tables.Add(row[0] as string);
            }
            return tables;

        }
        #endregion

        #region Import


        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="dt"></param>
        /// <param name="useTransaction">是否使用事务</param>
        public static void Import(this SqlConnection conn, DataTable dt, bool useTransaction = false)
        {

            if (null == dt || dt.Columns.Count == 0 || dt.Rows.Count == 0) return;

            //创建表结构的SQL语句

            // CREATE TABEL IF NOT EXISTS ，一般情况下用这句比较好，如果原来就有同名的表，没有这句就会出错
            StringBuilder sql = new StringBuilder("insert into ").Append(dt.TableName).Append("(");


            using (var cmd = new SqlCommand())
            {

                foreach (DataColumn dc in dt.Columns)
                {
                    string columnName = dc.ColumnName;

                    sql.Append(columnName).Append(",");
                }
                sql.Length--;
                sql.Append(") values(");

                var param = cmd.Parameters;
                var item = dt.Rows[0];
                foreach (DataColumn dc in dt.Columns)
                {
                    string columnName = dc.ColumnName;

                    sql.Append("@").Append(columnName).Append(",");
                    //sql.Append("?,");

                    param.AddWithValue(columnName, item[dc]);
                }
                sql.Length--;
                sql.Append(")");

                cmd.Connection = conn;
                cmd.CommandText = sql.ToString();

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



        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static int Import(this SqlConnection conn, DataSet ds)
        {
            using (var trans = conn.BeginTransaction())
            {
                try
                {
                    int sumCount = 0;
                    foreach (DataTable dt in ds.Tables)
                    {
                        conn.Import(dt,false);
                        sumCount += dt.Rows.Count;
                    }
                    trans.Commit();
                    return sumCount;
                }
                catch (Exception)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }
        #endregion

    }
}
