using Ex8.EtlModel.DatabaseJobManifest;
using Ex8.SqlDml.Reader.Dbms;

namespace Ex8.SqlDml.Reader
{
    public interface ISqlReaderSelector
    {
        ISqlReader GetReader(DatabaseTypeEnum databaseType);
    }
}