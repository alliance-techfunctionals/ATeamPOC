using Oracle.Copy.Model.DatabaseJobManifest;
using System.Collections.Generic;

namespace Oracle.Copy.Model.UnitOfWork
{
    public class SqlUnitOfWork : ISqlUnitOfWork
    {
        public string SourceDatabase { get; set; }

        public string TargetDatabase { get; set; }

        public string ColumnsMeta { get; set; }

        public string SelectDml { get; set; }

        public string UpdateDml { get; set; }

        public Table Table { get; set; }

        public Dictionary<string, object> Header { get; set; }

        public List<string> SetupTempDml { get; set; } = new List<string>();

        public string UpdateFromTempDml { get; set; }

        public string ClearTempDml { get; set; }

        public string JobUnitName { get; set; }

        public string TableName { get { return $"{Table.schema_name}.{Table.table_name}"; } }
    }
}