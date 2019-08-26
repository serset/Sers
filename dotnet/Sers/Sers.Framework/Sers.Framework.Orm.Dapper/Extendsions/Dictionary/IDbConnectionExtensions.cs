using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Dapper;

namespace Sers.Core.Extensions
{
    public static partial class IDbConnectionExtensions
    {

        #region Insert    

        /// <summary>
        /// (Lith Framework)把数据模型插入到数据库表中
        /// <para> 返回值：新插入数据自增列的值</para>
        /// <para> 若数据模型中没有有效数据则抛异常 </para>
        /// <para> 会清空所有数据库参数 </para>
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="model"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static bool Insert(this IDbConnection conn, IDictionary model, String tableName)
        {
            StringBuilder builder1 = new StringBuilder(" insert into ").Append(tableName).Append(" ( ");
            StringBuilder builder2 = new StringBuilder("@");

            DynamicParameters sqlParam = new DynamicParameters();

            String fieldName;
            Object value;
            foreach (DictionaryEntry entry in model)
            {
                if (null != (value = entry.Value))
                {
                    fieldName = entry.Key as string;

                    builder1.Append(fieldName).Append(",");
                    builder2.Append(fieldName).Append(",@");
                    sqlParam.Add(fieldName,value);
                }
            }


            if (builder2.Length < 2)
            {
                throw new Exception("没有传入有效数据");
            }

            builder1.Length -= 1;
            builder2.Length -= 2;
            builder1.Append(") values(").Append(builder2).Append(");");

            return conn.Execute(builder1.ToString(), sqlParam) >0;

        }
        #endregion


        #region Update
        public static int Update(this IDbConnection conn, IDictionary model, String tableName, string sqlWhere, DynamicParameters sqlParam = null, bool sendNullField=false)
        {
            if (sqlParam == null) sqlParam = new DynamicParameters();

            StringBuilder builder = new StringBuilder(" update ").Append(tableName).Append(" set ");

            string fieldName;
            Object value;

            if (sendNullField)
            {
                foreach (DictionaryEntry entry in model)
                {
                    fieldName = entry.Key.ToString();
                    value = entry.Value;
                    builder.Append(fieldName).Append("= @").Append(fieldName).Append(",");
                
                    sqlParam.Add(fieldName, value);
                }
            }
            else
            {
               
                foreach (DictionaryEntry entry in model)
                {
                    if (null != (value = entry.Value))
                    {
                        fieldName = entry.Key.ToString();
                        builder.Append(fieldName).Append("= @").Append(fieldName).Append(",");
                        sqlParam.Add(fieldName, value);
                    }
                }
            }

            if (',' != builder[builder.Length - 1])
            {
                throw new Exception("没有传入有效数据");
            }

            builder.Length -= 1;
            builder.Append(" where  1=1 ").Append(sqlWhere);
            return conn.Execute(builder.ToString(), sqlParam);
        }
        #endregion

    }
}
