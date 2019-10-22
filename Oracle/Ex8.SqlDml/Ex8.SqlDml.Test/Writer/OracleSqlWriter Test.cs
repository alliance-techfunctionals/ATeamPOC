using Ex8.EtlModel.DatabaseJobManifest;
using Ex8.EtlModel.UnitOfWork;
using Ex8.Helper.Assembly;
using Ex8.Helper.Serialization;
using Ex8.SqlDml.Writer.Dbms;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using Xunit;

namespace Ex8.SqlDml.Test.Writer
{
    public class OracleSqlWriter_Test
    {
        private const string _inputRoot = "TestData\\Input\\";
        private const string connectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=tcp)(HOST=oracle1.sql.exatebot.com)(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME=xepdb1)));User Id=TEST_USER;Password=ExateDbUser123!;";
        private const string destinationTableName = "EX8_TEMP_PERSON";
        // private const string _outputRoot = "TestData\\Output\\";
        [Fact]
        public void Can_getDataTest()
        {           
            var input = GetJsonFile<TargetSql>(_inputRoot, "xepdb1.target.person.json");
            OracleSqlWriter target = new OracleSqlWriter();
            target.ExecuteSqlText(connectionString, input.SetupTempDml);
            var dataTable = target.GetData(connectionString, input.SelectDml);                        
            Assert.NotEmpty(dataTable.Tables);            
        }

        [Fact]
        public void Can_BulkCopyTest()
        {
            var input = GetJsonFile<TargetSql>(_inputRoot, "xepdb1.target.person.json");
            OracleSqlWriter target = new OracleSqlWriter();            
            var dataTable = target.GetData(connectionString, input.SelectDml);
            DataTable data = dataTable.Tables[0];           
            var outputnoOfRecord = target.BulkCopy(connectionString, destinationTableName, data);
            Assert.Equal(data.Rows.Count, outputnoOfRecord);
        }



        private T GetJsonFile<T>(string root, string file)
        {
            var outputPath = Path.Combine(AssemblyHelper.GetCurrentExecutingAssemblyPath(), string.Format("{0}{1}", root, file));
            string json = File.ReadAllText(outputPath);
            return json.ParseJson<T>();
        }
    }
}