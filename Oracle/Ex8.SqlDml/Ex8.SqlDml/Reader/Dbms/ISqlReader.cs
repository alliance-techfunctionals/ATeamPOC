using Ex8.EtlModel.DatabaseJobManifest;
using System.Collections.Generic;

namespace Ex8.SqlDml.Reader.Dbms
{
    public interface ISqlReader
    {
        DatabaseTypeEnum DatabaseType { get; }
        T ExecuteScalar<T>(string connectionString, string sql);
        IEnumerable<T> ExecuteQuery<T>(string connectionString, string sql);
    }
}