using System;
using Ex8.EtlModel.DatabaseJobManifest;
using Ex8.Helper.Assembly;
using Ex8.Helper.Serialization;
using Ex8.SqlDml.Writer.Dbms;
using FluentAssertions;
using System.Data;
using System.IO;
using Xunit;
using System.Collections.Generic;

namespace Ex8.SqlDml.Integration.Test.Writer.Dbms
{
    public class Bulk_Insertion
    {

        private const string _inputRoot = "TestData\\Input\\";
        private const string connectionString = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=tcp)(HOST=oracle1.sql.exatebot.com)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=xepdb1)));User Id=TEST_USER;Password=ExateDbUser123!;";

        [Fact]
        public void CanBulkInserstion()
        {

            var data = CreateTable(1000000); // create a simple datatable for Persons Table with 1 mills records

            var manifestObject = GetJsonFile<DatabaseJobManifest>(_inputRoot, "database.manifest.xepdb1.json");

            //initializing Table-TempName with Person Table - to simply re-use our BulkCopy Function as is with no change.
            manifestObject.manifest.tables[0].temp_name = manifestObject.manifest.tables[0].table_name; 

            var target = new OracleSqlWriter();
            target.ExecuteSqlText(connectionString, 
                new List<string> { $"truncate table {manifestObject.manifest.tables[0].temp_name}" });         // Cleaning the Person Table first before adding these records with data
            var outputnoOfRecord = target.BulkCopy(connectionString, manifestObject.manifest.tables[0], data); // This call should copy records from data to Persons directly. 
            data.Rows.Count.Should().Be(outputnoOfRecord);

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
        public DataTable CreateTable(int RecordsToBeAdded = 500000)
        {
            var dt = new DataTable();
            string fName = "Daniel", lName = "saunders";

            dt.Columns.Add("PERSON_ID", typeof(Int32));
            dt.Columns.Add("FIRST_NAME", typeof(string));
            dt.Columns.Add("LAST_NAME", typeof(string));
            for (int counter = 1; counter <= RecordsToBeAdded; counter++)
            {
                dt.Rows.Add(counter, fName + counter, lName + counter);
            }
            return dt;
        }
    }
}
