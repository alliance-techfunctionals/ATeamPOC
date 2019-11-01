using System;
using Ex8.EtlModel.DatabaseJobManifest;
using Ex8.EtlModel.UnitOfWork;
using Ex8.Helper.Assembly;
using Ex8.Helper.Serialization;
using Ex8.SqlDml.Writer.Dbms;
using FluentAssertions;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Xunit;
using System.Diagnostics;
using CsvHelper;
using Ex8.SqlDml.Reader.Dbms;

namespace Ex8.SqlDml.Integration.Test.Writer.Dbms
{
    public class PerformanceTesting
    {
        private const string _inputRoot = "TestData\\Input\\";
        private const string _outputRoot = "TestData\\Output\\";

        private const string connectionString = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=tcp)(HOST=oracle1.sql.exatebot.com)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=xepdb1)));User Id=TEST_USER;Password=ExateDbUser123!;";

        [Fact]
        public void MeasureUploadTableCall()
        {
            int RecordsCount = 500000;
            var data = CreateTable(RecordsCount);

            var manifestObject = GetJsonFile<DatabaseJobManifest>(_inputRoot, "database.manifest.xepdb1.json");
            var sql = GetJsonFile<TargetSql>(_inputRoot, "xepdb1.target.person.json");

            Stopwatch stopwatch = new Stopwatch();  
            stopwatch.Start(); // start timer

            var target = new OracleSqlWriter();
            target.UploadTable(connectionString,
                sql.SetupTempDml,
                manifestObject.manifest.tables[0],
                data,
                new List<string> { sql.UpdateFromTempDml, sql.ClearTempDml });

            stopwatch.Stop();  // end timer
            var ElapsedDuration = stopwatch.Elapsed; // this is the elapsed duration now for updating RecordsCount records in table database

            // simply log the value in a csv file output
            WriteLogCsvFile(new CsvLogger {TestDate = DateTime.Now, NoOfRecords = RecordsCount, TimeElapsed = ElapsedDuration});

          
        }

        private T GetJsonFile<T>(string root, string file)
        {
            var outputPath = Path.Combine(AssemblyHelper.GetCurrentExecutingAssemblyPath(), string.Format("{0}{1}", root, file));
            string json = File.ReadAllText(outputPath);
            return json.ParseJson<T>();
        }

        public DataTable CreateTable(int NoOfRecords = 1000)
        {
            var table = new DataTable();
            string FName = "Daniel", LName = "Saunders";
                  
            table.Columns.Add("PERSON_ID", typeof(Int32));
            table.Columns.Add("FIRST_NAME", typeof(string));
            table.Columns.Add("LAST_NAME", typeof(string));
            for (int counter=1; counter <= NoOfRecords; counter++)
            {
                table.Rows.Add(counter, FName + counter, LName + counter);
            }          
            return table;
        }

        private void WriteLogCsvFile(CsvLogger data)
        {
            var path = Path.Combine(AssemblyHelper.GetCurrentExecutingAssemblyPath(), string.Format("{0}{1}", _outputRoot, "PerformanceLogger.csv"));
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
