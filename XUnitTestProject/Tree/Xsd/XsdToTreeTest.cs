using FluentAssertions;
using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using XsdToObjectTree.UnitTest.Helper;
using Xunit;
using XsdToObjectTreeLibrary.Tree.Model;
using XsdToObjectTreeLibrary.Tree.Xsd;

namespace XUnitTestProject.Tree.Xsd
{
    public class XsdToTreeTest
    {
        private const string _inputRoot = "TestData\\Tree\\Input\\xsd\\";
        private const string _outputRoot = "TestData\\Tree\\Output\\xsd\\";

        [Fact]
        public void XsdToTree_Demo3()
        {
            var xss = GetXmlSchema("demo3.xsd");
            var expectedResult = GetExpectedResult("demo3.example.json");
            var target = new XsdToTree();
            var result = target.GetTree(xss);
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void XsdToTree_Demo2()
        {
            var xss = GetXmlSchema("demo2.xsd");
            var expectedResult = GetExpectedResult("demo2.example.json");
            var target = new XsdToTree();
            var result = target.GetTree(xss);
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void XsdToTree_Demo1()
        {
            var xss = GetXmlSchema("demo1.xsd");
            var expectedResult = GetExpectedResult("demo1.example.json");
            var target = new XsdToTree();
            var result = target.GetTree(xss);
            result.Should().BeEquivalentTo(expectedResult);
        }

        private Node GetExpectedResult(string path)
        {
            var outputPath = Path.Combine(AssemblyHelper.GetCurrentExecutingAssemblyPath(), string.Format("{0}{1}", _outputRoot, path));
            string json = File.ReadAllText(outputPath);
            return json.ParseJson<Node>();
        }

        private XmlSchemaSet GetXmlSchema(string path)
        {
            var inputPath = Path.Combine(AssemblyHelper.GetCurrentExecutingAssemblyPath(), string.Format("{0}{1}", _inputRoot, path));
            string xml = File.ReadAllText(inputPath);
            var reader = XmlReader.Create(new StringReader(xml));
            var xur = new XmlUrlResolver { Credentials = System.Net.CredentialCache.DefaultCredentials };
            var xss = new XmlSchemaSet { XmlResolver = xur };
            xss.Add(null, reader);
            xss.Compile();
            return xss;
        }
    }
}
