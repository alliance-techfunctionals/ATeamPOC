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
    public class OracleSqlWriterTest
    {
        private const string _inputRoot = "TestData\\Input\\";
        private const string connectionString = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=tcp)(HOST=oracle1.sql.exatebot.com)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=xepdb1)));User Id=TEST_USER;Password=ExateDbUser123!;";
        // private const string _outputRoot = "TestData\\Output\\";  // Its not being used here.

        [Fact(Skip = "Integration Test. Manual execution only for now")]
        public void Can_BulkCopy()
        {
            var data = CreateTable();
            var manifestObject = GetJsonFile<DatabaseJobManifest>(_inputRoot, "database.manifest.xepdb1.json");

            OracleSqlWriter target = new OracleSqlWriter();
            var outputnoOfRecord = target.BulkCopy(connectionString, manifestObject.manifest.tables[0], data);

            data.Rows.Count.Should().Be(outputnoOfRecord);
        }

        [Fact(Skip = "Integration Test. Manual execution only for now")]
        public void CanUploadTable()
        {
            var manifestObject = GetJsonFile<DatabaseJobManifest>(_inputRoot, "database.manifest.xepdb1.json");
            var sql = GetJsonFile<TargetSql>(_inputRoot, "xepdb1.target.person.json");
            var data = CreateTable();

            var target = new OracleSqlWriter();
            target.UploadTable(connectionString,
                sql.SetupTempDml,
                manifestObject.manifest.tables[0],
                data,
                new List<string> { sql.UpdateFromTempDml, sql.ClearTempDml });

            var reader = new OracleSqlReader();
            var result = reader.GetData(connectionString, $"select * from TEST_USER.Person");
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
            table.Columns.Add("PERSON_ID", typeof(decimal));
            table.Columns.Add("FIRST_NAME", typeof(string));
            table.Columns.Add("LAST_NAME", typeof(string));

            table.Rows.Add(1, "Daniel", "Saunders");
            table.Rows.Add(2, "Sonal", "Rattan");
            table.Rows.Add(3, "Peter", "Lancos");
            table.Rows.Add(4, "Suraj", "Nittoor");

            // changed this so that to see if I can have different values back into the same table as a test purpose only!
            //table.Rows.Add(1, "Anuj", "Jain");
            //table.Rows.Add(2, "Neha", "Verma");
            //table.Rows.Add(3, "Khushboo", "Jain");
            //table.Rows.Add(4, "Nupur", "Garg");
            return table;
        }
    }
}