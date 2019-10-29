using Ex8.SqlDml.Builder.Command;
using System.Data;
using System.Data.SqlClient;
using Xunit;

namespace Ex8.SqlDml.Test.Builder.Command
{
    public class MsSqlCommandStringBuilderTest
    {
        [Fact]
        public void Test_Should_Generate_SQL()
        {
            var cmd = new SqlCommand("GetEntity", null);
            cmd.Parameters.AddWithValue("@foobar", 1);
            cmd.Parameters.Add(new SqlParameter
            {
                ParameterName = "@outParam",
                Direction = ParameterDirection.Output,
                SqlDbType = SqlDbType.Int
            });
            cmd.Parameters.Add(new SqlParameter
            {
                Direction = ParameterDirection.ReturnValue
            });
            cmd.CommandType = CommandType.StoredProcedure;
            var sqlCommandStringBuilder = new MsSqlCommandStringBuilder();
            var sql = sqlCommandStringBuilder.GetCommandText(cmd);

            Assert.NotEmpty(sql);
        }

        [Fact]
        public void Test_Should_Generate_Update()
        {
            var cmd = new SqlCommand(
                "update SalesLT.Address   SET  AddressLine1=@param1 where AddressLine1=@whereparam1", null);
            cmd.Parameters.AddWithValue("@param1", "7000 Victoria Park Avenue");
            cmd.Parameters.AddWithValue("@whereparam1", "7000 Victoria Park Avenue");

            cmd.CommandType = CommandType.Text;
            var expectedSQl =
                "DECLARE @param1 NVARCHAR(MAX ) = N'7000 Victoria Park Avenue';\r\nDECLARE @whereparam1 NVARCHAR(MAX ) = N'7000 Victoria Park Avenue';\r\nupdate SalesLT.Address   SET  AddressLine1=@param1 where AddressLine1=@whereparam1\r\n;\r\n";
            var sqlCommandStringBuilder = new MsSqlCommandStringBuilder();
            var result = sqlCommandStringBuilder.GetCommandText(cmd);

            Assert.NotEmpty(result);
            Assert.Equal(expectedSQl.Replace("\n", "").Replace("\r", ""), result.Replace("\n", "").Replace("\r", ""));
        }

        [Fact]
        public void Test_Should_Generate_InlineUpdate()
        {
            var cmd = new SqlCommand(
                "update SalesLT.Address   SET  AddressLine1=@param1 where AddressLine1=@whereparam1", null);
            cmd.Parameters.AddWithValue("@param1", "7000 Victoria Park Avenue");
            cmd.Parameters.AddWithValue("@whereparam1", "7000 Victoria Park Avenue");

            cmd.CommandType = CommandType.Text;
            var expectedSQl =
                "update SalesLT.Address   SET  AddressLine1=N'7000 Victoria Park Avenue' where AddressLine1=N'7000 Victoria Park Avenue'";
            var sqlCommandStringBuilder = new MsSqlCommandStringBuilder();
            var result = sqlCommandStringBuilder.GetCommandTextInline(cmd);

            Assert.NotEmpty(result);
            Assert.Equal(expectedSQl.Replace("\n", "").Replace("\r", ""), result.Replace("\n", "").Replace("\r", ""));
        }
    }
}