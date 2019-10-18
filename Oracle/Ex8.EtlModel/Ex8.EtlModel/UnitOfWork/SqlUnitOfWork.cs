using System.Collections.Generic;
using Ex8.EtlModel.DatabaseJobManifest;

namespace Ex8.EtlModel.UnitOfWork
{
    public class SqlUnitOfWork  : ISqlUnitOfWork
    {
        public DatabaseTypeEnum DatabaseType { get; set; }

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

        public string TableName { get { return $"{this.Table.schema_name}.{this.Table.table_name}"; } }

        public JobTypeEnum JobType { get; set; }
    }
}