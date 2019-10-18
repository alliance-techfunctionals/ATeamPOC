using FluentAssertions;
using Oracle.Copy.Model.DatabaseJobManifest;
using Oracle.Copy.Model.UnitOfWork;
using Oracle.Copy.SqlDml;
using Oracle.Copy.Test.Helper;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Oracle.Copy.Test
{
    public class OracleSqlBuilderTest
    {
        private const string _inputRoot = "TestData\\Input\\";
        private const string _outputRoot = "TestData\\Output\\";

        [Fact]
        public void Can_BuildSourceSql()
        {
            var input = GetJsonFile<DatabaseJobManifest>(_inputRoot, "database.manifest.xepdb1.json");
            var target = new OracleSqlBuilder();
            var result = target.BuildSourceSql(input.manifest.tables.First());

            var expectedResult = GetJsonFile<SourceSql>(_outputRoot, "xepdb1.source.sql.json"); //TODO create output file
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void Can_BuildTargetSql()
        {
            var input = GetJsonFile<DatabaseJobManifest>(_inputRoot, "database.manifest.xepdb1.json");

            var target = new OracleSqlBuilder();
            var result = target.BuildTargetSql(input.manifest.tables.First());





            var expectedResult = GetJsonFile<TargetSql>(_outputRoot, "xepdb1.target.sql.json"); //TODO create output file


            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void Can_testSourceSql()
        {
            var input = GetJsonFile<DatabaseJobManifest>(_inputRoot, "test.json");
            var target = new OracleSqlBuilder();
            var result = target.BuildSourceSql(input.manifest.tables.Last());

            var expectedResult = GetJsonFile<SourceSql>(_outputRoot, "testOutput.source.json");
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void Can_testTargetSql()
        {
            var input = GetJsonFile<DatabaseJobManifest>(_inputRoot, "test.json");

            var target = new OracleSqlBuilder();
            var result = target.BuildTargetSql(input.manifest.tables.Last());





            var expectedResult = GetJsonFile<TargetSql>(_outputRoot, "testOutput.target.json");


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
