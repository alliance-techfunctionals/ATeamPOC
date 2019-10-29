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
    public class OracleSqlBuilder : ISqlBuilder
    {
        public DatabaseTypeEnum DatabaseType => DatabaseTypeEnum.Oracle;

        public SourceSql BuildSourceSql(Table table)
        {
            var selectPkDml =
                 "SELECT coalesce(cc.column_name,'') as ColumnName, uc.data_type as DataType " +
                 "FROM ALL_CONS_COLUMNS cc JOIN ALL_CONSTRAINTS c ON ( cc.owner = c.owner AND cc.table_name = c.table_name AND c.constraint_name = cc.constraint_name ) " +
                 "JOIN USER_TAB_COLUMNS uc ON cc.table_name = uc.table_name AND cc.column_name = uc.column_name " +
                $"WHERE c.CONSTRAINT_TYPE = 'P' AND cc.table_name = '{table.table_name.ToUpper()}' AND cc.Owner = '{table.schema_name.ToUpper()}'";

            var selectRowCountDml = $"select count(*) from {table.schema_name}.{table.table_name}";

            return new SourceSql { SelectPkDml = selectPkDml, SelectRowCountDml = selectRowCountDml };
        }

        public TargetSql BuildTargetSql(Table table)
        {
            table.temp_name = $"ex8_temp_{table.table_name}";

            var builder = new SqlBuilder();
            var selectTemplate = builder.AddTemplate($"SELECT {table.pk_column_name}, /**select**/ from {table.schema_name}.{table.table_name} /**where**/ ");

            foreach (var col in table.columns)
            {
                builder.Select(col.columnName);
            }

            string tempTable="",lastPart="";
            var updateFromTempDml = "UPDATE ( SELECT ";
             foreach (var col in table.columns)
            {
                updateFromTempDml += $"ActualTable.{col.columnName} As ActualTable_{col.columnName},";
                tempTable += $"TempTable.{col.columnName} As TempTable_{col.columnName}";
                lastPart += $"t.ActualTable_{col.columnName} = t.TempTable_{col.columnName}";               
                if (table.columns[table.columns.Length - 1].columnName != col.columnName)
                {                    
                    tempTable += ",";
                    lastPart += ",";
                }
            }
            updateFromTempDml += tempTable;
            updateFromTempDml += $" FROM {table.schema_name}.{table.table_name} ActualTable INNER JOIN {table.temp_name} TempTable on TempTable.{table.pk_column_name} = ActualTable.{table.pk_column_name} ) t ";
            updateFromTempDml += "SET "+lastPart;

            var clearTempDml = $"TRUNCATE TABLE {table.temp_name}";

            var tempTableDrop = "declare v_sql LONG; " +
                                "begin " +
                                $"v_sql:='truncate table {table.temp_name}'; " +
                                "execute immediate v_sql; " +
                                $"v_sql:='drop table {table.temp_name}'; " +
                                "execute immediate v_sql; " +
                                "EXCEPTION WHEN OTHERS THEN " +
                                "IF SQLCODE = -942 THEN NULL; " + //suppresses ORA-00942 exception
                                "ELSE RAISE; " +
                                "END IF; " +
                                "END; ";

            var builderTempTable = new SqlBuilder();
            var tempTableCreate = builderTempTable.AddTemplate(
                $"create global temporary table {table.temp_name} on commit preserve rows as " +
                $"select {table.pk_column_name}, /**select**/ from {table.schema_name}.{table.table_name} where rownum <= 0 ");

            var addPrimaryKey = $"alter table {table.temp_name} add constraint {table.temp_name}_pk primary key({table.pk_column_name})";

            foreach (var col in table.columns)
            {
                builderTempTable.Select(col.columnName);
            }

            return new TargetSql
            {
                SelectDml = selectTemplate.RawSql,
                SetupTempDml = new List<string> { tempTableDrop, tempTableCreate.RawSql, addPrimaryKey },
                UpdateFromTempDml = updateFromTempDml,
                ClearTempDml = clearTempDml
            };
        }

        public string SelectDmlPageBuilder(string selectDml, string pkColumn, int pageSize, int pageCount)
        {
            var selectDmlBuilder = new StringBuilder();
            selectDmlBuilder.AppendLine(selectDml);
            selectDmlBuilder.AppendLine($"where rownum between {(pageCount - 1) * pageSize + 1 } and {pageCount * pageSize}");
            selectDmlBuilder.AppendLine($"order by {pkColumn}");

            return selectDmlBuilder.ToString();
        }
    }
}