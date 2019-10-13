using FluentAssertions;
using System;
using System.IO;
using System.Linq;
using System.Xml;
using XsdToObjectTree.UnitTest.Helper;
using XsdToObjectTreeLibrary.Tree.Model;
using XsdToObjectTreeLibrary.Tree.Xml;
using Xunit;

namespace XUnitTestProject.Tree.Xml
{
    public class XmlToTreeTest
    {
        private const string _inputRoot = "TestData\\Tree\\Input\\xml\\";
        private const string _outputRoot = "TestData\\Tree\\Output\\xml\\";

        [Fact]
        public void XmlToTree_BasicExample()
        {
            var xml = GetXml("basic.example.xml");
            var expectedResult = GetExpectedResult("basic.example.json");
            var target = new XmlToTree();
            var result = target.GetTree(xml);
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void XmlToTree_Mt300Example()
        {
            var xml = GetXml("mt300.example.xml");
            var expectedResult = GetExpectedResult("mt300.example..json");
            var target = new XmlToTree();
            var result = target.GetTree(xml);
            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void XmlToTree_TradeExample()
        {
            var xml = GetXml("trade-irs.example.xml");
            var expectedResult = GetExpectedResult("trade-irs.example.json");
            var target = new XmlToTree();
            var result = target.GetTree(xml);
            result.Should().BeEquivalentTo(expectedResult);
        }

        private Node GetExpectedResult(string path)
        {
            var outputPath = Path.Combine(AssemblyHelper.GetCurrentExecutingAssemblyPath(), string.Format("{0}{1}", _outputRoot, path));
            string json = File.ReadAllText(outputPath);
            return json.ParseJson<Node>();
        }

        private string GetXml(string path)
        {
            var inputPath = Path.Combine(AssemblyHelper.GetCurrentExecutingAssemblyPath(), string.Format("{0}{1}", _inputRoot, path));
            string xml = File.ReadAllText(inputPath);
            return xml;
        }
    }
}
