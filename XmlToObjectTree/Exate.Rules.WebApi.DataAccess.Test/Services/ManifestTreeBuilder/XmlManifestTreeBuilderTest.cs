using Exate.Rules.WebApi.DataAccess.Services.ManifestTreeBuilder;
using Exate.Rules.WebApi.DataAccess.Test.Helper;
using Exate.Rules.WebApi.Models;
using FluentAssertions;
using System;
using System.IO;
using System.Linq;
using System.Xml;
using Xunit;

namespace Exate.Rules.WebApi.DataAccess.Test.Services.ManifestTreeBuilder
{
    public class XmlManifestTreeBuilderTest
    {
        private const string _inputRoot = "TestData\\Input\\xml\\";
        private const string _outputRoot = "TestData\\Output\\xml\\";

        [Fact]
        public void XmlToTree_BasicExample_NoNamespace()
        {
            var xml = GetXml("basic.example.no.namespace.xml");
            var expectedResult = GetExpectedResult("basic.example.no.namespace.json");
            var target = new XmlManifestTreeBuilder();
            var result = target.GetTree(xml);
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void XmlToTree_BasicExample_DefaultNamespace()
        {
            var xml = GetXml("basic.example.default.namespace.xml");
            var expectedResult = GetExpectedResult("basic.example.default.namespace.json");
            var target = new XmlManifestTreeBuilder();
            var result = target.GetTree(xml);
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void XmlToTree_BasicExample_PrefixNamespace()
        {
            var xml = GetXml("basic.example.prefix.namespace.xml");
            var expectedResult = GetExpectedResult("basic.example.prefix.namespace.json");
            var target = new XmlManifestTreeBuilder();
            var result = target.GetTree(xml);
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void XmlToTree_Mt300Example()
        {
            var xml = GetXml("mt300.example.xml");
            var expectedResult = GetExpectedResult("mt300.example.json");
            var target = new XmlManifestTreeBuilder();
            var result = target.GetTree(xml);
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void XmlToTree_TradeExample()
        {
            var xml = GetXml("trade-irs.example.xml");
            var expectedResult = GetExpectedResult("trade-irs.example.json");
            var target = new XmlManifestTreeBuilder();
            var result = target.GetTree(xml);
            result.Should().BeEquivalentTo(expectedResult);
        }

        private ManifestTreeNode GetExpectedResult(string path)
        {
            var outputPath = Path.Combine(AssemblyHelper.GetCurrentExecutingAssemblyPath(), string.Format("{0}{1}", _outputRoot, path));
            string json = File.ReadAllText(outputPath);
            return JsonSerialization.ParseJson<ManifestTreeNode>(json);
        }

        private string GetXml(string path)
        {
            var inputPath = Path.Combine(AssemblyHelper.GetCurrentExecutingAssemblyPath(), string.Format("{0}{1}", _inputRoot, path));
            string xml = File.ReadAllText(inputPath);
            return xml;
        }
    }
}
