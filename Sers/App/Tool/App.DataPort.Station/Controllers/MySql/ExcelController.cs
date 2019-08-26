using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Sers.Core.Module.Api;
using Sers.Core.Module.Api.Data;
using Sers.Core.Module.ApiDesc.Attribute;
using Sers.Core.Module.SsApiDiscovery;
using Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute;
using Dapper;
using Sers.Core.Extensions;
using System.Data;
using Newtonsoft.Json.Linq;
using Sers.Core.Util.Common;
using System.IO;
using Sers.Core.Module.Log;
using Sers.Core.Util.SsError;
using Sers.Core.Module.SsApiDiscovery.ApiDesc.Attribute.Valid;
using Sers.ServiceStation.Util.StaticFileTransmit;
using Sers.Core.Module.Rpc;
using Sers.Framework.Data.Excel;
using Sers.Framework.Orm.Dapper.SqlHelp;

namespace App.DataPort.Station.Controllers.MySql
{
    public class ExcelController : IApiController
    {

        #region Export        

        /// <summary>
        /// 把执行结果作为sqlite文件返回
        /// </summary>
        /// <param name="ConnectionString">mysql连接字符串</param>
        /// <param name="sql">sql语句</param>
        /// <param name="param">sql参数</param>
        /// <param name="tableName">结果集的表名</param>
        /// <param name="fileName">导出文件的名称</param>
        /// <returns></returns>
        [SsRoute("mysql/excel/export")]
        [SsEqualNotNull(path = "caller.source", value = "GoverGateway", errorMessage = "无权限")]
        public byte[] Export(
            [SsExample("Data Source=c7.sersms.com;Database=db_authCenter;SslMode=none;User Id=root;Password=123456;CharSet=utf8;")]string ConnectionString,
            [SsExample("select 1 as s,@p2 as v2;select @p1 as p1;")]string sql,
            [SsExample("{\"p1\":\"v1\",\"p2\":\"v2\"}")]IDictionary<string,object> param=null,
            [SsExample("tb_order,tb_order_assign")]string[] tableName = null,
            [SsExample("account.xlsx")]string fileName = null)
        {
            DataSet ds;

            #region (x.1)在mysq中执行对应sql语句，获取返回数据集
            using (var conn= MySqlHelp.GetOpenConnection(ConnectionString))
            {
                ds= conn.ExecuteDataSet(sql, param);
            }
            if (tableName != null && tableName.Length != 0)
            {
                for (int i = Math.Min(tableName.Length, ds.Tables.Count)-1; i >=0; i--)
                {
                    ds.Tables[i].TableName = tableName[i];
                }
            }
            #endregion


            #region (x.2) 把数据集灌入生成的临时数据文件，返回给用户            
            var tempFile = CommonHelp.GetAbsPathByRealativePath("Data","Sers","Temp", CommonHelp.NewGuidLong()+".xlsx");
            try
            {
                //(x.x.1)把数据集灌入生成的临时数据文件
                ExcelHelp.SaveData(tempFile, ds);

                //(x.x.2)返回给用户
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = DateTime.Now.ToString("yyyyMMdd_HHmmss_ffff") + ".xlsx";
                }
                return StaticFileMap.DownloadFile(tempFile, "application/octet-stream", fileName);
            }            
            finally
            {
                #region 删除临时文件                
                try
                {
                    if(File.Exists(tempFile))
                        File.Delete(tempFile);
                }
                catch (Exception ex)
                { 
                }
                #endregion
            }
            #endregion
        }
        #endregion


        #region Import
        /*

        /// <summary>
        /// 导入数据。ConnectionString参数保存在Header中。
        /// </summary>
        /// <param name="fileData"></param>
        /// <returns></returns>
        [SsRoute("mysql/excel/import")]
        [SsEqualNotNull(path = "caller.source", value = "GoverGateway", errorMessage = "无权限")]
        public ApiReturn<int> Import(byte[] fileData)
        {
            DataSet ds;
            #region (x.1) 把内容保存到临时文件，并读取数据集         
            var tempFile = CommonHelp.GetAbsPathByRealativePath("Data", "Sers", "Temp", CommonHelp.NewGuidUlong() + ".xlsx");
            try
            {
                File.WriteAllBytes(tempFile,fileData);

                //(x.x.1)从临时文件获取数据集
                ds=ExcelHelp.ReadData(tempFile);
            }            
            finally
            {
                #region 删除临时文件                
                try
                {
                    if (File.Exists(tempFile))
                        File.Delete(tempFile);
                }
                catch (Exception ex)
                {
                }
                #endregion
            }
            #endregion

            #region (x.2)从header中获取ConnectionString         

            string ConnectionString = RpcContext.RpcData.http_header_Get("ConnectionString");
            if (string.IsNullOrEmpty(ConnectionString))
            {
                return new SsError { errorCode=2000,errorMessage= "请传递正确的ConnectionString" };
            }

            bool createTable =   RpcContext.RpcData.http_header_Get("CreateTable") =="true";
            #endregion


            #region (x.3)把数据集导入mysql
            using (var conn = MySqlHelp.GetOpenConnection(ConnectionString))
            {
                if (createTable)
                {
                    foreach (DataTable dt in ds.Tables)
                    {
                        conn.CreateTable(dt);
                    }
                }
                return conn.Import(ds);
            }
            #endregion
        }

        //*/
        #endregion

    }
}
