using Oracle.Copy.Model.DatabaseJobManifest;
using System.Collections.Generic;

namespace Oracle.Copy.Model.UnitOfWork
{
    public interface ISqlUnitOfWork
    {
        string SourceDatabase { get; set; }

        string TargetDatabase { get; set; }

        string ColumnsMeta { get; set; }

        string SelectDml { get; set; }

        List<string> SetupTempDml { get; set; }

        string UpdateDml { get; set; }

        string UpdateFromTempDml { get; set; }

        string ClearTempDml { get; set; }

        string JobUnitName { get; set; }

        Table Table { get; set; }

        string TableName { get; }

        Dictionary<string, object> Header { get; set; }
    }
}