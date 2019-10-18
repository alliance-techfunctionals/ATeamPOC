using Dapper;
using Ex8.EtlModel.DatabaseJobManifest;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
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
    }
}
