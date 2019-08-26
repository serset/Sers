using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sers.Core.Module.Api.Data;
using Sers.Core.Module.Sql.List;

namespace Sers.Core.Module.Sql.Mysql
{
    public class SqlBuild
    {

        public PageInfo pager;
        public IEnumerable<DataFilter>  filter;
        public IEnumerable<SortItem> sort;
        public Action<string, object> addSqlParam;



        /// <summary>
        /// " and name like @sf_name   and name2 like @sf_name2 "
        /// </summary>
        public string sqlWhere;
        /// <summary>
        /// " name asc,id desc "
        /// </summary>
        public string sqlOrderBy;
        /// <summary>
        /// " limit 1000,10 "
        /// </summary>
        public string sqlLimit;

       


        #region Build


        public SqlBuild Build()
        {
            /*
             select * from tb_order 
             where  1=1         
             and  filter1 like value1

             order by id desc
             limit {(pager.pageIndex - 1) * pager.pageSize}, {pager.pageSize}; 
             */
            string sql;

            #region (x.2)filter    
            sql = "";
            if (null != filter)
            {
                foreach (var item in filter)
                {
                    if (!ValidFieldName(item.fieldName))
                    {
                        throw new Exception("筛选条件存在不合法的字段名。");
                    }

                    if (!ValidOpt(item.opt))
                    {
                        throw new Exception("筛选条件存在不合法的操作符");
                    }
                    string paramName = item.sqlParamName;
                    if (string.IsNullOrWhiteSpace(paramName))
                    {
                        paramName = "sf_" + item.fieldName;
                    }
                    sql += " and " + item.fieldName + " " + item.opt + " @" + paramName;
                    addSqlParam(paramName, item.value);
                }
            }
            sqlWhere = sql;
            #endregion


            #region (x.3)sort
            if (null == sort)
            {
                sqlOrderBy = null;
            }
            else
            {
                sqlOrderBy = "";
                foreach (var item in sort)
                {
                    if (!ValidFieldName(item.fieldName))
                    {
                        throw new Exception("排序条件存在不合法的字段名。");
                    }
                    sqlOrderBy += "," + item.fieldName + " " + (item.isAsc ? " asc" : " desc");
                }

                if (string.IsNullOrEmpty(sqlOrderBy))
                {
                    return null;
                }
                else
                {
                    sqlOrderBy = sqlOrderBy.Substring(1);
                }
                sqlLimit = null == pager ? null : $" limit {(pager.pageIndex - 1) * pager.pageSize}, {pager.pageSize} ";
            }
            #endregion

            #region (x.4)limit
            sqlLimit = null == pager ? null : $" limit {(pager.pageIndex - 1) * pager.pageSize}, {pager.pageSize} ";
            #endregion
            return this;
        }


        //判断输入的字符串是否只包含 数字、大小写字母、下划线 和 .
        static readonly Regex regex = new Regex(@"^[A-Za-z0-9\\_\\.]+$");
        static bool ValidFieldName(string input)
        {            
            return regex.IsMatch(input);
        }


        //判断比较操作符是否合法
        static readonly string[] opts = new[] { "=", "!=", "like", ">", "<" , "<=", ">=", "in", "not in"};
        static bool ValidOpt(string input)
        {            
            return opts.Contains(input);
        }
        #endregion
    }
}
