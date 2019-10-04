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

            // contains all elements in the xsd file
            var jsonStr = JsonConvert.SerializeObject(result);

            // contains only elements with children to try and match the json output provided
            var jsonOnlyChildren = JsonConvert.SerializeObject(result.Children.Where(n => n.Children != null && n.Children.Count > 0));

            var expectedJsonStr = File.ReadAllText("sampleoutputs/demo3_updated.example.json");
            var expectedJsonChildrenStr = File.ReadAllText("sampleoutputs/demo3_updated_children.example.json");
            //Assert.Equal()

            var cleanJsonStr = Regex.Replace(expectedJsonStr, @"\s+", "");
            var cleanJsonChildrenStr = Regex.Replace(expectedJsonChildrenStr, @"\s+", "");

            Assert.Equal(cleanJsonStr, jsonStr);
            Assert.Equal(cleanJsonChildrenStr, jsonOnlyChildren);
        }
    }
}
