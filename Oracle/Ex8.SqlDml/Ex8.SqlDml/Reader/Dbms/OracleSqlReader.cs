using Dapper;
using Ex8.EtlModel.DatabaseJobManifest;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Ex8.SqlDml.Reader.Dbms
{
    public class OracleSqlReader : ISqlReader
    {
        public DatabaseTypeEnum DatabaseType => DatabaseTypeEnum.Oracle;

        public T ExecuteScalar<T>(string connectionString, string sql)
        {
            using (var targetConn = new OracleConnection(connectionString))
            {
                return targetConn.ExecuteScalar<T>(sql);
            }
        }

        public IEnumerable<T> ExecuteQuery<T>(string connectionString, string sql)
        {
            using (var targetConn = new OracleConnection(connectionString))
            {
                return targetConn.Query<T>(sql);
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
    }
}
