using FluentAssertions;
using Oracle.Copy.Model.DatabaseJobManifest;
using Oracle.Copy.Model.UnitOfWork;
using Oracle.Copy.SqlDml;
using Oracle.Copy.TableService;
using Oracle.Copy.Test.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace Oracle.Copy.Test
{
    public class SourceTableServiceTest
    {
        private const string _inputRoot = "TestData\\";
        private const string _outputRoot = "TestData\\";

        [Fact]
        public void Can_SetTableManifestData()
        {
            var input = GetJsonFile<DatabaseJobManifest>(_inputRoot, "database.manifest.xepdb1.json");
            var expectedResult = GetJsonFile<DatabaseJobManifest>(_outputRoot, "xepdb1.table.manifest.json"); //TODO create file

            var target = new SourceTableService(new SqlBuilderSelector(new List<ISqlBuilder> { new OracleSqlBuilder() }));
            target.SetTableManifestData(input);

            input.Should().BeEquivalentTo(expectedResult);
        }

        private T GetJsonFile<T>(string root, string file)
        {
            var outputPath = Path.Combine(AssemblyHelper.GetCurrentExecutingAssemblyPath(), string.Format("{0}{1}", root, file));
            string json = File.ReadAllText(outputPath);
            return json.ParseJson<T>();
        }
    }
}
