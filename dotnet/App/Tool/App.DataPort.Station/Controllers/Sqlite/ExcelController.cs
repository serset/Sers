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

namespace App.DataPort.Station.Controllers.Sqlite
{
    public class ExcelController : IApiController
    {

        #region SqliteToExcel        

        /// <summary>
        /// sqlite to excel
        /// </summary>
        /// <param name="fileData"></param>
        /// <returns></returns>
        [SsRoute("sqlite/sqliteToExcel")]
        [SsEqualNotNull(path = "caller.source", value = "GoverGateway", errorMessage = "无权限")]
        public byte[] SqliteToExcel(byte[] fileData)
        {
            DataSet ds;
            #region (x.1) 把sqlite文件内容保存到临时sqlite文件，并读取数据集         
            var tempFile = CommonHelp.GetAbsPathByRealativePath("Data", "Sers", "Temp", CommonHelp.NewGuidLong() + ".db");
            try
            {
                File.WriteAllBytes(tempFile, fileData);
                //(x.x.1)从临时sqlite文件获取数据集
                using (var conn = SqliteHelp.GetOpenConnection(tempFile))
                {
                    ds = conn.GetAllData();
                }
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


            #region (x.2) 把数据集灌入生成的临时数据文件，返回给用户   
            string fileName = RpcContext.RpcData.http_header_Get("fileName");
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = DateTime.Now.ToString("yyyyMMdd_HHmmss_ffff") + ".xlsx";
            }

            tempFile = CommonHelp.GetAbsPathByRealativePath("Data","Sers","Temp", CommonHelp.NewGuidLong()+ ".xlsx");
            try
            {
                //(x.x.1)把数据集灌入生成的临时数据文件
                ExcelHelp.SaveData(tempFile, ds); 

                //(x.x.2)返回给用户
                return StaticFileMap.DownloadFile(tempFile, "application/octet-stream",fileName);
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


        #region ExcelToSqlite
        /// <summary>
        /// excel to sqlite
        /// </summary>
        /// <param name="fileData"></param>
        /// <returns></returns>
        [SsRoute("sqlite/excelToSqlite")]
        [SsEqualNotNull(path = "caller.source", value = "GoverGateway", errorMessage = "无权限")]
        public byte[] ExcelToSqlite(byte[] fileData)
        {
            #region (x.0)从header获取参数

            bool firstRowIsColumnName= RpcContext.RpcData.http_header_Get("firstRowIsColumnName") !="false";

            bool resetTableName = RpcContext.RpcData.http_header_Get("resetTableName") == "true";


            string fileName = RpcContext.RpcData.http_header_Get("fileName");
            #endregion

            DataSet ds;
            #region (x.1) 把内容保存到临时文件，并读取数据集  
            var tempFile = CommonHelp.GetAbsPathByRealativePath("Data", "Sers", "Temp", CommonHelp.NewGuidLong() + ".xlsx");
            try
            {
                File.WriteAllBytes(tempFile,fileData);

                //(x.x.1)从临时文件获取数据集
                ds=ExcelHelp.ReadData(tempFile, firstRowIsColumnName);

                if (resetTableName)
                {
                    for(int index=0;index<ds.Tables.Count;index++)
                    {
                        ds.Tables[index].TableName = "table"+(index+1);
                    }
                }

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


            #region (x.2) 把数据集灌入生成的临时sqlite文件，返回给用户   
            
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = DateTime.Now.ToString("yyyyMMdd_HHmmss_ffff") + ".db";
            }
            tempFile = CommonHelp.GetAbsPathByRealativePath("Data", "Sers", "Temp", CommonHelp.NewGuidLong() + ".db");
            try
            {
                //(x.x.1)把数据集灌入生成的临时sqlite文件
                using (var conn = SqliteHelp.GetOpenConnection(tempFile))
                {
                    foreach (DataTable dt in ds.Tables)
                    {
                        conn.CreateTable(dt);
                        conn.Import(dt);
                    }
                }

                //(x.x.2)返回给用户
                return StaticFileMap.DownloadFile(tempFile, "application/octet-stream", fileName);
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
        }
        #endregion



        #region ExcelToJson
        /// <summary>
        /// excel to json
        /// </summary>
        /// <param name="fileData"></param>
        /// <returns></returns>
        [SsRoute("sqlite/excelToJson")]
        [SsEqualNotNull(path = "caller.source", value = "GoverGateway", errorMessage = "无权限")]
        public ApiReturn<object> ExcelToJson(byte[] fileData)
        {
            #region (x.0)从header获取参数

            bool firstRowIsColumnName = RpcContext.RpcData.http_header_Get("firstRowIsColumnName") != "false";

            bool resetTableName = RpcContext.RpcData.http_header_Get("resetTableName") == "true";

            int maxRowCount = -1;
            int.TryParse(RpcContext.RpcData.http_header_Get("maxRowCount") ?? "-2" , out maxRowCount);
            string fileName = RpcContext.RpcData.http_header_Get("fileName");
            #endregion

            DataSet ds;
            #region (x.1) 把内容保存到临时文件，并读取数据集  
            var tempFile = CommonHelp.GetAbsPathByRealativePath("Data", "Sers", "Temp", CommonHelp.NewGuidLong() + ".xlsx");
            try
            {
                File.WriteAllBytes(tempFile, fileData);

                //(x.x.1)从临时文件获取数据集
                ds = ExcelHelp.ReadData(tempFile, firstRowIsColumnName);

                if (resetTableName)
                {
                    for (int index = 0; index < ds.Tables.Count; index++)
                    {
                        ds.Tables[index].TableName = "table" + (index + 1);
                    }
                }

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


            #region (x.2) 把数据集返回给用户   
            if (maxRowCount > 0)
            {
                foreach (DataTable dt in ds.Tables)
                {
                    while (dt.Rows.Count > maxRowCount)
                    {
                        dt.Rows.RemoveAt(maxRowCount);
                    }
                    dt.AcceptChanges();
                }
            }
            return ds;
            #endregion
        }
        #endregion
    }
}
