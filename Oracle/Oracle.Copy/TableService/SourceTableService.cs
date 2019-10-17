using Dapper;
using Oracle.Copy.Model.DatabaseJobManifest;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oracle.Copy.TableService
{
    public class SourceTableService : ISourceTableService
    {
        private ISqlBuilderSelector _builderSelector;

        public SourceTableService(ISqlBuilderSelector builderSelector)
        {
            _builderSelector = builderSelector;
        }

        public void SetTableManifestData(DatabaseJobManifest manifestObject)
        {
            var builder = _builderSelector.GetBuilder(manifestObject.manifest.databaseType);

            using (var sourceConn = new OracleConnection(manifestObject.manifest.sourceConnectionString))
            {
                foreach (var table in manifestObject.manifest.tables)
                {
                    var sourceSql = builder.BuildSourceSql(table);
                    table.RecordCount = sourceConn.ExecuteScalar<long>(sourceSql.SelectRowCountDml);
                    table.PkColumnName = sourceConn.ExecuteScalar<string>(sourceSql.SelectPkDml);
                }
            }
        }
    }
}
