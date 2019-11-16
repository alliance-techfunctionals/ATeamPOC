﻿using Ex8.SqlDml.Reader.Dbms;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Ex8.SqlDml.Integration.Test.Reader.Dbms
{
    public class PostgreSqlReaderTest
    {
        private const string connectionString = "User ID=postgres;Password=India@123;Host=localhost;Port=5432;Database=Ex8db1;";

       
        [Fact]
        public void Can_GetData()
        {
            var target = new PostgreSqlReader();
            var dataTable = target.GetData(connectionString, "SELECT ID, FirstName, LastName, Email, Contact, Age\n from sales.Customer;");

            int expectedOutput = target.ExecuteScalar<int>(connectionString, $"Select count(*) from sales.Customer");
            int actualOutput = dataTable.Tables[0].Rows.Count;

            dataTable.Tables.Should().NotBeEmpty();
            dataTable.Tables[0].Rows.Should().NotBeEmpty();
            expectedOutput.Should().Be(actualOutput);
        }
    }
}
