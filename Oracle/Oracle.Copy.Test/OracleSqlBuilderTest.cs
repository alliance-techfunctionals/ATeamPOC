using FluentAssertions;
using Oracle.Copy.Model.DatabaseJobManifest;
using Oracle.Copy.Model.UnitOfWork;
using Oracle.Copy.Test.Helper;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Oracle.Copy.Test
{
    public class OracleSqlBuilderTest
    {
        private const string _inputRoot = "TestData\\";
        private const string _outputRoot = "TestData\\";

        [Fact]
        public void Can_BuildSourceSql()
        {
            var input = GetJsonFile<DatabaseJobManifest>(_inputRoot, "database.manifest.xepdb1.json");
            var expectedResult = GetJsonFile<SourceSql>(_outputRoot, "xepdb1.source.sql.json"); //TODO create output file

            var target = new OracleSqlBuilder();
            var result = target.BuildSourceSql(input.manifest.tables.First());

            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void Can_BuildTargetSql()
        {
            var input = GetJsonFile<DatabaseJobManifest>(_inputRoot, "database.manifest.xepdb1.json");
            var expectedResult = GetJsonFile<SourceSql>(_outputRoot, "xepdb1.target.sql.json"); //TODO create output file

            var target = new OracleSqlBuilder();
            var result = target.BuildTargetSql(input.manifest.tables.First());

            result.Should().BeEquivalentTo(expectedResult);
        }

        private T GetJsonFile<T>(string root, string file)
        {
            var outputPath = Path.Combine(AssemblyHelper.GetCurrentExecutingAssemblyPath(), string.Format("{0}{1}", root, file));
            string json = File.ReadAllText(outputPath);
            return json.ParseJson<T>();
        }
    }
}
