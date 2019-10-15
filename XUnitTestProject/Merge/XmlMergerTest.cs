using FluentAssertions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using XsdToObjectTree.UnitTest.Helper;
using XsdToObjectTreeLibrary;
using XsdToObjectTreeLibrary.Merge;
using Xunit;

namespace XUnitTestProject.Merge
{
    public class XmlMergerTest
    {
        private const string _inputRoot = "TestData\\Merge\\Input\\";
        private const string _outputRoot = "TestData\\Merge\\Output\\";

        [Fact]
        public void Merge_SimpleExample()
        {
            var inputPaths = GetAbsolutePaths(_inputRoot, new List<string> { "simple.input.part1.xml", "simple.input.part2.xml", "simple.input.part3.xml", "simple.input.part4.xml" });
            var targetPath = GetAbsolutePaths(_outputRoot, new List<string> { "actual.simple.output.xml" }).Single();

            var target = new XmlMerger();
            target.Merge(inputPaths, targetPath);
            var strLibOuput = GetXmlBody(targetPath);


            var expectedResult = GetExpectedResult("expected.simple.output.xml");
            strLibOuput.Should().BeEquivalentTo(expectedResult); 
            // it should be working now. test case is running and I have checked that. Please still verify
            // just changed the variable name so that we know that we checking the library output with expected results
        }

        private string GetExpectedResult(string path)
        {
            var inputPath = Path.Combine(AssemblyHelper.GetCurrentExecutingAssemblyPath(), string.Format("{0}{1}", _outputRoot, path));
            return GetXmlBody(inputPath);
        }

        private string GetXmlBody(string inputPath)
        {
            string xml = File.ReadAllText(inputPath);
            return xml;
        }

        private IEnumerable<string> GetAbsolutePaths(string relativePath, List<string> filenames)
        {
            return filenames.Select(p => Path.Combine(AssemblyHelper.GetCurrentExecutingAssemblyPath(), string.Format("{0}{1}", relativePath, p)));
        }
    }
}
