using Ex8.EtlModel.DatabaseJobManifest;
using Ex8.EtlModel.UnitOfWork;
using Ex8.Helper.Assembly;
using Ex8.Helper.Serialization;
using Ex8.SqlDml.Reader.Dbms;
using Ex8.SqlDml.Writer.Dbms;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using Xunit;

namespace Ex8.SqlDml.Test.Writer
{
    public class OracleSqlWriterTest
    {
        private const string _inputRoot = "TestData\\Input\\";
        private const string connectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=tcp)(HOST=oracle1.sql.exatebot.com)(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME=xepdb1)));User Id=TEST_USER;Password=ExateDbUser123!;";
        private const string destinationTableName = "EX8_TEMP_PERSON";
        // private const string _outputRoot = "TestData\\Output\\";

        [Fact]
        public void Can_BulkCopy()
        {
            var manifestObject = GetJsonFile<DatabaseJobManifest>(_inputRoot, "database.manifest.xepdb1.json");
            var data = CreateTable();

            OracleSqlWriter target = new OracleSqlWriter();
            var outputnoOfRecord = target.BulkCopy(connectionString, destinationTableName, manifestObject.manifest.tables[0], data);

            Assert.Equal(data.Rows.Count, outputnoOfRecord);
            var reader = new OracleSqlReader();
            var result = reader.GetData(connectionString, $"select * from {destinationTableName}");
            Assert.NotEmpty(result.Tables[0].Rows);
        }

        private T GetJsonFile<T>(string root, string file)
        {
            var outputPath = Path.Combine(AssemblyHelper.GetCurrentExecutingAssemblyPath(), string.Format("{0}{1}", root, file));
            string json = File.ReadAllText(outputPath);
            return json.ParseJson<T>();
        }

        public DataTable CreateTable()
        {
            var table = new DataTable();
            table.Columns.Add("PERSON_ID", typeof(decimal));
            table.Columns.Add("FIRST_NAME", typeof(string));
            table.Columns.Add("LAST_NAME", typeof(string));

            table.Rows.Add(1, "Daniel", "Saunders");
            table.Rows.Add(2, "Sonal", "Rattan");
            table.Rows.Add(3, "Peter", "Lancos");
            table.Rows.Add(4, "Suraj", "Nittoor");

            return table;
        }
    }
}