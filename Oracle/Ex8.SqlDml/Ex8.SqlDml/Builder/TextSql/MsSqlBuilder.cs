using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using Ex8.EtlModel.DatabaseJobManifest;
using Ex8.EtlModel.UnitOfWork;
using Ex8.Helper.Serialization;

namespace Ex8.SqlDml.Builder.TextSql
{
    public class MsSqlBuilder : ISqlBuilder
    {
        public DatabaseTypeEnum DatabaseType => DatabaseTypeEnum.SqlServer;

        public SourceSql BuildSourceSql(Table table)
        {
            var selectPkDml = "SELECT isnull(cu.COLUMN_NAME,'') as ColumnName, c.data_type as DataType " +
                                 "FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE cu " +
                                 "JOIN INFORMATION_SCHEMA.COLUMNS c " +
                                 "  ON cu.table_schema = c.table_schema " +
                                 "  AND cu.TABLE_NAME = c.table_name " +
                                 "  AND cu.COLUMN_NAME = c.COLUMN_NAME " +
                                "WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_SCHEMA + '.' + QUOTENAME(CONSTRAINT_NAME)), 'IsPrimaryKey') = 1 " +
                                 $"AND cu.TABLE_NAME = '{table.table_name}' AND cu.TABLE_SCHEMA = '{table.schema_name}'";

            var selectRowCountDml = $"select count(*) from {table.schema_name}.{table.table_name} WITH (NOLOCK) ";

            return new SourceSql { SelectPkDml = selectPkDml, SelectRowCountDml = selectRowCountDml };
        }

        public TargetSql BuildTargetSql(Table table)
        {
            table.temp_name = $"#{table.table_name}";

            var builder = new SqlBuilder();
            var template = builder.AddTemplate($"SELECT {table.pk_column_name}, /**select**/ from {table.schema_name}.{table.table_name} WITH (NOLOCK) /**where**/ ");

            foreach (var col in table.columns)
            {
                builder.Select(col.columnName);
            }

            var updateFromTempDml = $"UPDATE {table.schema_name}.{table.table_name} SET ";

            foreach (var col in table.columns)
            {
                updateFromTempDml += $"{table.schema_name}.{table.table_name}.{col.columnName} = tempupdate.{col.columnName}";
                if (table.columns[table.columns.Length - 1].columnName != col.columnName)
                {
                    updateFromTempDml += ",";
                }
            }

            updateFromTempDml += $" from {table.schema_name}.{table.table_name} ";
            updateFromTempDml += $" inner join {table.temp_name} tempupdate on (tempupdate.{table.pk_column_name} = {table.schema_name}.{table.table_name}.{table.pk_column_name} ) ";

            var clearTempDml = $"DROP TABLE {table.temp_name}";

            var builderTempTable = new SqlBuilder();
            var templateTempTable = builderTempTable.AddTemplate(
                $"SELECT top 0 {table.pk_column_name}, /**select**/ into  {table.temp_name} from {table.schema_name}.{table.table_name} WITH (NOLOCK); ");

            foreach (var col in table.columns)
            {
                builderTempTable.Select(col.columnName);
            }

            return new TargetSql
            {
                SelectDml = template.RawSql,
                SetupTempDml = new List<string> { templateTempTable.RawSql },
                UpdateFromTempDml = updateFromTempDml,
                ClearTempDml = clearTempDml
            };
        }

        public string SelectDmlPageBuilder(string selectDml, string pkColumn, int pageSize, int pageCount)
        {
            var selectDmlBuilder = new StringBuilder();
            selectDmlBuilder.AppendLine(selectDml);
            selectDmlBuilder.AppendLine($"order by {pkColumn}");
            selectDmlBuilder.AppendLine($"OFFSET {pageSize} * ({pageCount}- 1) ROWS");
            selectDmlBuilder.AppendLine($"FETCH NEXT {pageSize} ROWS ONLY OPTION (RECOMPILE);");

            return selectDmlBuilder.ToString();
        }
    }
}