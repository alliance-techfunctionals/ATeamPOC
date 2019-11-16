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
    public class PostgreSqlWriterTest
    {
        private const string _inputRoot = "TestData\\Input\\";
        private const string connectionString = "User ID=postgres;Password=India@123;Host=localhost;Port=5432;Database=Ex8db1;";

        [Fact]
        public void Can_BulkCopy()
        {
            DataTable data = CreateTable();

            var manifestObject = GetJsonFile<DatabaseJobManifest>(_inputRoot, "database.manifest.PostgreSql.json");
            var inputSqlQueries = GetJsonFile<TargetSql>(_inputRoot, "postgresql.target.customer.json");

            PostgreSqlWriter target = new PostgreSqlWriter();
            target.ExecuteSqlText(connectionString, inputSqlQueries.SetupTempDml);  //Temp Table Created at this stage {ex8_temp_ATeam}
            var outputNOfRecords = target.BulkCopy(connectionString, manifestObject.manifest.tables[0], data); // copy records into temp. table {ex8_temp_ATeam}
            data.Rows.Count.Should().Be(outputNOfRecords);
        }


        [Fact]
        public void CanUploadTable()
        {
            var manifestObject = GetJsonFile<DatabaseJobManifest>(_inputRoot, "database.manifest.PostgreSql.json");
            var sql = GetJsonFile<TargetSql>(_inputRoot, "postgresql.target.customer.json");
            var data = CreateTable();

            var target = new PostgreSqlWriter();
            target.UploadTable(connectionString,
                sql.SetupTempDml,
                manifestObject.manifest.tables[0],
                data,
                new List<string> { sql.UpdateFromTempDml, sql.ClearTempDml });

            var reader = new PostgreSqlReader();

            // Only get the records what you have Updated in database on the basis of Ids {Primary Keys}
            var result = reader.GetData(connectionString, $"select * from sales.Customer where Id in ({string.Join(",", data.AsEnumerable().Select(r => r.Field<dynamic>("Id")))})");
            result.Tables.Should().NotBeEmpty();
            result.Tables[0].Rows.Should().NotBeEmpty();
            result.Tables[0].Should().Equals(data);
        }


        private T GetJsonFile<T>(string root, string file)
        {
            var outputPath = Path.Combine(AssemblyHelper.GetCurrentExecutingAssemblyPath(), string.Format("{0}{1}", root, file));
            string json = File.ReadAllText(outputPath);
            return json.ParseJson<T>();
        }

        private DataTable CreateTable()
        {
            DataTable table = new DataTable();
            table.Clear();
            table.Columns.Add("ID", typeof(Int32));
            table.Columns.Add("FIRSTNAME", typeof(string));
            table.Columns.Add("LASTNAME", typeof(string));
            table.Columns.Add("EMAIL", typeof(string));
            table.Columns.Add("CONTACT", typeof(string));
            table.Columns.Add("AGE", typeof(Int32));

            table.Rows.Add(new object[] { 1, "Daniel", "Saunders", "Daniel@gmail.com", "9898989898", 45 });
            table.Rows.Add(new object[] { 2, "Sonal", "Rattan", "Sonal@gmail.com", "9898989898", 45 });
            table.Rows.Add(new object[] { 3, "Peter", "Lancos", "Peter@gmail.com", "9898989898", 45 });
            table.Rows.Add(new object[] { 4, "Suraj", "Nittoor", "Suraj@gmail.com", "9898989898", 45 });
            table.Rows.Add(new object[] { 5, "Anuj", "Badjatya", "Anuj@gmail.com", "9898989898", 45 });
            table.Rows.Add(new object[] { 6, "Neha", "Verma", "Neha@gmail.com", "9898989898", 45 });
            table.Rows.Add(new object[] { 7, "AbhayKm", "Kumar", "Abhay@gmail.com", "9898989898", 45 });
            table.Rows.Add(new object[] { 8, "Nupur", "Garg", "Nupur@gmail.com", "9898989898", 45 });
            return table;
        }
    }
}
