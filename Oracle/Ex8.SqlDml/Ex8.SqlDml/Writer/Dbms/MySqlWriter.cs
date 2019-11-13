using Dapper;
using Ex8.EtlModel.DatabaseJobManifest;
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.IO;
using Ex8.Helper.Assembly;

[assembly: InternalsVisibleTo("Ex8.SqlDml.Integration.Test")]
namespace Ex8.SqlDml.Writer.Dbms
{
    public class MySqlWriter : ISqlWriter
    {
        public DatabaseTypeEnum DatabaseType => DatabaseTypeEnum.MySql;

        public void UploadTable(string connectionString, List<string> setupSql, Table tableInfo, DataTable uploadData, List<string> postSql)
        {
            using (var connection = new MySqlConnection(connectionString))
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
            using (var targetConn = new MySqlConnection(connectionString))
            {
                foreach (var sql in sqlList)
                {
                    targetConn.Execute(sql);
                }
            }
        }

        //  MySql Implementation of BulkCopy 
        //  1. first the records have to be copied into a temp. csv file 
        //  2. load the csv file directly into the destination table
        //  3. delete the temp .csv file as its not needed
        internal void BulkCopy(MySqlConnection connection, string destinationTableName, DataTable sourceTable)
        {
            try
            {
                string tmpcsvfilepath = Path.Combine(AssemblyHelper.GetCurrentExecutingAssemblyPath(), string.Format("{0}{1}", "TestData\\Output\\", "TmpBulkfile.csv"));               
                datatocsvfile(tmpcsvfilepath, sourceTable); // copy records into a temp csv
                
                var msbl = new MySqlBulkLoader(connection);
                msbl.TableName = destinationTableName;
                msbl.FileName = tmpcsvfilepath;
                msbl.FieldTerminator = ",";
                msbl.FieldQuotationCharacter = '"';
                msbl.NumberOfLinesToSkip = 1;
                msbl.Local = true;  //for pushing the data from local.csv to remote
                msbl.Load(); // load from csv into the destination table
                System.IO.File.Delete(tmpcsvfilepath); // delete the temp file
            }
            catch(Exception ex) { 
                throw ex;
            }           
        }

        public int BulkCopy(string connectionString, Table tableInfo, DataTable sourceTable)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                
                BulkCopy(connection, tableInfo.temp_name, sourceTable);
                var recordsCount = connection.ExecuteScalar<int>($"select count(*) from {tableInfo.temp_name}");
                return recordsCount;
            }
        }   
        
        // copy all records from source table into a .csv file filepath
        internal void datatocsvfile(string filepath,DataTable sourceTable)
        {  
            try
            {
                IEnumerable<string> columnNames = sourceTable.Columns.Cast<DataColumn>().Select(column => column.ColumnName);

                StringBuilder sb = new StringBuilder();
                sb.AppendLine(string.Join(",", columnNames));

                foreach (DataRow row in sourceTable.Rows)
                {                    
                    string[] fields = row.ItemArray.Select(field => field.ToString()).ToArray();
                    sb.AppendLine(string.Join(",", fields));
                }
                File.WriteAllText(filepath, sb.ToString());
            }
             catch(Exception ex)
            {
                throw ex;
            }

        }

    }
}
