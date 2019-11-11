using Dapper;
using Ex8.EtlModel.DatabaseJobManifest;
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Data;
using System.Linq;

namespace Ex8.SqlDml.Reader.Dbms
{
    public class MySqlReader : ISqlReader
    {
        public DatabaseTypeEnum DatabaseType => DatabaseTypeEnum.MySql;

        public T ExecuteScalar<T>(string connectionString, string sql)
        {
            using (var targetConn = new MySqlConnection(connectionString))
            {
                return targetConn.ExecuteScalar<T>(sql);
            }
        }

        public IEnumerable<T> ExecuteQuery<T>(string connectionString, string sql)
        {
            using (var targetConn = new MySqlConnection(connectionString))
            {
                return targetConn.Query<T>(sql);
            }
        }

        public DataSet GetData(string connectionString, string selectSql)
        {
            var ds = new DataSet();

            using (var adapter = new MySqlDataAdapter(selectSql, connectionString))
            {
                adapter.Fill(ds);
                return ds;
            }
        }
    }
}
