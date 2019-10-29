using Ex8.SqlDml.Reader.Dbms;
using FluentAssertions;
using Xunit;

namespace Ex8.SqlDml.Integration.Test.Reader.Dbms
{
    public class OracleSqlReaderTest
    {
        private const string connectionString = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=tcp)(HOST=oracle1.sql.exatebot.com)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=xepdb1)));User Id=TEST_USER;Password=ExateDbUser123!;";

        //TODO should not be run in CI-CD pipeline
        //[Fact]
        public void Can_GetData()
        {
            var target = new OracleSqlReader();
            var dataTable = target.GetData(connectionString, "SELECT PERSON_ID, first_name , last_name\n from TEST_USER.Person  ");

            dataTable.Tables.Should().NotBeEmpty();
            dataTable.Tables[0].Rows.Should().NotBeEmpty();
        }
    }
}