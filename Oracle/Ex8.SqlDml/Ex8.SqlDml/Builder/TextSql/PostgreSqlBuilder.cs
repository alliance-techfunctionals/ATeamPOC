using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Dapper;
using Ex8.EtlModel.DatabaseJobManifest;
using Ex8.EtlModel.UnitOfWork;
using Ex8.Helper.Serialization;

namespace Ex8.SqlDml.Builder.TextSql
{
    public class PostgreSqlBuilder : ISqlBuilder
    {
        public DatabaseTypeEnum DatabaseType => DatabaseTypeEnum.Postgres;

        public SourceSql BuildSourceSql(Table table)
        {
            var selectPkDml =
                 "SELECT a.attname, format_type(a.atttypid, a.atttypmod) AS data_type " +
                 "FROM pg_index i " +
                 "JOIN pg_attribute a ON a.attrelid = i.indrelid " +
                 "AND a.attnum = ANY(i.indkey) " +
                $"WHERE i.indrelid = '{table.schema_name}.{table.table_name}'::regclass " +
                 "AND i.indisprimary;";

            var selectRowCountDml = $"select count(*) from {table.schema_name}.{table.table_name}";

            return new SourceSql { SelectPkDml = selectPkDml, SelectRowCountDml = selectRowCountDml };
        }

        public TargetSql BuildTargetSql(Table table)
        {
            table.temp_name = $"ex8_temp_{table.table_name}";

            var columnNameCsv = string.Join(", ", table.columns.Select(c => c.name));

            var selectSql = $"select {table.pk_column_name}, {columnNameCsv} " +
                             $" from ( select {table.pk_column_name}, {columnNameCsv}, row_number() over(order by {table.pk_column_name}) as seqnum " +
                                      $" from {table.schema_name}.{table.table_name} ) q";


            var updateFromTempDml = $"UPDATE {table.schema_name}.{table.table_name} SET ";

            foreach (var col in table.columns)
            {
                updateFromTempDml += $"{col.name} = tmpTable.{col.name}";
                if (table.columns[table.columns.Length - 1].name != col.name)
                {
                    updateFromTempDml += ", ";
                }
            }

            updateFromTempDml += $" FROM {table.temp_name} tmpTable WHERE {table.schema_name}.{table.table_name}.{table.pk_column_name} = tmpTable.{table.pk_column_name};";

            var clearTempDml = $"TRUNCATE TABLE {table.temp_name}";

            var tempTableCreate = $"create temp TABLE {table.temp_name} as " +
                $"select {table.pk_column_name}, {columnNameCsv} from {table.schema_name}.{table.table_name} limit 0;";


            return new TargetSql
            {
                SelectDml = selectSql,
                SetupTempDml = new List<string> { tempTableCreate },
                UpdateFromTempDml = updateFromTempDml,
                ClearTempDml = clearTempDml
            };
        }

        public string SelectDmlPageBuilder(string selectDml, string pkColumn, int pageSize, int pageCount)
        {
            var selectDmlBuilder = new StringBuilder();
            selectDmlBuilder.AppendLine(selectDml);
            selectDmlBuilder.AppendLine($"order by {pkColumn}");
            selectDmlBuilder.AppendLine($"LIMIT {pageSize}");
            selectDmlBuilder.AppendLine($"OFFSET {pageSize} * ({pageCount}- 1);");

            return selectDmlBuilder.ToString();
        }
    }
}
