using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ex8.EtlModel.DatabaseJobManifest;
using Ex8.EtlModel.UnitOfWork;
using Ex8.Helper.Assembly;
using Ex8.Helper.Serialization;
using Ex8.SqlDml.Builder.TextSql;
using FluentAssertions;
using Xunit;

namespace Ex8.SqlDml.Test.Builder.TextSql
{
    public class MsSqlBuilderTest
    {
        private const string _inputRoot = "TestData/Input/";
        private const string _outputRoot = "TestData/Output/";

        [Fact]
        public void Can_BuildSourceSql()
        {
            var input = GetJsonFile<DatabaseJobManifest>(_inputRoot, "database.job.adventureWorks.json");
            var tables = input.manifest.tables;
            var expectedResultFiles = new List<string> { "adventureWorks.source.address.json", "adventureWorks.source.customer.json" };

            var target = new MsSqlBuilder();

            for (int i = 0; i < tables.Length; i++)
            {
                var result = target.BuildSourceSql(tables[i]);
                var expectedResult = GetJsonFile<SourceSql>(_outputRoot, expectedResultFiles[i]);
                result.Should().BeEquivalentTo(expectedResult);
            }
        }

        [Fact]
        public void Can_BuildTargetSql()
        {
            var input = GetJsonFile<DatabaseJobManifest>(_inputRoot, "database.job.adventureWorks.json");
            var tables = input.manifest.tables;
            var expectedResultFiles = new List<string> { "adventureWorks.target.address.json", "adventureWorks.target.customer.json" };

            var target = new MsSqlBuilder();
            for (int i = 0; i < tables.Length; i++)
            {
                var result = target.BuildTargetSql(tables[i]);
                var expectedResult = GetJsonFile<TargetSql>(_outputRoot, expectedResultFiles[i]);
                result.Should().BeEquivalentTo(expectedResult);
            }
        }

        private T GetJsonFile<T>(string root, string file)
        {
            var outputPath = Path.Combine(AssemblyHelper.GetCurrentExecutingAssemblyPath(), string.Format("{0}{1}", root, file));
            string json = File.ReadAllText(outputPath);
            return json.ParseJson<T>();
        }
    }
}