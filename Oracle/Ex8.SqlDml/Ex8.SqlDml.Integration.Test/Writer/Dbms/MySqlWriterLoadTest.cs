using CsvHelper;
using Ex8.EtlModel.DatabaseJobManifest;
using Ex8.EtlModel.UnitOfWork;
using Ex8.Helper.Assembly;
using Ex8.Helper.Serialization;
using Ex8.SqlDml.Writer.Dbms;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using Xunit;

namespace Ex8.SqlDml.Integration.Test.Writer.Dbms
{
    public class MySqlWriterLoadTest
    {
        private const string _inputRoot = "TestData\\Input\\";
        private const string _outputRoot = "TestData\\Output\\";

        private const string connectionString = "server=182.50.133.84;port=3306;user='ex8db1';password='ex8db1@123';database=ex8_db1;AllowLoadLocalInfile='true'";

        [Fact]
        public void UploadTable_SetupSource()
        {
            var data = CreateTable("Pre", 500000);
            var manifestObject = GetJsonFile<DatabaseJobManifest>(_inputRoot, "database.manifest.mysql.json");
            var table = manifestObject.manifest.tables[0];
            table.temp_name = table.qualified_table_name;  //initialized Table-TempName with ATeam Table

            var target = new MySqlWriter();
            target.ExecuteSqlText(connectionString, new List<string> { $"truncate table {table.temp_name}" }); // Cleaning the ATeam Table first before adding these records with data

            var outputnoOfRecord = target.BulkCopy(connectionString, table, data); // This call should copy records from data to ATeam directly. 
            data.Rows.Count.Should().Be(outputnoOfRecord);
        }

        [Fact]
        public void UploadTable_LoadTest()
        {
            int recordCount = 500000;
            var data = CreateTable("Post", recordCount);

            var manifestObject = GetJsonFile<DatabaseJobManifest>(_inputRoot, "database.manifest.mysql.json");
            var sql = GetJsonFile<TargetSql>(_inputRoot, "mysql.target.ateam.json");

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start(); // start timer

            var target = new MySqlWriter();

            target.UploadTable(connectionString,
                sql.SetupTempDml,
                manifestObject.manifest.tables[0],
                data,
                new List<string> { sql.UpdateFromTempDml, sql.ClearTempDml });

            stopwatch.Stop();  // end timer
            var ElapsedDuration = stopwatch.Elapsed; // this is the elapsed duration for updating RecordsCount records in table database

            WriteLogCsvFile(new CsvLogger { TestDate = DateTime.Now, NoOfRecords = recordCount, TimeElapsed = ElapsedDuration });
        }

        private T GetJsonFile<T>(string root, string file)
        {
            var outputPath = Path.Combine(AssemblyHelper.GetCurrentExecutingAssemblyPath(), string.Format("{0}{1}", root, file));
            string json = File.ReadAllText(outputPath);
            return json.ParseJson<T>();
        }

        // This funtion would create a datatable with a specific no. of records
        // by default - I have set this up for 500,000 but could be changed      
        public DataTable CreateTable(string runState, int RecordsToBeAdded = 500000)
        {
            var dt = new DataTable();
            string fName = "Daniel", lName = "Saunders";

            dt.Columns.Add("ID", typeof(Int32));
            dt.Columns.Add("FIRST_NAME", typeof(string));
            dt.Columns.Add("LAST_NAME", typeof(string));           

            for (int counter = 1; counter <= RecordsToBeAdded; counter++)
            {
                dt.Rows.Add(counter,
                    $"{runState}{fName}_{counter}",
                    $"{runState}{lName}_{counter}"                  
                    );
            }
            return dt;
        }

        private void WriteLogCsvFile(CsvLogger data)
        {
            var path = Path.Combine(AssemblyHelper.GetCurrentExecutingAssemblyPath(), string.Format("{0}{1}", _outputRoot, "MySqlPerformanceLogger.csv"));
            using (StreamWriter writer = new StreamWriter(path, append: true))
            {
                var Testwriter = new CsvWriter(writer);
                Testwriter.Configuration.Delimiter = ",";
                Testwriter.Configuration.AutoMap<CsvLogger>();
                Testwriter.Configuration.HasHeaderRecord = true;
                var record = new List<CsvLogger> { data };
                Testwriter.WriteRecords(record);
                writer.Flush();
            }
        }
    }
}
