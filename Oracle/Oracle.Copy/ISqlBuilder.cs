using Oracle.Copy.Model.DatabaseJobManifest;
using Oracle.Copy.Model.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Oracle.Copy
{
    public interface ISqlBuilder
    {
        DatabaseTypeEnum DatabaseType { get; }
        SourceSql BuildSourceSql(Table table);
        TargetSql BuildTargetSql(Table table);
        string SelectDmlPageBuilder(string selectDml, string pkColumn, int pageSize, int pageCount);
    }
}
