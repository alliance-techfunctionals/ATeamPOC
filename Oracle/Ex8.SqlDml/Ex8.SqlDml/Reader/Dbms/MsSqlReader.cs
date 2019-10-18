using Dapper;
using Ex8.EtlModel.DatabaseJobManifest;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Ex8.SqlDml.Reader.Dbms
{
    public class MsSqlReader : ISqlReader
    {
        public DatabaseTypeEnum DatabaseType => DatabaseTypeEnum.MsSql;

        public T ExecuteScalar<T>(string connectionString, string sql)
        {
            using (var targetConn = new SqlConnection(connectionString))
            {
                return targetConn.ExecuteScalar<T>(sql);
            }
        }
    }
}
