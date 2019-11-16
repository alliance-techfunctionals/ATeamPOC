using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;
using Dapper;
using Ex8.EtlModel.DatabaseJobManifest;
using Npgsql;
using NpgsqlTypes;
using Ex8.EtlModel;
using PostgreSQLCopyHelper;
using Ex8.Helper.Converter;

namespace Ex8.SqlDml.Writer.Dbms
{
    public class PostgreSqlWriter : ISqlWriter
    {
        public DatabaseTypeEnum DatabaseType => DatabaseTypeEnum.Postgres;

        public void UploadTable(string connectionString, List<string> setupSql, Table tableInfo, DataTable uploadData, List<string> postSql)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                foreach (var sql in setupSql)
                {
                    connection.Execute(sql);
                }

                BulkCopy(connection, tableInfo.temp_name, tableInfo, uploadData);

                foreach (var sql in postSql)
                {
                    connection.Execute(sql);
                }
            }
        }

        public void ExecuteSqlText(string connectionString, List<string> sqlList)
        {
            using (var targetConn = new NpgsqlConnection(connectionString))
            {
                foreach (var sql in sqlList)
                {
                    targetConn.Execute(sql);
                }
            }
        }

        public int BulkCopy(string connectionString, Table tableInfo, DataTable data)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                return BulkCopy(connection, tableInfo.temp_name, tableInfo, data);
            }
        }

        internal int BulkCopy(NpgsqlConnection connection, string destinationTableName, Table tableInfo, DataTable data)
        {

            var copyHelper = new PostgreSQLCopyHelper<Customer>(destinationTableName)
                .MapInteger("id", x => x.Id)
                .MapVarchar("FirstName", x => x.FirstName)
                .MapVarchar("LastName", x => x.LastName)
                .MapVarchar("email", x => x.Email)
                .MapVarchar("contact", x => x.Contact)
                .MapInteger("age", x => x.Age);


            IEnumerable<Customer> entities = data.DataTableToList<Customer>();

            return Convert.ToInt32(copyHelper.SaveAll(connection, entities));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="colValueArrr"></param>
        /// <returns>
        ///    pgParam
        /// </returns>
        internal NpgsqlParameter _PostgreParameter(string dataType, List<dynamic> colValueArrr)
        {
            NpgsqlParameter pgParam = new NpgsqlParameter();
            switch (dataType.ToLower())
            {
                case "number":
                case "Decimal":
                case "integer":
                    pgParam.NpgsqlDbType = NpgsqlDbType.Double;
                    break;
                case "varchar":
                case "text":
                    pgParam.NpgsqlDbType = NpgsqlDbType.Varchar;
                    break;
                case "datetime":
                case "date":
                    pgParam.NpgsqlDbType = NpgsqlDbType.Date;
                    break;
                // To Do: by Neha - I have to add more datatypes..for now I am just testing this for the above 4 only..
                default:
                    break;
            }

            pgParam.Value = colValueArrr.ToArray();
            return pgParam;
        }



        /// <summary>
        /// Expected command text = INSERT INTO {destinationTableName} (PERSON_ID, FIRST_NAME, LAST_NAME) VALUES (:1, :2, :3)
        /// </summary>
        /// <param name="destinationTableName"></param>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        internal string GetCommandText(string destinationTableName, Table tableInfo)
        {
            int i = 1;
            var sql = new StringBuilder("INSERT INTO " + destinationTableName + " (" + tableInfo.pk_column_name); // handling Primary Key col.
            var values = new StringBuilder($"VALUES ( :{i}");

            foreach (var col in tableInfo.columns)
            {
                sql.Append($",{col.name}");
                i++;
                values.Append($",:{i}");
            }
            sql.Append(") ");
            sql.Append(values.ToString());
            sql.Append(")");
            return sql.ToString();
        }
    }
}
