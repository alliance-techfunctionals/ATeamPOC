using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.IO;
using Dapper;
using MySql.Data.MySqlClient;
using Ex8.Helper.Dsv;
using Ex8.EtlModel.DatabaseJobManifest;

[assembly: InternalsVisibleTo("Ex8.SqlDml.Integration.Test")]
namespace Ex8.SqlDml.Writer.Dbms
{
    public class MySqlWriter : ISqlWriter
    {
        private const string BulkUploadDelimiter = ",";
        public DatabaseTypeEnum DatabaseType => DatabaseTypeEnum.MySql;

        public void UploadTable(string connectionString, List<string> setupSql, Table tableInfo, DataTable uploadData, List<string> postSql)
        {
            string tempFile = Path.GetTempFileName() + ".csv";

            try
            {
                DsvConverter.WriteDatableDsv(uploadData, tempFile, BulkUploadDelimiter);

                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    foreach (var sql in setupSql)
                    {
                        connection.Execute(sql);
                    }

                    BulkCopy(connection, tableInfo.temp_name, tempFile);

                    foreach (var sql in postSql)
                    {
                        connection.Execute(sql);
                    }
                }
            }
            finally
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
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

        internal int BulkCopy(MySqlConnection connection, string destinationTableName, string sourceFilePath)
        {   
            var msbl = new MySqlBulkLoader(connection);
            msbl.TableName = destinationTableName;
            msbl.FileName = sourceFilePath;
            msbl.FieldTerminator = BulkUploadDelimiter;
            msbl.FieldQuotationCharacter = '"';
            msbl.NumberOfLinesToSkip = 1;
            msbl.Local = true;  //for pushing the data from local.csv to remote
            return msbl.Load(); // load from csv into the destination table        
        }

        public int BulkCopy(string connectionString, Table tableInfo, DataTable sourceTable)
        {
            string tempFile = Path.GetTempFileName() + ".csv";
            try
            {
                DsvConverter.WriteDatableDsv(sourceTable, tempFile, BulkUploadDelimiter);

                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    var recordsCount = BulkCopy(connection, tableInfo.temp_name, tempFile);
                    return recordsCount;
                }
            }
            finally
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }          
    }
}
