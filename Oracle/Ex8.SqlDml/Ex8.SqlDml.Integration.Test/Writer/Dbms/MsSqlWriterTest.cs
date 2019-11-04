using Ex8.EtlModel.DatabaseJobManifest;
using Ex8.EtlModel.UnitOfWork;
using Ex8.Helper.Assembly;
using Ex8.Helper.Serialization;
using Ex8.SqlDml.Reader.Dbms;
using Ex8.SqlDml.Writer.Dbms;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using Xunit;

namespace Ex8.SqlDml.Integration.Test.Writer.Dbms
{
    public class MsSqlWriterTest
    {
        private const string _inputRoot = "TestData\\Input\\";
        private const string connectionString = "Data Source=winserver1.vm.exatebot.com; Initial Catalog=AdventureWorksLT2016; User ID=ex8ExecuteUser; Password=lbv9hFlO9s1j;";

        [Fact]
        public void Can_BulkCopy()
        {
            var data = CreateTable();
            var manifestObject = GetJsonFile<DatabaseJobManifest>(_inputRoot, "database.job.adventureWorks.json");
           
            var inputSqlQueries = GetJsonFile<TargetSql>(_inputRoot, "adventureWorks.target.customer.json");
           
            MsSqlWriter target = new MsSqlWriter();
            target.ExecuteSqlText(connectionString, inputSqlQueries.SetupTempDml);
           
            var outputnoOfRecord = target.BulkCopy(connectionString, manifestObject.manifest.tables[1], data);
            data.Rows.Count.Should().Be(outputnoOfRecord);
        }

        [Fact]
        public void CanUploadTable()
        {
            var manifestObject = GetJsonFile<DatabaseJobManifest>(_inputRoot, "database.job.adventureWorks.json");
            var inputSqlQueries = GetJsonFile<TargetSql>(_inputRoot, "adventureWorks.target.customer.json");
            var data = CreateTable();

            var target = new MsSqlWriter();

            target.UploadTable(connectionString,
                inputSqlQueries.SetupTempDml,
                manifestObject.manifest.tables[1],
                data,
                new List<string> { inputSqlQueries.UpdateFromTempDml, inputSqlQueries.ClearTempDml }); 

            var reader = new MsSqlReader();
            var result = reader.GetData(connectionString, $"select * from SalesLT.CustomerATeam");
            result.Tables.Should().NotBeEmpty();
            result.Tables[0].Rows.Should().NotBeEmpty();

        }

        private T GetJsonFile<T>(string root, string file)
        {
            var outputPath = Path.Combine(AssemblyHelper.GetCurrentExecutingAssemblyPath(), string.Format("{0}{1}", root, file));
            string json = File.ReadAllText(outputPath);
            return json.ParseJson<T>();
        }

        private DataTable CreateTable()
        {
            var table = new DataTable();

            table.Columns.Add("CUSTOMERID",typeof(Int32));
            table.Columns.Add("FIRSTNAME", typeof(string));
            table.Columns.Add("MiddleNAME", typeof(string));
            table.Columns.Add("LASTNAME", typeof(string));
            table.Columns.Add("FULLNAME", typeof(string));
            table.Columns.Add("COMPANYNAME", typeof(string));
            table.Columns.Add("EmailAddress", typeof(string));
            table.Columns.Add("PHONE", typeof(string));

            table.Rows.Add(1,"Daniel","", "Saunders", "Daniel Saunders","EXATE TECHNOLOGY", "Saunders@exateTechnology.com","9797989895");
            table.Rows.Add(2,"Sonal", "", "Rattan", "Sonal Rattan", "EXATE TECHNOLOGY", "Sonal@exateTechnology.com", "9797989895");
            table.Rows.Add(3,"Peter", "", "Lancos", "Peter Lancos", "EXATE TECHNOLOGY", "Lancos@exateTechnology.com", "9797989895");
            table.Rows.Add(4,"Suraj", "", "Nittoor", "Suraj Nittoor", "EXATE TECHNOLOGY", "Suraj@exateTechnology.com", "9797989895");
           
          return table;
        }
    }
}
