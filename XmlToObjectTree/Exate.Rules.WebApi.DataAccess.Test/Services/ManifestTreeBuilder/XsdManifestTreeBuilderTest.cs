using Exate.Rules.WebApi.DataAccess.Services.ManifestTreeBuilder;
using Exate.Rules.WebApi.DataAccess.Test.Helper;
using Exate.Rules.WebApi.Models;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace Exate.Rules.WebApi.DataAccess.Test.Services.ManifestTreeBuilder
{
    public class XsdManifestTreeBuilderTest
    {
        private const string _inputRoot = "TestData\\Input\\xsd\\";
        private const string _outputRoot = "TestData\\Output\\xsd\\";

        [Fact]
        public void XsdToTree_Demo3()
        {
            var xss = GetXmlSchemaText("demo3.xsd");
            var expectedResult = GetExpectedResult("demo3.example.json");
            var target = new XsdManifestTreeBuilder();
            var result = target.GetTree(xss);
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void XsdToTree_Demo2()
        {
            var xss = GetXmlSchemaText("demo2.xsd");
            var expectedResult = GetExpectedResult("demo2.example.json");
            var target = new XsdManifestTreeBuilder();
            var result = target.GetTree(xss);
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void XsdToTree_Demo1()
        {
            var xss = GetXmlSchemaText("demo1.xsd");
            var expectedResult = GetExpectedResult("demo1.example.json");
            var target = new XsdManifestTreeBuilder();
            var result = target.GetTree(xss);
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void XsdToTree_Basic_NoNamespace()
        {
            var xss = GetXmlSchemaText("basic.example.no.namespace.xsd");
            var expectedResult = GetExpectedResult("basic.example.no.namespace.json");
            var target = new XsdManifestTreeBuilder();
            var result = target.GetTree(xss);
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void XsdToTree_Basic_DefaultNamespace()
        {
            var xss = GetXmlSchemaText("basic.example.default.namespace.xsd");
            var expectedResult = GetExpectedResult("basic.example.default.namespace.json");
            var target = new XsdManifestTreeBuilder();
            var result = target.GetTree(xss);
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void XsdToTree_Basic_PrefixNamespace()
        {
            var xss = GetXmlSchemaText("basic.example.prefix.namespace.xsd");
            var expectedResult = GetExpectedResult("basic.example.prefix.namespace.json");
            var target = new XsdManifestTreeBuilder();
            var result = target.GetTree(xss);
            result.Should().BeEquivalentTo(expectedResult);
        }

        private ManifestTreeNode GetExpectedResult(string path)
        {
            var outputPath = Path.Combine(AssemblyHelper.GetCurrentExecutingAssemblyPath(), string.Format("{0}{1}", _outputRoot, path));
            string json = File.ReadAllText(outputPath);
            return JsonSerialization.ParseJson<ManifestTreeNode>(json);
        }

        private string GetXmlSchemaText(string path)
        {
            var inputPath = Path.Combine(AssemblyHelper.GetCurrentExecutingAssemblyPath(), string.Format("{0}{1}", _inputRoot, path));
            string xml = File.ReadAllText(inputPath);
            return xml;
        }
    }
}
