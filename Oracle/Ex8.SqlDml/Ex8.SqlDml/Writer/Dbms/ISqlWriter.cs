using Ex8.EtlModel.DatabaseJobManifest;
using System.Collections.Generic;
using System.Data;

namespace Ex8.SqlDml.Writer.Dbms
{
    public interface ISqlWriter
    {
        DatabaseTypeEnum DatabaseType { get; }
        void ExecuteSqlText(string connectionString, List<string> sqlList);
        DataSet GetData(string connectionString, string selectSql);
        void BulkCopy(string connectionString, string destinationTableName, DataTable data);
    }
}