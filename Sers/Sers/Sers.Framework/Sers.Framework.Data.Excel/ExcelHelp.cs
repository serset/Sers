using System.Data;
using System.IO;
using OfficeOpenXml;

namespace Sers.Framework.Data.Excel
{
    public class ExcelHelp
    {



        public static void SaveData(string filePath, DataSet ds)
        {

            FileInfo file = new FileInfo(filePath);
            using (ExcelPackage package = new ExcelPackage(file))
            {
                foreach (DataTable dt in ds.Tables)
                {

                    // 添加worksheet
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(dt.TableName);

                    worksheet.Cells.LoadFromDataTable(dt, true);
                    continue;

                    #region (x.x.1) 保存Column                   
                    int colIndex = 0;
                    foreach (DataColumn column in dt.Columns)
                    {
                        colIndex++;

                        worksheet.Cells[1, colIndex].Value = column.ColumnName;
                        worksheet.Cells[1, colIndex].Style.Font.Bold = true;
                    }
                    #endregion

                    #region (x.x.2) 保存 rows
                    int rowIndex = 1;
                    foreach (DataRow row in dt.Rows)
                    {
                        rowIndex++;

                        for (colIndex = 0; colIndex < dt.Columns.Count; colIndex++)
                        {
                            worksheet.Cells[rowIndex, colIndex].Value = row[colIndex];
                        }
                    }
                    #endregion              
                }
                package.Save();
            }
        }


        public static DataSet ReadData(string filePath,bool firstRowIsColumnName=true)
        {
            var ds = new DataSet();
            using (ExcelPackage package = new ExcelPackage(new FileInfo(filePath)))
            {
                var count = package.Workbook.Worksheets.Count;
                for (int k = 1; k <= count; k++)  //worksheet是从1开始的
                {
                    var worksheet = package.Workbook.Worksheets[k];                     

                    var dt = new DataTable(worksheet.Name);
                    ds.Tables.Add(dt);

                    int row = worksheet.Dimension.Rows;
                    int col = worksheet.Dimension.Columns;
                    var cells = worksheet.Cells;

                    int rowIndex = 1;
                    #region (x.x.1) Column

                    if (firstRowIsColumnName)
                    {
                        rowIndex = 2;
                        for (int i = 0; i < col; i++)
                        {
                            var colName = cells[1, i + 1].Value?.ToString();
                            var type = typeof(string);
                            //try
                            //{
                            //    type = cells[1, 1].Value.GetType();
                            //}
                            //catch (System.Exception ex)
                            //{
                            //}
                            dt.Columns.Add(colName, type);
                        }
                    }
                    else
                    {
                        rowIndex = 1;
                        for (int i = 0; i < col; i++)
                        {
                            var colName = "column"+(i+1);
                            var type = typeof(string);
                            dt.Columns.Add(colName, type);
                        }
                    }

                    #endregion

                    #region (x.x.2) row                    
                    for (; rowIndex <= row; rowIndex++)
                    {
                        var rowValue = new object[col];
                        for (int colIndex = 1; colIndex <= col; colIndex++)
                        {
                            //rowValue[colIndex - 1] = "" + cells[rowIndex, colIndex].Value;
                            rowValue[colIndex - 1] =  cells[rowIndex, colIndex].Value;
                        }
                        dt.Rows.Add(rowValue);
                    }
                    #endregion
                }
            }

            return ds;
        }


    }
}
