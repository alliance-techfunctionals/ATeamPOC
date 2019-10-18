using Dapper;
using Ex8.EtlModel.DatabaseJobManifest;
using Oracle.DataAccess.Client;
using OracleConnection = Oracle.ManagedDataAccess.Client.OracleConnection;
using OracleDataAdapter = Oracle.ManagedDataAccess.Client.OracleDataAdapter;
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

        public DataSet GetData(string connectionString, string selectSql)
        {
            var ds = new DataSet();

            using (var connection = new OracleConnection(connectionString))
            using (var adapter = new OracleDataAdapter(selectSql, connectionString))
            {
                adapter.Fill(ds);
                return ds;
            }
        }

        public void BulkCopy(string connectionString, string destinationTableName, DataTable data)
        {
            using (var connection = new Oracle.DataAccess.Client.OracleConnection(connectionString))
            {
                connection.Open();
                using (var bulkCopy = new OracleBulkCopy(connection, OracleBulkCopyOptions.Default))
                {
                    bulkCopy.BulkCopyTimeout = 0;
                    bulkCopy.DestinationTableName = destinationTableName;
                    bulkCopy.WriteToServer(data);
                    bulkCopy.Close();
                }
            }
        }
    }
}
