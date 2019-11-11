using Ex8.SqlDml.Reader.Dbms;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Ex8.SqlDml.Integration.Test
{
    public class MySqlReaderTest
    {
        private const string connectionString = "server=localhost;port=3306;user='root';password='';database=ex8db1";

        [Fact]
        public void Can_GetData()
        {
            var target = new MySqlReader();
            var dataTable = target.GetData(connectionString, $"Select * from ex8db1.ATeam");

            int expectedOutput = target.ExecuteScalar<int>(connectionString, $"Select count(*) from ex8db1.ATeam");
            int actualOutput = dataTable.Tables[0].Rows.Count;

            dataTable.Tables.Should().NotBeEmpty();
            dataTable.Tables[0].Rows.Should().NotBeEmpty();
            expectedOutput.Should().Be(actualOutput);
        }
    }
}
