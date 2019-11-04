using Dapper;
using Ex8.EtlModel.DatabaseJobManifest;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Ex8.SqlDml.Writer.Dbms
{
    public class MsSqlWriter : ISqlWriter
    {

        // This is because the scope of a Local Temporary table is only bounded with the current connection of the current user
        SqlConnection _sqlConnection = new SqlConnection();

        public DatabaseTypeEnum DatabaseType => DatabaseTypeEnum.SqlServer;

        public void UploadTable(string connectionString, List<string> setupSql, Table tableInfo, DataTable uploadData, List<string> postSql)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (var sql in setupSql)
                {
                    connection.Execute(sql);
                }

                BulkCopy(connection, tableInfo.temp_name, uploadData);

                foreach (var sql in postSql)
                {
                    connection.Execute(sql);
                }
            }
        }

        public void ExecuteSqlText(string connectionString, List<string> sqlList)
        {
            _sqlConnection.ConnectionString = connectionString;

            // Neha@ATeam - I commented these lines as I dont want to open a connection everytime to execute a sql-query.
            // I have a property _sqlConnection which I can reuse.

            //using (var targetConn = new SqlConnection(connectionString))
            //{
            //    targetConn.Open();

            _sqlConnection.Open();
            foreach (var sql in sqlList)
            {
                _sqlConnection.Execute(sql);
            }               
        }

        internal void BulkCopy(SqlConnection connection, string destinationTableName, DataTable sourceTable)
        {
            using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.KeepIdentity, null))
            {
                bulkCopy.BulkCopyTimeout = 0;
                bulkCopy.DestinationTableName = destinationTableName;                             
                bulkCopy.WriteToServer(sourceTable);              
                bulkCopy.Close();
            }
        }

        public int BulkCopy(string connectionString, Table tableInfo, DataTable sourceTable)
        {
            if (_sqlConnection.State != ConnectionState.Open)
            {
                _sqlConnection.ConnectionString = connectionString;
                _sqlConnection.Open();
            }

            BulkCopy(_sqlConnection, tableInfo.temp_name, sourceTable); // call this function to bulkcopy the data from source dt to dest. dt

            var recordsCount = _sqlConnection.ExecuteScalar($"select Count(*) from {tableInfo.temp_name}"); // check #records in dest Table
            _sqlConnection.Close();    // close the connection as you are done with copying..
            return (int)recordsCount;  // this returns the number of records inserted in dest.table
        }
    }
}
