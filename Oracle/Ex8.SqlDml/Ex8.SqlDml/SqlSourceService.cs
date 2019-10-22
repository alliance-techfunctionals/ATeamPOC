using Ex8.EtlModel.DatabaseJobManifest;
using Ex8.EtlModel.UnitOfWork;
using Ex8.SqlDml.Builder;
using Ex8.SqlDml.Reader;
using System;
using System.Linq;

namespace Ex8.SqlDml
{
    public class SqlSourceService : ISqlSourceService
    {
        private ISqlBuilderSelector _builderSelector;
        private ISqlReaderSelector _readerSelector;

        public SqlSourceService(ISqlBuilderSelector builderSelector, ISqlReaderSelector readerSelector)
        {
            _builderSelector = builderSelector;
            _readerSelector = readerSelector;
        }

        public void SetTableManifestData(DatabaseJobManifest manifestObject)
        {
            var builder = _builderSelector.GetBuilder(manifestObject.manifest.databaseType);
            var reader = _readerSelector.GetReader(manifestObject.manifest.databaseType);
            var sourceConnectionString = manifestObject.manifest.sourceConnectionString;

            foreach (var table in manifestObject.manifest.tables)
            {
                var sourceSql = builder.BuildSourceSql(table);
                table.record_count = reader.ExecuteScalar<long>(sourceConnectionString, sourceSql.SelectRowCountDml);
                var pkInfo = reader.ExecuteQuery<PrimaryKey>(sourceConnectionString, sourceSql.SelectPkDml).SingleOrDefault();
                if (pkInfo != null)
                {
                    table.pk_column_name = pkInfo.ColumnName;
                    table.pk_data_type = pkInfo.DataType;
                }
                
            }
        }
    }
}
