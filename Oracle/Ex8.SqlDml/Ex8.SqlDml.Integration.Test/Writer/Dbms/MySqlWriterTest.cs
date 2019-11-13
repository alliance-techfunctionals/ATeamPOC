using Ex8.EtlModel.DatabaseJobManifest;
using Ex8.EtlModel.UnitOfWork;
using Ex8.Helper.Assembly;
using Ex8.Helper.Serialization;
using Ex8.SqlDml.Reader.Dbms;
using Ex8.SqlDml.Writer.Dbms;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
//using System.Linq;
using System.Text;
using Xunit;

namespace Ex8.SqlDml.Integration.Test.Writer.Dbms
{
    public class MySqlWriterTest
    {
        private const string _inputRoot = "TestData\\Input\\";
        private const string connectionString = "server=182.50.133.84;port=3306;user='ex8db1';password='ex8db1@123';database=ex8_db1;AllowLoadLocalInfile='true'";

        [Fact]
        public void Can_BulkCopy()
        {
            var data = CreateTable();
            
            var manifestObject = GetJsonFile<DatabaseJobManifest>(_inputRoot, "database.manifest.mysql.json");
            var inputSqlQueries = GetJsonFile<TargetSql>(_inputRoot, "mysql.target.ateam.json");

            MySqlWriter target = new MySqlWriter();
                      
            target.ExecuteSqlText(connectionString, inputSqlQueries.SetupTempDml);  //Temp Table Created at this stage {ex8_temp_ATeam}

            var outputNOfRecords = target.BulkCopy(connectionString, manifestObject.manifest.tables[0], data); // copy records into temp. table {ex8_temp_ATeam}
            data.Rows.Count.Should().Be(outputNOfRecords);
        }


        [Fact]
        public void CanUploadTable()
        {
            var manifestObject = GetJsonFile<DatabaseJobManifest>(_inputRoot, "database.manifest.mysql.json");
            var sql = GetJsonFile<TargetSql>(_inputRoot, "mysql.target.ateam.json");
            var data = CreateTable();

            var target = new MySqlWriter();
            target.UploadTable(connectionString,
                sql.SetupTempDml,
                manifestObject.manifest.tables[0],
                data,
                new List<string> { sql.UpdateFromTempDml, sql.ClearTempDml });

            var reader = new MySqlReader();

            // Only get the records what you have Updated in database on the basis of Ids {Primary Keys}
            var result = reader.GetData(connectionString, $"select * from ex8_db1.ATeam where Id in ({string.Join(",",data.AsEnumerable().Select(r => r.Field<dynamic>("Id")))})"); 
            result.Tables.Should().NotBeEmpty();
            result.Tables[0].Rows.Should().NotBeEmpty();
            result.Tables[0].Should().Equals(data);
        }


        private T GetJsonFile<T>(string root, string file)
        {
            var outputPath = Path.Combine(AssemblyHelper.GetCurrentExecutingAssemblyPath(), string.Format("{0}{1}", root, file));
            string json = File.ReadAllText(outputPath);
            return json.ParseJson<T>();
        }

        private DataTable CreateTable()
        {
            var table = new DataTable();
            table.Columns.Add("ID", typeof(decimal));
            table.Columns.Add("FIRST_NAME", typeof(string));
            table.Columns.Add("LAST_NAME", typeof(string));

            table.Rows.Add(1, "Daniel", "Saunders");
            table.Rows.Add(2, "Sonal", "Rattan");
            table.Rows.Add(3, "Peter", "Lancos");
            table.Rows.Add(4, "Suraj", "Nittoor");
            table.Rows.Add(5, "Anuj", "Badjatya");
            table.Rows.Add(6, "Neha", "Verma");
            table.Rows.Add(7, "Abhay", "Kumar");
            table.Rows.Add(8, "Nupur", "Garg");
            return table;
        }

    }
}
