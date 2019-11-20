using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;
using Dapper;
using Ex8.EtlModel.DatabaseJobManifest;
using Npgsql;
using NpgsqlTypes;
using Ex8.EtlModel;
using PostgreSQLCopyHelper;
using Ex8.Helper.Converter;

namespace Ex8.SqlDml.Writer.Dbms
{
    public class PostgreSqlWriter : ISqlWriter
    {
        public DatabaseTypeEnum DatabaseType => DatabaseTypeEnum.Postgres;

        public void UploadTable(string connectionString, List<string> setupSql, Table tableInfo, DataTable uploadData, List<string> postSql)
        {
            using (var connection = new NpgsqlConnection(connectionString))
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
            using (var targetConn = new NpgsqlConnection(connectionString))
            {
                foreach (var sql in sqlList)
                {
                    targetConn.Execute(sql);
                }
            }
        }

        public int BulkCopy(string connectionString, Table tableInfo, DataTable data)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                return BulkCopy(connection, tableInfo.temp_name, tableInfo, data);
            }
        }

        internal int BulkCopy(NpgsqlConnection connection, string destinationTableName, Table tableInfo, DataTable data)
        {
            using (var writer = connection.BeginBinaryImport($"COPY {destinationTableName} FROM STDIN (FORMAT BINARY)"))
            {
                foreach (DataRow dataRows in data.Rows)
                {
                    writer.StartRow();
                    BulkCopy(writer, destinationTableName, tableInfo, dataRows);
                }
                var count = writer.CompleteAsync();
                return Convert.ToInt32(count.Result);
            }
        }

        internal void BulkCopy(NpgsqlBinaryImporter writer, string destinationTableName, Table tableInfo, DataRow dataRows)
        {
            WriteToStream(writer, tableInfo.pk_data_type, dataRows.Field<dynamic>(tableInfo.pk_column_name));

            foreach (var dataColumn in tableInfo.columns)
            {
                WriteToStream(writer, dataColumn.dataType, dataRows.Field<dynamic>(dataColumn.name));
            }
        }

        internal void WriteToStream(NpgsqlBinaryImporter writer, string dataType, dynamic item)
        {

            switch (dataType.ToLower())
            {
                case "numeric":
                    writer.Write(item, NpgsqlDbType.Numeric);
                    break;
                case "smallint":
                    writer.Write(item, NpgsqlDbType.Smallint);
                    break;
                case "integer":
                case "int":
                    writer.Write(item, NpgsqlDbType.Integer);
                    break;
                case "bigint":
                    writer.Write(item, NpgsqlDbType.Bigint);
                    break;
                case "float":
                case "real":
                    writer.Write(item, NpgsqlDbType.Real);
                    break;
                case "varchar":
                    writer.Write(item, NpgsqlDbType.Varchar);
                    break;
                case "text":
                    writer.Write(item, NpgsqlDbType.Text);
                    break;
                case "datetime":
                case "date":
                    writer.Write(item, NpgsqlDbType.Date);
                    break;
                case "char":
                    writer.Write(item, NpgsqlDbType.Char);
                    break;
                case "boolean":
                    writer.Write(item, NpgsqlDbType.Boolean);
                    break;
                case "double":
                    writer.Write(item, NpgsqlDbType.Double);
                    break;
                case "money":
                    writer.Write(item, NpgsqlDbType.Money);
                    break;
                case "time":
                    writer.Write(item, NpgsqlDbType.Time);
                    break;
                default:
                    throw new ArgumentException($"Unsupported Data Type: {dataType}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="colValueArrr"></param>
        /// <returns>
        ///    pgParam
        /// </returns>
        internal NpgsqlParameter _PostgreParameter(string dataType, List<dynamic> colValueArrr)
        {
            var param = new NpgsqlParameter();
            switch (dataType.ToLower())
            {
                case "float":
                case "real":
                    param.NpgsqlDbType = NpgsqlDbType.Real;
                    break;
                case "numeric":
                    param.NpgsqlDbType = NpgsqlDbType.Numeric;
                    break;
                case "smallint":
                    param.NpgsqlDbType = NpgsqlDbType.Smallint;
                    break;
                case "int":
                    param.NpgsqlDbType = NpgsqlDbType.Integer;
                    break;
                case "bigint":
                    param.NpgsqlDbType = NpgsqlDbType.Bigint;
                    break;
                case "char":
                    param.NpgsqlDbType = NpgsqlDbType.Char;
                    break;
                case "varchar":
                    param.NpgsqlDbType = NpgsqlDbType.Varchar;
                    break;
                case "text":
                    param.NpgsqlDbType = NpgsqlDbType.Text;
                    break;
                case "datetime":
                case "date":
                    param.NpgsqlDbType = NpgsqlDbType.Date;
                    break;
                default:
                    throw new ArgumentException($"Unsupported Data Type: {dataType}");
            }

            param.Value = colValueArrr.ToArray();
            return param;
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
                sql.Append($",{col.name}");
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
