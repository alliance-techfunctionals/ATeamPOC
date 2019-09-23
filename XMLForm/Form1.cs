using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;
using XMLLibrary;

namespace XMLForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add("", XmlReader.Create(new StringReader(File.ReadAllText("new.xsd"))));
            XMLLibrary.XMLLibrary xMLLibrary = new XMLLibrary.XMLLibrary();

            List<ObjectNode> elementName = xMLLibrary.AnalyseSchema(schemas);

            foreach(var element in elementName)
            {
               
            }



        }
    }
}
