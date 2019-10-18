using Ex8.EtlModel.DatabaseJobManifest;

namespace Ex8.SqlDml.Reader.Dbms
{
    public interface ISqlReader
    {
        DatabaseTypeEnum DatabaseType { get; }
        T ExecuteScalar<T>(string connectionString, string sql);
    }
}