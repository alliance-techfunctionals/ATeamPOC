using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.XPath;

namespace XMLObject
{
    class Program
    {

        static void Main(string[] args)
        {
            #region Comment
            //var xsd = XDocument.Load("new.xsd");
            //var prefix = xsd.Root.GetNamespaceOfPrefix("xs");
            //var vehicle = xsd.Root.Element(prefix + "element");
            //var xelement = xsd.Root;
            //var attributes = xelement.Attributes().ToArray();

            ////var xpath= xelement.XPathSelectElements();

            //var xelements = xelement.Elements();

            //foreach (var eleme in xelements)
            //{
            //    var ee = eleme.Elements();

            //}
            //foreach (var attr in attributes)
            //{
            //    var newAttr = attr;

            //    if (attr.Name.Namespace != null)
            //        newAttr = new XAttribute(attr.Name.LocalName, attr.Value);

            //    xelement.Add(newAttr);



            //};






            #endregion

            Program program = new Program();
            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add("", XmlReader.Create(new StringReader(File.ReadAllText("new.xsd"))));      
            schemas.Compile();
            program.AnalyseSchema(schemas);
            Console.ReadLine();

        }

        public void AnalyseSchema(XmlSchemaSet set)
        {
            XmlSchema customerSchema = null;
            foreach (XmlSchema schema in set.Schemas())
            {
               
                Console.WriteLine("Schema");
                customerSchema = schema;
            }
            Console.WriteLine("Schema Contains " + customerSchema.Elements.Values.Count + " Children");
            foreach (XmlSchemaElement element in customerSchema.Elements.Values)
            {
                space = "   ";
               

                RecursiveElementAnalyser(element,space);
            }

        }


        static string space = "";
        public void RecursiveElementAnalyser(XmlSchemaElement element,string space)
        {
            string elementName = element.Name;
            string dataType = element.SchemaTypeName.Name.ToString();
            Console.WriteLine(space + "Node Type: Element Name:" + elementName + " (Type:" + dataType + ")");

            // Get the complex type of the Customer element.
            XmlSchemaComplexType complexType = element.ElementSchemaType as XmlSchemaComplexType;

            if (complexType != null)
            {
                // If the complex type has any attributes, get an enumerator 
                // and write each attribute name to the console.
                if (complexType.AttributeUses.Count > 0)
                {
                    IDictionaryEnumerator enumerator =
                        complexType.AttributeUses.GetEnumerator();
                    space = space + "   ";
                    while (enumerator.MoveNext())
                    {
                        XmlSchemaAttribute attribute =
                            (XmlSchemaAttribute)enumerator.Value;

                        string attrDataType = attribute.AttributeSchemaType.TypeCode.ToString();

                        string attrName = string.Format(space + "(Attribute:: {0}({1}))", attribute.Name, attrDataType);

                        Console.WriteLine(attrName);
                    }
                }

                // Get the sequence particle of the complex type.
                XmlSchemaSequence sequence = complexType.ContentTypeParticle as XmlSchemaSequence;

                if (sequence != null)
                {
                    space = space + "    ";
                    // Iterate over each XmlSchemaElement in the Items collection.
                    Console.WriteLine(space+"Sequence Contain " + sequence.Items.Count + " Children  ");
                    
                    foreach (var childElement in sequence.Items)
                    {


                        var xmlSchemaElement = childElement as XmlSchemaElement;
                        if (xmlSchemaElement != null)
                        {
                            
                            RecursiveElementAnalyser( xmlSchemaElement, space);
                        }
                        else
                        {
                            // support for XmlSchemaChoise element list
                            var choice = childElement as XmlSchemaChoice;
                            if (choice != null)
                            {
                                foreach (var choiceElement in choice.Items)
                                {
                                    var xmlChoiceSchemaElement = choiceElement as XmlSchemaElement;
                                    if (xmlChoiceSchemaElement != null)
                                    {
                                        RecursiveElementAnalyser(xmlChoiceSchemaElement, space);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            XmlSchemaSimpleType simpleType = element.ElementSchemaType as XmlSchemaSimpleType;
            if (simpleType != null)
            {
                if (simpleType.LinePosition != 0)
                {
                    space = space + "   ";
                    Console.WriteLine(space+"Simple Type");
                    XmlSchemaSimpleTypeRestriction restriction = simpleType.Content as XmlSchemaSimpleTypeRestriction;

                    if (restriction != null)
                    {
                        if (restriction.Facets.Count > 0)
                        {
                            Console.WriteLine(space+"Restriction Contain " + restriction.Facets.Count + " Children  ");
                            
                            foreach (var child in restriction.Facets)
                            {
                                var XmlSchemaFacet = child as XmlSchemaFacet;
                                if (XmlSchemaFacet != null)
                                {
                                    Console.WriteLine(space + child.ToString() + " is " + XmlSchemaFacet.Value + "");

                                }
                            }
                        }
                    }
                }
            }
        }
      
    }
}
