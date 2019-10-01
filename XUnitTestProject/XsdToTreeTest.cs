using System;
using System.IO;
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
        public void XsdToTree_Demo2()
        {
            var xss = GetXmlSchema("TestData\\Input\\demo2.xsd");
            var expectedResult = GetExpectedResult("TestData\\Output\\demo2.example.output.json");

            var target = new XsdToTree();
            var result = target.AnalyseSchema(xss);
        }

        [Fact]
        public void XsdToTree_Demo3()
        {
            var xss = GetXmlSchema("TestData\\Input\\demo3.xsd");
            var expectedResult = GetExpectedResult("TestData\\Output\\demo3.example.output.json");
            var target = new XsdToTree();
            var result = target.AnalyseSchema(xss);
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
