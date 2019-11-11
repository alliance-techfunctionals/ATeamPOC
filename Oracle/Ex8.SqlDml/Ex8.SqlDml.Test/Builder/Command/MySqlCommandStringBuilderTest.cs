using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using MySql.Data.MySqlClient;
using System.Data;
using Ex8.SqlDml.Builder.Command;

namespace Ex8.SqlDml.Test.Builder.Command
{
    public class MySqlCommandStringBuilderTest
    {

        [Fact]
        public void Test_Should_Generate_SQL()
        {
            var cmd = new MySqlCommand("GetEntity", null);
            cmd.Parameters.Add(new MySqlParameter("@foobar", 1));
            cmd.Parameters.Add(new MySqlParameter()
            {
                ParameterName = "@outParam",
                Direction = ParameterDirection.Output,
                MySqlDbType = MySqlDbType.Int32
            });
            cmd.Parameters.Add(new MySqlParameter()
            {
                Direction = ParameterDirection.ReturnValue
            });
            cmd.CommandType = CommandType.StoredProcedure;
            var sqlCommandStringBuilder = new MySqlCommandStringBuilder();
            var sql = sqlCommandStringBuilder.GetCommandText(cmd);

            Assert.NotEmpty(sql);
        }

        [Fact]
        public void Test_Should_Generate_Update()
        {
            var cmd = new MySqlCommand("update ex8db1.ATeam  SET  first_name=@param1 where last_name=@whereparam1", null);
            cmd.Parameters.Add(new MySqlParameter("@param1", "Charlotte"));
            cmd.Parameters.Add(new MySqlParameter("@whereparam1", "Saunders"));

            cmd.CommandType = CommandType.Text;
            var expectedSQl =
                "DECLARE @param1 VARCHAR(9) = N'Charlotte';\r\nDECLARE @whereparam1 VARCHAR(8) = N'Saunders';\r\nupdate ex8db1.ATeam  SET  first_name=@param1 where last_name=@whereparam1\r\n;\r\n";
            var sqlCommandStringBuilder = new MySqlCommandStringBuilder();
            var result = sqlCommandStringBuilder.GetCommandText(cmd);

            Assert.NotEmpty(result);
            Assert.Equal(expectedSQl.Replace("\n", "").Replace("\r", ""), result.Replace("\n", "").Replace("\r", ""));
        }

        [Fact]
        public void Test_Should_Generate_InlineUpdate()
        {
            var cmd = new MySqlCommand("update ex8db1.ATeam SET first_name=@param1 where last_name=@whereparam1", null);
            cmd.Parameters.Add(new MySqlParameter("@param1", "Charlotte"));
            cmd.Parameters.Add(new MySqlParameter("@whereparam1", "Saunders"));

            cmd.CommandType = CommandType.Text;
            var expectedSQl =
                "update ex8db1.ATeam SET first_name=N'Charlotte' where last_name=N'Saunders'";
            var sqlCommandStringBuilder = new MySqlCommandStringBuilder();
            var result = sqlCommandStringBuilder.GetCommandTextInline(cmd);

            Assert.NotEmpty(result);
            Assert.Equal(expectedSQl.Replace("\n", "").Replace("\r", ""), result.Replace("\n", "").Replace("\r", ""));
        }
    }
}
