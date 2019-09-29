using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using XsdToObjectTreeLibrary;

namespace XsdToObjectTree
{
    class Program
    {
        static void Main(string[] args)
        {
            string xmlfile = File.ReadAllText("samplexsds/shipping.xsd");
            var xmlreader = XmlReader.Create(new StringReader(xmlfile));

            XmlSchemaSet schemaset = new XmlSchemaSet();
            XmlUrlResolver xur = new XmlUrlResolver();
            xur.Credentials = System.Net.CredentialCache.DefaultCredentials;

            schemaset.XmlResolver = xur;
            schemaset.Add(null, xmlreader);
            schemaset.Compile();

            var xsd2Tree = new XsdToTree();
            var nodes = xsd2Tree.AnalyseSchema(schemaset);
            Console.ReadLine();
        }

        
    }
}
