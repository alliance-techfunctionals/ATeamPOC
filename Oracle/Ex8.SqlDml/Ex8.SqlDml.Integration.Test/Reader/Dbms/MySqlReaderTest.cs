using Ex8.SqlDml.Reader.Dbms;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Ex8.SqlDml.Integration.Test.Reader.Dbms
{
    public class MySqlReaderTest
    {
        private const string connectionString = "server=182.50.133.84;port=3306;user='ex8db1';password='ex8db1@123';database=ex8_db1";

        [Fact(Skip = "Integration Test. Manual execution only for now")]
        public void Can_GetData()
        {
            var target = new MySqlReader();
            var dataTable = target.GetData(connectionString, $"Select * from ex8_db1.ATeam");

            int expectedOutput = target.ExecuteScalar<int>(connectionString, $"Select count(*) from ex8_db1.ATeam");
            int actualOutput = dataTable.Tables[0].Rows.Count;

            dataTable.Tables.Should().NotBeEmpty();
            dataTable.Tables[0].Rows.Should().NotBeEmpty();
            expectedOutput.Should().Be(actualOutput);
        }
    }
}
