using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Ex8.EtlModel.DatabaseJobManifest;
using Npgsql;
using System.Linq;

namespace Ex8.SqlDml.Reader.Dbms
{
    public class PostgreSqlReader : ISqlReader
    {
        public DatabaseTypeEnum DatabaseType => DatabaseTypeEnum.Postgres;

        public T ExecuteScalar<T>(string connectionString, string sql)
        {
            using (var targetConn = new NpgsqlConnection(connectionString))
            {
                return targetConn.ExecuteScalar<T>(sql);
            }
        }

        public IEnumerable<T> ExecuteQuery<T>(string connectionString, string sql)
        {
            using (var targetConn = new NpgsqlConnection(connectionString))
            {
                return targetConn.Query<T>(sql);
            }
        }

        public DataSet GetData(string connectionString, string selectSql)
        {
            var ds = new DataSet();
            using (var connection = new NpgsqlConnection(connectionString))
            using (var adapter = new NpgsqlDataAdapter(selectSql, connectionString))
            {
                adapter.Fill(ds);
                return ds;
            }
        }
    }
}
