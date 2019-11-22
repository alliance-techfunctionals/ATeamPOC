using Ex8.SqlDml.Reader.Dbms;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Ex8.SqlDml.Integration.Test.Reader.Dbms
{
    public class MsSqlReaderTest
    {
        private const string connectionString = "Data Source=winserver1.vm.exatebot.com; Initial Catalog=AdventureWorksLT2016; User ID=ex8ExecuteUser; Password=lbv9hFlO9s1j;";

        [Fact(Skip = "Integration Test. Manual execution only for now")]
        public void Can_GetData()
        {
            var target = new MsSqlReader();
            var dataTable = target.GetData(connectionString, "Select * from SalesLT.CustomerATeam  ");

            dataTable.Tables.Should().NotBeEmpty();
            dataTable.Tables[0].Rows.Should().NotBeEmpty();
        }
    }
}
