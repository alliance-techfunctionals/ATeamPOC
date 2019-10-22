using Ex8.EtlModel.DatabaseJobManifest;
using Ex8.EtlModel.UnitOfWork;
using Ex8.Helper.Assembly;
using Ex8.Helper.Serialization;
using Ex8.SqlDml.Reader.Dbms;
using System.IO;
using Xunit;

namespace Ex8.SqlDml.Test.Writer
{
    public class OracleSqlReaderTest
    {
        private const string _inputRoot = "TestData\\Input\\";
        private const string connectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=tcp)(HOST=oracle1.sql.exatebot.com)(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME=xepdb1)));User Id=TEST_USER;Password=ExateDbUser123!;";
        // private const string _outputRoot = "TestData\\Output\\";

        [Fact]
        public void Can_GetData()
        {           
            var target = new OracleSqlReader();
            var dataTable = target.GetData(connectionString, "SELECT PERSON_ID, first_name , last_name\n from TEST_USER.Person  ");                        
            Assert.NotEmpty(dataTable.Tables);            
        }

        private T GetJsonFile<T>(string root, string file)
        {
            var outputPath = Path.Combine(AssemblyHelper.GetCurrentExecutingAssemblyPath(), string.Format("{0}{1}", root, file));
            string json = File.ReadAllText(outputPath);
            return json.ParseJson<T>();
        }
    }
}