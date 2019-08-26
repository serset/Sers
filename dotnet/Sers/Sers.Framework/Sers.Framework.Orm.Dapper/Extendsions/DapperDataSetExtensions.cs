using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
 

namespace Sers.Core.Extensions
{
    
    public static  class DapperDataSetExtensions
    {
        /// <summary>
        /// ( DapperDataSetExtensions)
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static DataTable ExecuteDataTable(this IDbConnection conn, string sql,object param=null)
        {
            DataTable table = new DataTable();
            using (var reader = conn.ExecuteReader(sql, param))
            {
                table.Load(reader);
                return table;
            }
        }         

    }
}
