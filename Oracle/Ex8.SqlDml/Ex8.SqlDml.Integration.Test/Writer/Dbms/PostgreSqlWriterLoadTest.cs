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
    public class PostgreSqlWriterLoadTest
    {
        private const string _inputRoot = "TestData\\Input\\";
        private const string _outputRoot = "TestData\\Output\\";

        private const string connectionString = "User ID=postgres;Password=India@123;Host=localhost;Port=5432;Database=Ex8db1;";

        [Fact]
        public void UploadTable_SetupSource()
        {
            var data = CreateTable("Pre", 60000);
            var manifestObject = GetJsonFile<DatabaseJobManifest>(_inputRoot, "database.manifest.PostgreSql.json");
            var table = manifestObject.manifest.tables[0];
            table.temp_name = table.qualified_table_name; //initializing Table-TempName with Person Table

            var target = new PostgreSqlWriter();
            target.ExecuteSqlText(connectionString, new List<string> { $"truncate table {table.temp_name}" }); // Cleaning the Person Table first before adding these records with data

            var outputnoOfRecord = target.BulkCopy(connectionString, table, data); // This call should copy records from data to Persons directly. 
            data.Rows.Count.Should().Be(outputnoOfRecord);
        }

        [Fact]
        public void UploadTable_LoadTest()
        {
            int recordCount = 500000;
            var data = CreateTable("Post", recordCount);

            var manifestObject = GetJsonFile<DatabaseJobManifest>(_inputRoot, "database.manifest.PostgreSql.json");
            var sql = GetJsonFile<TargetSql>(_inputRoot, "postgresql.target.customer.json");

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start(); // start timer

            var target = new PostgreSqlWriter();
            target.UploadTable(connectionString,
                sql.SetupTempDml,
                manifestObject.manifest.tables[0],
                data,
                new List<string> { sql.UpdateFromTempDml, sql.ClearTempDml });

            stopwatch.Stop();  // end timer
            var ElapsedDuration = stopwatch.Elapsed; // this is the elapsed duration now for updating RecordsCount records in table database

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
        // it keeps appending FirstName and LastName strings with a counter
        public DataTable CreateTable(string runState, int RecordsToBeAdded = 500000)
        {
            var table = new DataTable();
            string fName = "Daniel", lName = "Saunders";
            table.Columns.Add("ID", typeof(decimal));
            table.Columns.Add("FIRSTNAME", typeof(string));
            table.Columns.Add("LASTNAME", typeof(string));
            table.Columns.Add("EMAIL", typeof(string));
            table.Columns.Add("CONTACT", typeof(string));
            table.Columns.Add("AGE", typeof(Int32));
            for (int counter = 1; counter <= RecordsToBeAdded; counter++)
            {
                table.Rows.Add(counter, 
                    $"{runState}{fName}_{counter}",
                    $"{runState}{lName}_{counter}",
                    $"{runState}{fName}_{counter}@gmail.com",
                    $"9898989898",
                    $"45"
                    );
            }
            return table;
        }

        private void WriteLogCsvFile(CsvLogger data)
        {
            var path = Path.Combine(AssemblyHelper.GetCurrentExecutingAssemblyPath(), string.Format("{0}{1}", _outputRoot, "PostgresPerformanceLogger.csv"));
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
