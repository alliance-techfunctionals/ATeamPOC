using Ex8.EtlModel.DatabaseJobManifest;
using System.Collections.Generic;
using System.Data;

namespace Ex8.SqlDml.Reader.Dbms
{
    public interface ISqlReader
    {
        DatabaseTypeEnum DatabaseType { get; }
        T ExecuteScalar<T>(string connectionString, string sql);
        DataSet GetData(string connectionString, string selectSql);
        IEnumerable<T> ExecuteQuery<T>(string connectionString, string sql);
    }
}