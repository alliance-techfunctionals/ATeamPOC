using FluentAssertions;
using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using XsdToObjectTree.UnitTest.Helper;
using XsdToObjectTreeLibrary;
using XsdToObjectTreeLibrary.Model;
using Xunit;

namespace XUnitTestProject
{
    public class XsdToTreeTest
    {
        [Fact]
        public void XsdToTree_Demo3()
        {
            var xss = GetXmlSchema("TestData\\Input\\demo3.xsd");
            var expectedResult = GetExpectedResult("TestData\\Output\\demo3.example.json");
            var target = new XsdToTree();
            var result = target.GetTree(xss);
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void XsdToTree_Demo2()
        {
            var xss = GetXmlSchema("TestData\\Input\\demo2.xsd");
            var expectedResult = GetExpectedResult("TestData\\Output\\demo2.example.json");

            var target = new XsdToTree();
            var result = target.GetTree(xss);
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void XsdToTree_Demo1()
        {
            var xss = GetXmlSchema("TestData\\Input\\demo1.xsd");
            var expectedResult = GetExpectedResult("TestData\\Output\\demo2.example.json");

            var target = new XsdToTree();
            var result = target.GetTree(xss);
            result.Should().BeEquivalentTo(expectedResult);
        }

        private Node GetExpectedResult(string path)
        {
            var outputPath = Path.Combine(AssemblyHelper.GetCurrentExecutingAssemblyPath(), path);
            string json = File.ReadAllText(outputPath);
            return JsonSerialization.ParseJson<Node>(json);
        }

        private XmlSchemaSet GetXmlSchema(string path)
        {
            var inputPath = Path.Combine(AssemblyHelper.GetCurrentExecutingAssemblyPath(), path);
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
