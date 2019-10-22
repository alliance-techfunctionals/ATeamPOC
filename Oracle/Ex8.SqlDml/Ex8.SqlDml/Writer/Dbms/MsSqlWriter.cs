using Dapper;
using Ex8.EtlModel.DatabaseJobManifest;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Ex8.SqlDml.Writer.Dbms
{
    public class MsSqlWriter : ISqlWriter
    {
        public DatabaseTypeEnum DatabaseType => DatabaseTypeEnum.MsSql;

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

        public DataSet GetData(string connectionString, string selectSql)
        {
            var ds = new DataSet();

            using (var connection = new SqlConnection(connectionString))
            using (var adapter = new SqlDataAdapter(selectSql, connectionString))
            {
                adapter.Fill(ds);
                return ds;
            }
        }

        public int BulkCopy(string connectionString, string destinationTableName, DataTable data)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.KeepIdentity, null))
                {
                    bulkCopy.BulkCopyTimeout = 0;
                    bulkCopy.DestinationTableName = destinationTableName;
                    bulkCopy.WriteToServer(data);
                    bulkCopy.Close();
                }
            }
            return 0;
        }
    }
}
