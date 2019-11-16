using Ex8.SqlDml.Builder.Command;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Xunit;

namespace Ex8.SqlDml.Test.Builder.Command
{
    public class PostgreSqlCommandStringBuilderTest
    {
        [Fact]
        public void Test_Should_Generate_SQL()
        {
            var cmd = new NpgsqlCommand("GetEntity", null);
            cmd.Parameters.Add(new NpgsqlParameter("@foobar", 1));
            cmd.Parameters.Add(new NpgsqlParameter()
            {
                ParameterName = "@outParam",
                Direction = ParameterDirection.Output,
                NpgsqlDbType = NpgsqlDbType.Integer
            });
            cmd.Parameters.Add(new NpgsqlParameter()
            {
                Direction = ParameterDirection.ReturnValue
            });
            cmd.CommandType = CommandType.StoredProcedure;
            var sqlCommandStringBuilder = new PostgreSqlCommandStringBuilder();
            var sql = sqlCommandStringBuilder.GetCommandText(cmd);

            Assert.NotEmpty(sql);
        }

        [Fact]
        public void Test_Should_Generate_Update()
        {
            var cmd = new NpgsqlCommand("update sales.Customer  SET  firstName=@param1 where lastName=@whereparam1", null);
            cmd.Parameters.Add(new NpgsqlParameter("@param1", "Charlotte"));
            cmd.Parameters.Add(new NpgsqlParameter("@whereparam1", "Saunders"));

            cmd.CommandType = CommandType.Text;
            var expectedSQl =
                "DECLARE @param1 TEXT(0)(MAX ) = N'Charlotte';\r\nDECLARE @whereparam1 TEXT(0)(MAX ) = N'Saunders';\r\nupdate sales.Customer  SET  firstName=@param1 where lastName=@whereparam1\r\n;\r\n";
            var sqlCommandStringBuilder = new PostgreSqlCommandStringBuilder();
            var result = sqlCommandStringBuilder.GetCommandText(cmd);

            Assert.NotEmpty(result);
            Assert.Equal(expectedSQl.Replace("\n", "").Replace("\r", ""), result.Replace("\n", "").Replace("\r", ""));
        }

        [Fact]
        public void Test_Should_Generate_InlineUpdate()
        {
            var cmd = new NpgsqlCommand("update sales.Customer SET firstName=@param1 where lastName=@whereparam1", null);
            cmd.Parameters.Add(new NpgsqlParameter("@param1", "Charlotte"));
            cmd.Parameters.Add(new NpgsqlParameter("@whereparam1", "Saunders"));

            cmd.CommandType = CommandType.Text;
            var expectedSQl =
                "update sales.Customer SET firstName=N'Charlotte' where lastName=N'Saunders'";
            var sqlCommandStringBuilder = new PostgreSqlCommandStringBuilder();
            var result = sqlCommandStringBuilder.GetCommandTextInline(cmd);

            Assert.NotEmpty(result);
            Assert.Equal(expectedSQl.Replace("\n", "").Replace("\r", ""), result.Replace("\n", "").Replace("\r", ""));
        }
    }
}
