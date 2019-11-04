using Dapper;
using Ex8.EtlModel.DatabaseJobManifest;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("Ex8.SqlDml.Integration.Test")]
namespace Ex8.SqlDml.Writer.Dbms
{
    public class MsSqlWriter : ISqlWriter
    {
        public DatabaseTypeEnum DatabaseType => DatabaseTypeEnum.SqlServer;

        public void UploadTable(string connectionString, List<string> setupSql, Table tableInfo, DataTable uploadData, List<string> postSql)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (var sql in setupSql)
                {
                    connection.Execute(sql);
                }

                BulkCopy(connection, tableInfo.temp_name, uploadData);

                foreach (var sql in postSql)
                {
                    connection.Execute(sql);
                }
            }
        }

        public void ExecuteSqlText(string connectionString, List<string> sqlList)
        {
            using (var targetConn = new SqlConnection(connectionString))
            {
                foreach (var sql in sqlList)
                {
                    targetConn.Execute(sql);
                }
            }
        }

        internal void BulkCopy(SqlConnection connection, string destinationTableName, DataTable data)
        {
            using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.KeepIdentity, null))
            {
                bulkCopy.BulkCopyTimeout = 0;
                bulkCopy.DestinationTableName = destinationTableName;
                bulkCopy.WriteToServer(data);
                bulkCopy.Close();
            }
        }

        public int BulkCopy(string connectionString, Table tableInfo, DataTable sourceTable)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                BulkCopy(connection, tableInfo.temp_name, sourceTable);
                var recordsCount = connection.ExecuteScalar<int>($"select count(*) from {tableInfo.temp_name}");
                return recordsCount;
            }
        }
    }
}
