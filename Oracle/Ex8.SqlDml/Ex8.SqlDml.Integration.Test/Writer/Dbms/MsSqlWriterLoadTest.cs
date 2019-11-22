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
        private const string connectionString = "Data Source=winserver1.vm.exatebot.com; Initial Catalog=AdventureWorksLT2016_dev; User ID=ex8ExecuteUser; Password=lbv9hFlO9s1j;";

        private const int InitialCustomerTableRowCount = 847;
        private const int InitialAddressTableRowCount = 450;

        [Fact(Skip = "Integration Test. Manual execution only for now")]
        public void UploadTable_CustomerATeam_SetupSource()
        {
            var data = CreateTable("Pre", 500000);
            var table = GetJsonFile<Table>(_inputRoot, "table.SalesLT.CustomerATeam.json");
            table.temp_name = table.qualified_table_name; //initializing Table-TempName with CustomerATeam Table

            var target = new MsSqlWriter();
            target.ExecuteSqlText(connectionString, new List<string> { $"truncate table {table.temp_name}" }); // Cleaning the CustomerATeam Table first before adding these records with data

            var outputnoOfRecord = target.BulkCopy(connectionString, table, data); // This call should copy records from data to CustomerATeam directly. 
            data.Rows.Count.Should().Be(outputnoOfRecord);
        }

        [Fact(Skip = "Integration Test. Manual execution only for now")]
        public void UploadTable_CustomerATeam_LoadTest()
        {
            int recordCount = 500000;
            var data = CreateTable("Post", recordCount);

            var table = GetJsonFile<Table>(_inputRoot, "table.SalesLT.CustomerATeam.json");
            var sql = GetJsonFile<TargetSql>(_inputRoot, "adventureWorks.target.customer.json");

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start(); // start timer

            var target = new MsSqlWriter();
           
            target.UploadTable(connectionString,
                sql.SetupTempDml,
                table,
                data,
                new List<string> { sql.UpdateFromTempDml, sql.ClearTempDml });

            stopwatch.Stop();  // end timer
            var ElapsedDuration = stopwatch.Elapsed; // this is the elapsed duration now for updating RecordsCount records in table database
            WriteLogCsvFile(new CsvLogger { TestDate = DateTime.Now, NoOfRecords = recordCount, TimeElapsed = ElapsedDuration });
        }

        [Fact(Skip = "Integration Test. Manual execution only for now")]
        public void UploadTable_Customer_SetupSource()
        {
            var data = CreateTableForCustomer("Pre", 300000);
            var table = GetJsonFile<Table>(_inputRoot, "table.SalesLT.Customer.json");
            table.temp_name = table.qualified_table_name; //initializing Table-TempName with Customer Table

            var target = new MsSqlWriter();
           
            var rowsInserted = target.BulkCopy(connectionString, table, data);
            data.Rows.Count.Should().Be(rowsInserted - InitialCustomerTableRowCount );
        }

        [Fact(Skip = "Integration Test. Manual execution only for now")]
        public void UploadTable_Address_SetupSource()
        {
            var data = CreateTableForAddress("Pre",1000000);
            var table = GetJsonFile<Table>(_inputRoot, "table.SalesLT.Address.json");
            table.temp_name = table.qualified_table_name; 

            var target = new MsSqlWriter();

            var rowsInserted = target.BulkCopy(connectionString, table, data);
            data.Rows.Count.Should().Be(rowsInserted - InitialAddressTableRowCount);
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

        public DataTable CreateTableForCustomer(string runState, int RecordsToBeAdded = 500000)
        {
            var dt = new DataTable();
            string fName = "Daniel", lName = "Saunders", cName = "exate", phone = "9898989856", title = "Mr.", SalesPerson = @"adventure-works\jillian";

            int startingRecordCustId = 30119; // this i checked from database, should I do this by dynamically pulling this up ?           
            int lastRecordCustId = (RecordsToBeAdded - InitialCustomerTableRowCount) + (startingRecordCustId - 1); // calculate the last record Id here 

            DateTime dateTime = DateTime.Now;
            dt.Columns.Add("CustomerID", typeof(Int32));
            dt.Columns.Add("NameStyle", typeof(bool));
            dt.Columns.Add("Title", typeof(string));
            dt.Columns.Add("FirstName", typeof(string));
            dt.Columns.Add("MiddleName", typeof(string));
            dt.Columns.Add("LastName", typeof(string));
            dt.Columns.Add("Suffix", typeof(string));
            dt.Columns.Add("FullName", typeof(string));
            dt.Columns.Add("CompanyName", typeof(string));
            dt.Columns.Add("SalesPerson", typeof(string));
            dt.Columns.Add("EmailAddress", typeof(string));
            dt.Columns.Add("Phone", typeof(string));
            dt.Columns.Add("PasswordHash", typeof(string));
            dt.Columns.Add("PasswordSalt", typeof(string));
            dt.Columns.Add("rowguid", typeof(Guid));
            dt.Columns.Add("ModifiedDate", typeof(DateTime));

            for (int counter = startingRecordCustId; counter <= lastRecordCustId; counter++)
            {
                dt.Rows.Add(counter,
                    false,
                    $"{title}",
                    $"{fName}_{counter}",
                    null,
                    $"{lName}_{counter}",
                    null,
                    $"{fName}_{counter} {lName}_{counter}",
                    $"{cName}",
                    $"{SalesPerson}_{counter}",
                    $"{fName}_{counter}@{cName}.com",
                    $"{phone}",
                    $"U{counter}/CrPqSzwLTtwgBehfpIl7f1LHSFpZw1qnG1sMzFjo=",
                    $"{Guid.NewGuid().ToString("d").Substring(1, 7).ToUpper()}=",
                    Guid.NewGuid(),
                    $"{dateTime}"
                    );
            }
            return dt;
        }

        public DataTable CreateTableForAddress(string runState, int RecordsToBeAdded = 500000)
        {
            var dt = new DataTable();
            string add1 = "Agra", state = "UP", city = "agra", country = "India", postal = "282004";
           
            int startingRecordAddId = 11383;  // this i checked from database last Id = 11382 so we start from 11383 onwards
            int lastRecordAddId = (RecordsToBeAdded - InitialAddressTableRowCount) + (startingRecordAddId - 1); // calculate the last record Address Id 

            DateTime dateTime = DateTime.Now;
            dt.Columns.Add("AddressID", typeof(Int32));
            dt.Columns.Add("AddressLine1", typeof(string));
            dt.Columns.Add("City", typeof(string));           
            dt.Columns.Add("StateProvince", typeof(string));
            dt.Columns.Add("CountryRegion", typeof(string));
            dt.Columns.Add("PostalCode", typeof(string));
            dt.Columns.Add("ModifiedDate", typeof(string));


            for (int counter = startingRecordAddId; counter <= lastRecordAddId; counter++) // 
            {
                dt.Rows.Add(counter,
                    $"{add1}",
                    $"{city}_{counter}",
                    $"{state}_{counter}",
                    $"{country}_{counter}",
                    $"{postal}",
                    $"{dateTime}"
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
