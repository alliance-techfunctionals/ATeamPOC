using Dapper;
using Ex8.EtlModel.DatabaseJobManifest;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Ex8.SqlDml.Writer.Dbms
{
    public class OracleSqlWriter : ISqlWriter
    {
        public DatabaseTypeEnum DatabaseType => DatabaseTypeEnum.Oracle;

        public void UploadTable(string connectionString, List<string> setupSql, Table tableInfo, DataTable uploadData, List<string> postSql)
        {
            using (var connection = new OracleConnection(connectionString))
            {
                connection.Open();

                foreach (var sql in setupSql)
                {
                    connection.Execute(sql);
                }

                BulkCopy(connection, tableInfo.temp_name, tableInfo, uploadData);

                foreach (var sql in postSql)
                {
                    connection.Execute(sql);
                }
            }
        }

        public void ExecuteSqlText(string connectionString, List<string> sqlList)
        {
            using (var targetConn = new OracleConnection(connectionString))
            {
                foreach (var sql in sqlList)
                {
                    targetConn.Execute(sql);
                }
            }
        }

        public int BulkCopy(string connectionString, Table tableInfo, DataTable data)
        {
            using (var connection = new OracleConnection(connectionString))
            {
                connection.Open();
                return BulkCopy(connection, tableInfo.temp_name, tableInfo, data);
            }
        }

        internal int BulkCopy(OracleConnection connection, string destinationTableName, Table tableInfo, DataTable data)
        {
            var pk_colname  = tableInfo.pk_column_name;
            var pk_datatype = tableInfo.pk_data_type;

            List<dynamic> colValueArrr = new List<dynamic>(data.Rows.Count);
            OracleCommand cmd = connection.CreateCommand();
            cmd.CommandText = GetCommandText(destinationTableName, tableInfo);
            cmd.ArrayBindCount = data.Rows.Count;

            // for primary key column Values
            colValueArrr = data.AsEnumerable().Select(r => r.Field<dynamic>(pk_colname)).ToList();
            cmd.Parameters.Add(_OracleParameter(pk_datatype, colValueArrr));

            // Foreach - Looping for All columns except PK col
            foreach (var tableCol in tableInfo.columns) //TODO avoid nested for loop. O(n)2 performance
            {
                colValueArrr = data.AsEnumerable().Select(r => r.Field<dynamic>(tableCol.columnName)).ToList();
                cmd.Parameters.Add(_OracleParameter(tableCol.dataType, colValueArrr));
            }

            return cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="colValueArrr"></param>
        /// <returns>
        ///    oracleParam
        /// </returns>
        internal OracleParameter _OracleParameter(string dataType, List<dynamic> colValueArrr)
        {
            OracleParameter oracleParam = new OracleParameter();
            switch (dataType.ToLower())
            {
                case "number":
                case "Decimal":
                case "Int32":
                    oracleParam.OracleDbType = OracleDbType.Decimal;
                    break;
                case "varchar2":
                case "text":
                    oracleParam.OracleDbType = OracleDbType.NVarchar2;
                    break;
                case "datetime":
                case "date":
                    oracleParam.OracleDbType = OracleDbType.Date;
                    break;
                // To Do: by Neha - I have to add more datatypes..for now I am just testing this for the above 4 only..
                default:
                    break;
            }

            oracleParam.Value = colValueArrr.ToArray();
            return oracleParam;
        }



        /// <summary>
        /// Expected command text = INSERT INTO {destinationTableName} (PERSON_ID, FIRST_NAME, LAST_NAME) VALUES (:1, :2, :3)
        /// </summary>
        /// <param name="destinationTableName"></param>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        internal string GetCommandText(string destinationTableName, Table tableInfo)
        {
            int i = 1;
            var sql = new StringBuilder("INSERT INTO " + destinationTableName + " (" + tableInfo.pk_column_name); // handling Primary Key col.
            var values = new StringBuilder($"VALUES ( :{i}");

            foreach (var col in tableInfo.columns)
            {
                sql.Append($",{col.columnName}");
                i++;
                values.Append($",:{i}");
            }
            sql.Append(") ");
            sql.Append(values.ToString());
            sql.Append(")");
            return sql.ToString();
        }
    }  
}