using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using XsdToObjectTree.UnitTest.Helper;
using XsdToObjectTreeLibrary;
using Xunit;

namespace XUnitTestProject
{
    public class XsdToTreeTest
    {
        [Fact]
        public void XsdToTree_Demo3()
        {
            var path = Path.Combine(AssemblyHelper.GetCurrentExecutingAssemblyPath(), "samplexsds\\demo3.xsd");
            string xml = File.ReadAllText(path);
            var reader = XmlReader.Create(new StringReader(xml));

            var xur = new XmlUrlResolver { Credentials = System.Net.CredentialCache.DefaultCredentials };
            var xss = new XmlSchemaSet { XmlResolver = xur };
            xss.Add(null, reader);
            xss.Compile();

            var target = new XsdToTree();
            var result = target.AnalyseSchema(xss);

            var jsonStrLibOutput = JsonConvert.SerializeObject(result);

            var expectedjsonfile = Path.Combine(AssemblyHelper.GetCurrentExecutingAssemblyPath(), "sampleoutputs\\demo3.example.json");
            var expectedJsonStr = File.ReadAllText(expectedjsonfile);
            expectedJsonStr = Regex.Replace(expectedJsonStr, @"\s+", "");
            Assert.Equal(expectedJsonStr, jsonStrLibOutput);
        }
    }
}
