using Ex8.EtlModel.DatabaseJobManifest;
using Ex8.SqlDml.Builder.TextSql;

namespace Ex8.SqlDml.Builder
{
    public interface ISqlBuilderSelector
    {
        ISqlBuilder GetBuilder(DatabaseTypeEnum databaseType);
    }
}