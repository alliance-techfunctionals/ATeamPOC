using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ex8.EtlModel.DatabaseJobManifest;
using Ex8.EtlModel.UnitOfWork;

namespace Ex8.SqlDml.Builder.TextSql
{
    public class MySqlBuilder : ISqlBuilder
    {
        public DatabaseTypeEnum DatabaseType => DatabaseTypeEnum.MySql;

        public SourceSql BuildSourceSql(Table table)
        {
            var selectPkDml = "select cu.column_name as columnname, c.data_type as datatype " +
                                 "from information_schema.key_column_usage cu " +
                                 "join information_schema.columns c " +
                                 "on cu.table_schema = c.table_schema " +
                                 "and cu.table_name = c.table_name " +
                                 "and cu.column_name = c.column_name " +
                                $"where cu.table_schema = '{table.schema_name}' and constraint_name = 'primary' and " +
                                 $"cu.TABLE_NAME = '{table.table_name}';";

            var selectRowCountDml = $"select count(*) from {table.schema_name}.{table.table_name}";

            return new SourceSql { SelectPkDml = selectPkDml, SelectRowCountDml = selectRowCountDml };
        }

        public TargetSql BuildTargetSql(Table table)
        {
            table.temp_name = $"ex8_temp_{table.table_name}";

            var columnNameCsv = string.Join(", ", table.columns.Select(c => c.name));
            var selectSql = $"SELECT {table.pk_column_name}, {columnNameCsv} " +
                             $"from {table.schema_name}.{table.table_name};";


            var updateFromTempDml = $"UPDATE {table.schema_name}.{table.table_name} actualTable INNER JOIN {table.temp_name} tmpTable "
                                     + $"on actualTable.{table.pk_column_name} = tmpTable.{table.pk_column_name} SET ";

            foreach (var col in table.columns)
            {
                updateFromTempDml += $"actualTable.{col.name} = tmpTable.{col.name}";
                if (table.columns[table.columns.Length - 1].name != col.name)
                {
                    updateFromTempDml += ", ";
                }
            }

            var clearTempDml = $"DROP TEMPORARY TABLE {table.temp_name};";

            var templateTempTable = $"CREATE TEMPORARY TABLE {table.temp_name} SELECT {table.pk_column_name}, {columnNameCsv} FROM {table.schema_name}.{table.table_name} LIMIT 0;";

            return new TargetSql
            {
                SelectDml = selectSql,
                SetupTempDml = new List<string> { templateTempTable },
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