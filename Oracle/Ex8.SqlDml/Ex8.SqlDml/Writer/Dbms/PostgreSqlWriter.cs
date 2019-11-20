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
                foreach (DataRow dataRow in data.Rows)
                {
                    writer.StartRow();
                    BulkCopy(writer, destinationTableName, tableInfo, dataRow);
                }
                var count = writer.CompleteAsync();
                return Convert.ToInt32(count.Result);
            }
        }

        internal void BulkCopy(NpgsqlBinaryImporter writer, string destinationTableName, Table tableInfo, DataRow dataRow)
        {
            WriteToStream(writer, tableInfo.pk_data_type, dataRow.Field<dynamic>(tableInfo.pk_column_name));
            foreach (var dataColumn in tableInfo.columns)
            {
                WriteToStream(writer, dataColumn.dataType, dataRow.Field<dynamic>(dataColumn.name));
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
                    throw new ArgumentException($"Unsupported Data Type in PostGres DB: {dataType}");
            }
        }
    }
}
