using System.Data;
using Oracle.ManagedDataAccess.Client;
using Xunit;
using Ex8.SqlDml.Builder.Command;

namespace Ex8.SqlDml.Test.Builder.Command
{
    public class OracleSqlCommandStringBuilderTest
    {
        [Fact]
        public void Test_Should_Generate_SQL()
        {
            var cmd = new OracleCommand("GetEntity", null);
            cmd.Parameters.Add(new OracleParameter("@foobar", 1));
            cmd.Parameters.Add(new OracleParameter()
            {
                ParameterName = "@outParam",
                Direction = ParameterDirection.Output,
                OracleDbType = OracleDbType.Int32
            });
            cmd.Parameters.Add(new OracleParameter()
            {
                Direction = ParameterDirection.ReturnValue
            });
            cmd.CommandType = CommandType.StoredProcedure;
            var sqlCommandStringBuilder = new OracleSqlCommandStringBuilder();
            var sql = sqlCommandStringBuilder.GetCommandText(cmd);

            Assert.NotEmpty(sql);
        }

        [Fact]
        public void Test_Should_Generate_Update()
        {
            var cmd = new OracleCommand("update TEST_USER.person  SET  first_name=@param1 where last_name=@whereparam1", null);
            cmd.Parameters.Add(new OracleParameter("@param1", "Charlotte"));
            cmd.Parameters.Add(new OracleParameter("@whereparam1", "Saunders"));

            cmd.CommandType = CommandType.Text;
            var expectedSQl =
                "DECLARE @param1 VARCHAR2 = N'Charlotte';\r\nDECLARE @whereparam1 VARCHAR2 = N'Saunders';\r\nupdate TEST_USER.person  SET  first_name=@param1 where last_name=@whereparam1\r\n;\r\n";
            var sqlCommandStringBuilder = new OracleSqlCommandStringBuilder();
            var result = sqlCommandStringBuilder.GetCommandText(cmd);

            Assert.NotEmpty(result);
            Assert.Equal(expectedSQl.Replace("\n", "").Replace("\r", ""), result.Replace("\n", "").Replace("\r", ""));
        }

        [Fact]
        public void Test_Should_Generate_InlineUpdate()
        {
            var cmd = new OracleCommand("update TEST_USER.person   SET  first_name=@param1 where last_name=@whereparam1", null);
            cmd.Parameters.Add(new OracleParameter("@param1", "Charlotte"));
            cmd.Parameters.Add(new OracleParameter("@whereparam1", "Saunders"));

            cmd.CommandType = CommandType.Text;
            var expectedSQl =
                "update TEST_USER.person   SET  first_name=N'Charlotte' where last_name=N'Saunders'";
            var sqlCommandStringBuilder = new OracleSqlCommandStringBuilder();
            var result = sqlCommandStringBuilder.GetCommandTextInline(cmd);

            Assert.NotEmpty(result);
            Assert.Equal(expectedSQl.Replace("\n", "").Replace("\r", ""), result.Replace("\n", "").Replace("\r", ""));
        }
    }
}