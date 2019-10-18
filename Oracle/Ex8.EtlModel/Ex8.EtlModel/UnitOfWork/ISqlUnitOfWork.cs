using System.Collections.Generic;
using Ex8.EtlModel.DatabaseJobManifest;

namespace Ex8.EtlModel.UnitOfWork
{
    public interface ISqlUnitOfWork
    {
        DatabaseTypeEnum DatabaseType { get; set; }

        string SourceDatabase { get; set; }

        string TargetDatabase { get; set; }

        string ColumnsMeta { get; set; }

        string SelectDml { get; set; }

        List<string> SetupTempDml { get; set; }

        string UpdateDml { get; set; }

        string UpdateFromTempDml { get; set; }

        string ClearTempDml { get; set; }

        string JobUnitName { get; set; }        

        JobTypeEnum JobType { get; set; }

        Table Table { get; set; }

        string TableName { get; }

        Dictionary<string, object> Header { get; set; }
    }
}