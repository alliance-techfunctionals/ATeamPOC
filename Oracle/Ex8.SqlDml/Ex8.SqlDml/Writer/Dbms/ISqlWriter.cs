using Ex8.EtlModel.DatabaseJobManifest;
using System.Collections.Generic;
using System.Data;

namespace Ex8.SqlDml.Writer.Dbms
{
    public interface ISqlWriter
    {
        DatabaseTypeEnum DatabaseType { get; }
        void UploadTable(string connectionString, List<string> setupSql, Table tableInfo, DataTable uploadData, List<string> postSql);
        void ExecuteSqlText(string connectionString, List<string> sqlList);
    }
}