using Dapper;
using Ex8.EtlModel.DatabaseJobManifest;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Ex8.SqlDml.Reader.Dbms
{
    public class MsSqlReader : ISqlReader
    {
        public DatabaseTypeEnum DatabaseType => DatabaseTypeEnum.SqlServer;

        public T ExecuteScalar<T>(string connectionString, string sql)
        {
            using (var targetConn = new SqlConnection(connectionString))
            {
                return targetConn.ExecuteScalar<T>(sql);
            }
        }

        public IEnumerable<T> ExecuteQuery<T>(string connectionString, string sql)
        {
            using (var targetConn = new SqlConnection(connectionString))
            {
                return targetConn.Query<T>(sql);
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
    }
}
