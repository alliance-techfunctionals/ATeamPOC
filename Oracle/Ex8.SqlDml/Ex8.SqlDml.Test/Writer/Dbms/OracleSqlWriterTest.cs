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
        private const string connectionString = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=tcp)(HOST=oracle1.sql.exatebot.com)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=xepdb1)));User Id=TEST_USER;Password=ExateDbUser123!;";
        private const string destinationTableName = "EX8_TEMP_PERSON";
        // private const string _outputRoot = "TestData\\Output\\";  // Its not being used here.

        [Fact]
        public void Can_BulkCopy()
        {
            var datatocopy = CreateTable(); //first create the datatocopy table which will be copied to destination, we can pass this datatable as well ?

            var manifestObject = GetJsonFile<DatabaseJobManifest>(_inputRoot, "database.manifest.xepdb1.json");
            OracleSqlWriter target = new OracleSqlWriter();
            var outputnoOfRecord = target.BulkCopy(connectionString, destinationTableName, manifestObject.manifest.tables[0], datatocopy);

            Assert.Equal(datatocopy.Rows.Count, outputnoOfRecord);

            var reader = new OracleSqlReader();
            var result = reader.GetData(connectionString, $"select * from {destinationTableName}");
            Assert.NotEmpty(result.Tables[0].Rows);
        }


        [Fact]
        public void Can_ExecuteUpdateQuery()
        {
            var manifestObject = GetJsonFile<DatabaseJobManifest>(_inputRoot, "database.manifest.xepdb1.json");
            var queryFiles = GetJsonFile<TargetSql>(_inputRoot, "xepdb1.target.person.json");
            
            var datatocopy = CreateTable(); // first get the data to be updated
            OracleSqlWriter target = new OracleSqlWriter();
            target.BulkCopy(manifestObject.manifest.sourceConnectionString, destinationTableName, manifestObject.manifest.tables[0], datatocopy); // write data into temp table using this library

            int affectedrows = target.executeUpdateQuery(manifestObject.manifest.sourceConnectionString, queryFiles.UpdateFromTempDml); // update the table from temp
            Assert.NotEqual(-1, affectedrows); // assuming that if we get number of rows affected = -1 then there is some problem else update worked
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

            //table.Rows.Add(1, "Daniel", "Saunders");
            //table.Rows.Add(2, "Sonal", "Rattan");
            //table.Rows.Add(3, "Peter", "Lancos");
            //table.Rows.Add(4, "Suraj", "Nittoor");

            // changed this so that to see if I can have different values back into the same table as a test purpose only!
            table.Rows.Add(1, "Anuj", "Jain");
            table.Rows.Add(2, "Neha", "Verma");
            table.Rows.Add(3, "Khushboo", "Jain");
            table.Rows.Add(4, "Nupur", "Garg");
            //table.Rows.Add(5, "Anuj1", "Jain1");
            //table.Rows.Add(6, "Neha1", "Verma1");
            //table.Rows.Add(7, "DD1", "PP1");
            //table.Rows.Add(8, "JJ1", "KK1");
            //table.Rows.Add(9, "Anuj2", "Jain2");
            //table.Rows.Add(10, "Neha1", "Verma1");
            //table.Rows.Add(11, "DD1", "PP1");
            //table.Rows.Add(12, "JJ1", "KK1");
            return table;
        }
    }
}