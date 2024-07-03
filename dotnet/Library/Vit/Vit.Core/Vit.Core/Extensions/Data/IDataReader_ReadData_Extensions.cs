using System;
using System.Data;
using System.Runtime.CompilerServices;

namespace Vit.Extensions
{
    public static class IDataReader_ReadData_Extensions
    {


        #region ReadDataTable    
        /// <summary>
        /// 加载数据到DataTable。若不存在数据，则返回空DataTable
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="maxRowCount"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DataTable ReadDataTable(this IDataReader reader, int maxRowCount = int.MaxValue)
        {
            //var dt = new DataTable();
            //dt.Load(reader);
            //return dt;             


            if (reader.IsClosed || maxRowCount <= 0) return null;


            DataTable dt = new DataTable();
            int fieldCount = reader.FieldCount;
            for (int fieldIndex = 0; fieldIndex < fieldCount; ++fieldIndex)
            {
                if (reader.IsDateTime(fieldIndex))
                {
                    dt.Columns.Add(reader.GetName(fieldIndex), typeof(DateTime));
                    continue;
                }

                dt.Columns.Add(reader.GetName(fieldIndex), reader.GetFieldType(fieldIndex));
            }

            if (!reader.Read()) return dt;

            dt.BeginLoadData();
            object[] objValues = new object[fieldCount];
            int curRowCount = 0;

            while (true)
            {
                reader.GetValues(objValues);
                dt.LoadDataRow(objValues, true);

                curRowCount++;

                if (curRowCount >= maxRowCount)
                {
                    break;
                }

                if (!reader.Read()) break;
            }

            dt.EndLoadData();
            return dt;
        }
        #endregion





        #region ReadDataSet

        /// <summary>
        /// (Lith Framework)
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DataSet ReadDataSet(this IDataReader reader)
        {
            var ds = new DataSet();
            while (!reader.IsClosed)
            {
                var dt = reader.ReadDataTable();
                if (dt == null) break;
                ds.Tables.Add(dt);

                if (!reader.NextResult()) break;
            }
            return ds;
        }
        #endregion




    }
}
