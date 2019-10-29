using System.Collections.Generic;
using Ex8.EtlModel.DatabaseJobManifest;
using Ex8.EtlModel.UnitOfWork;

namespace Ex8.SqlDml.Builder.TextSql
{
    public interface ISqlBuilder
    {
        DatabaseTypeEnum DatabaseType { get; }
        SourceSql BuildSourceSql(Table table);
        TargetSql BuildTargetSql(Table table);
        string SelectDmlPageBuilder(string selectDml, string pkColumn, int pageSize, int pageCount);
    }
}