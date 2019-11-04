using Ex8.EtlModel.DatabaseJobManifest;
using Ex8.EtlModel.UnitOfWork;
using Ex8.Helper.Assembly;
using Ex8.SqlDml.Writer.Dbms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using Ex8.Helper.Serialization;
using System.Text;
using Xunit;
using FluentAssertions;
using CsvHelper;

namespace Ex8.SqlDml.Integration.Test.Writer.Dbms
{
   public class MsSqlWriterLoadTest
    {
        private const string _inputRoot = "TestData\\Input\\";
        private const string _outputRoot = "TestData\\Output\\";

        private const string connectionString = "Data Source=winserver1.vm.exatebot.com; Initial Catalog=AdventureWorksLT2016; User ID=ex8ExecuteUser; Password=lbv9hFlO9s1j;";

        [Fact(Skip = "Integration Test. Manual execution only for now")]
        public void UploadTable_SetupSource()
        {
            var data = CreateTable("Pre", 500000);
            var manifestObject = GetJsonFile<DatabaseJobManifest>(_inputRoot, "database.job.adventureWorks.json");
            var table = manifestObject.manifest.tables[1];
            table.temp_name = table.qualified_table_name; //initializing Table-TempName with CustomerATeam Table

            var target = new MsSqlWriter();
            target.ExecuteSqlText(connectionString, new List<string> { $"truncate table {table.temp_name}" }); // Cleaning the CustomerATeam Table first before adding these records with data

            var outputnoOfRecord = target.BulkCopy(connectionString, table, data); // This call should copy records from data to CustomerATeam directly. 
            data.Rows.Count.Should().Be(outputnoOfRecord);
        }

        [Fact(Skip = "Integration Test. Manual execution only for now")]
        public void UploadTable_LoadTest()
        {
            int recordCount = 500000;
            var data = CreateTable("Post", recordCount);

            var manifestObject = GetJsonFile<DatabaseJobManifest>(_inputRoot, "database.job.adventureWorks.json");
            var sql = GetJsonFile<TargetSql>(_inputRoot, "adventureWorks.target.customer.json");

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start(); // start timer

            var target = new MsSqlWriter();
           
            target.UploadTable(connectionString,
                sql.SetupTempDml,
                manifestObject.manifest.tables[1],
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
        public DataTable CreateTable(string runState, int RecordsToBeAdded = 500000)
        {
            var dt = new DataTable();
            string fName = "Daniel", lName = "Saunders", cName="Exate Technology",phone="9898989856";

            dt.Columns.Add("CUSTOMERID", typeof(Int32));
            dt.Columns.Add("FIRSTNAME", typeof(string));
            dt.Columns.Add("MiddleNAME", typeof(string));
            dt.Columns.Add("LASTNAME", typeof(string));
            dt.Columns.Add("FULLNAME", typeof(string));
            dt.Columns.Add("COMPANYNAME", typeof(string));
            dt.Columns.Add("EmailAddress", typeof(string));
            dt.Columns.Add("PHONE", typeof(string));

            for (int counter = 1; counter <= RecordsToBeAdded; counter++)
            {
                dt.Rows.Add(counter, 
                    $"{runState}{fName}_{counter}","", 
                    $"{runState}{lName}_{counter}", 
                    $"{runState}{fName}_{counter} {lName}_{counter}",
                    $"{runState}{cName}",
                    $"{runState}{fName}_{counter}@ExateTechnology.com",
                    $"{runState}{phone}"
                    );
            }
            return dt;
        }

        private void WriteLogCsvFile(CsvLogger data)
        {
            var path = Path.Combine(AssemblyHelper.GetCurrentExecutingAssemblyPath(), string.Format("{0}{1}", _outputRoot, "MsSqlPerformanceLogger.csv"));
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
