using Oracle.Copy.Model;
using System;
using System.Collections.Generic;
using Dapper;
using System.Text;
using Oracle.Copy.Model.DatabaseJobManifest;
using Oracle.Copy.Model.UnitOfWork;

namespace Oracle.Copy
{
    public class OracleSqlBuilder : ISqlBuilder
    {
        public DatabaseTypeEnum DatabaseType => DatabaseTypeEnum.Oracle;

        public SourceSql BuildSourceSql(Table table)
        {
            var selectPkDml =
                 "SELECT coalesce(cc.column_name,'') as PkCol " +
                 "FROM ALL_CONS_COLUMNS cc JOIN ALL_CONSTRAINTS c ON ( cc.owner = c.owner AND cc.table_name = c.table_name AND c.constraint_name = cc.constraint_name ) " +
                $"WHERE c.CONSTRAINT_TYPE = 'P' AND cc.table_name = '{table.table_name.ToUpper()}' AND cc.Owner = '{table.schema_name.ToUpper()}'";

            var selectRowCountDml = $"select count(*) from {table.schema_name}.{table.table_name}";

            return new SourceSql { SelectPkDml = selectPkDml, SelectRowCountDml = selectRowCountDml };
        }

        public TargetSql BuildTargetSql(Table table)
        {
            table.TempName = $"ex8_temp_{table.table_name}";

            var builder = new SqlBuilder();
            var template = builder.AddTemplate($"SELECT {table.PkColumnName}, /**select**/ from {table.schema_name}.{table.table_name} /**where**/ ");

            foreach (var col in table.columns)
            {
                builder.Select(col.columnName);
            }

            var updateFromTempDml = $"UPDATE {table.schema_name}.{table.table_name} {Environment.NewLine} SET ";

            foreach (var col in table.columns)
            {
                updateFromTempDml += $"{table.schema_name}.{table.table_name}.{col.columnName} = tempupdate.{col.columnName}";
                if (table.columns[table.columns.Length - 1].columnName != col.columnName)
                {
                    updateFromTempDml += ",";
                }
            }

            updateFromTempDml += $" from {table.schema_name}.{table.table_name} ";
            updateFromTempDml += $" inner join {table.TempName} tempupdate on (tempupdate.{table.PkColumnName} = {table.schema_name}.{table.table_name}.{table.PkColumnName} ) ";

            var clearTempDml = $"TRUNCATE TABLE {table.TempName}";

            var tempTableDrop = "declare v_sql LONG; " +
                                "begin " +
                                $"v_sql:='truncate table {table.TempName}'; " +
                                "execute immediate v_sql; " +
                                $"v_sql:='drop table {table.TempName}'; " +
                                "execute immediate v_sql; " +
                                "EXCEPTION WHEN OTHERS THEN " +
                                "IF SQLCODE = -942 THEN NULL; " + //suppresses ORA-00942 exception
                                "ELSE RAISE; " +
                                "END IF; " +
                                "END; ";

            var builderTempTable = new SqlBuilder();
            var tempTableCreate = builderTempTable.AddTemplate(
                $"create global temporary table {table.TempName} on commit preserve rows as " +
                $"select {table.PkColumnName}, /**select**/ from {table.schema_name}.{table.table_name} where rownum <= 0; ");

            foreach (var col in table.columns)
            {
                builderTempTable.Select(col.columnName);
            }

            return new TargetSql
            {
                SelectDml = template.RawSql,
                SetupTempDml = new List<string> { tempTableDrop, tempTableCreate.RawSql },
                UpdateFromTempDml = updateFromTempDml,
                ClearTempDml = clearTempDml
            };
        }

        public string SelectDmlPageBuilder(string selectDml, string pkColumn, int pageSize, int pageCount)
        {
            var selectDmlBuilder = new StringBuilder();
            selectDmlBuilder.AppendLine(selectDml);
            selectDmlBuilder.AppendLine($"where rownum between {((pageCount - 1) * pageSize) + 1 } and {pageCount * pageSize}");
            selectDmlBuilder.AppendLine($"order by {pkColumn}");

            return selectDmlBuilder.ToString();
        }
    }
}
