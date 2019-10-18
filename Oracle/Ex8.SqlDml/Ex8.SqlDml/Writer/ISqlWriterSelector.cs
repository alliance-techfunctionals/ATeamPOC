using Ex8.EtlModel.DatabaseJobManifest;
using Ex8.SqlDml.Writer.Dbms;

namespace Ex8.SqlDml.Writer
{
    public interface ISqlWriterSelector
    {
        ISqlWriter GetWriter(DatabaseTypeEnum databaseType);
    }
}