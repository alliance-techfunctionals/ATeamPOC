using Ex8.EtlModel.DatabaseJobManifest;
using Ex8.Helper.Assembly;
using Ex8.SqlDml.Builder.TextSql;
using Ex8.SqlDml.Reader;
using Ex8.SqlDml.Reader.Dbms;
using Ex8.Helper.Serialization;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using Ex8.SqlDml.Builder;

namespace Ex8.SqlDml.Test
{
    public class SqlSourceServiceTest
    {
        private const string _inputRoot = "TestData\\Input\\";
        private const string _outputRoot = "TestData\\Output\\";

        [Fact]
        public void Can_SetTableManifestData()
        {
            var input = GetJsonFile<DatabaseJobManifest>(_inputRoot, "database.manifest.xepdb1.json");
            var expectedResult = GetJsonFile<DatabaseJobManifest>(_outputRoot, "xepdb1.table.manifest.json");

            var target = CreateTarget();
            target.SetTableManifestData(input);

            input.Should().BeEquivalentTo(expectedResult);
        }

        private T GetJsonFile<T>(string root, string file)
        {
            var outputPath = Path.Combine(AssemblyHelper.GetCurrentExecutingAssemblyPath(), string.Format("{0}{1}", root, file));
            string json = File.ReadAllText(outputPath);
            return json.ParseJson<T>();
        }

        private static SqlSourceService CreateTarget()
        {
            return new SqlSourceService(
                new SqlBuilderSelector(new List<ISqlBuilder> { new OracleSqlBuilder() }),
                new SqlReaderSelector(new List<ISqlReader> { new OracleSqlReader() })
            );

        }
    }
}
