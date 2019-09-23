using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace XMLLibrary
{
    public class XMLLibrary
    {
        public List<ObjectNode> AnalyseSchema(XmlSchemaSet set)
        {
            List<ObjectNode> XmlObjects = new List<ObjectNode>();
            ObjectNode XmlObject = new ObjectNode();
            XmlSchema customerSchema = null;
            foreach (XmlSchema schema in set.Schemas())
            {
                customerSchema = schema;
            }
            XmlObject.Name = "";
            XmlObject.NodeType = "Schema";
            XmlObject.Childrens = customerSchema.Elements.Values.Count;
            XmlObject.Children = new List<ObjectNode>();
            foreach (XmlSchemaElement element in customerSchema.Elements.Values)
            {
                ObjectNode elementdata = new ObjectNode();
                RecursiveElementAnalyser(element, elementdata);
                XmlObject.Children.Add(elementdata);
            }
            XmlObjects.Add(XmlObject);
            return XmlObjects;
        }

        public void RecursiveElementAnalyser(XmlSchemaElement element, ObjectNode data)
        {

            data.Name = element.Name;
            data.DisplayName = element.Name;
            data.NodeType = "Element";
            data.Children = new List<ObjectNode>();
            string elementName = element.Name;
            string dataType = element.SchemaTypeName.Name.ToString();
            XmlSchemaComplexType complexType = element.SchemaType as XmlSchemaComplexType;

            if (complexType != null)
            {
                XmlSchemaContentModel xmlSchemaContent = complexType.ContentModel as XmlSchemaContentModel;
                if (xmlSchemaContent != null)
                {
                    XmlSchemaSimpleContentExtension xmlSimleContent = xmlSchemaContent.Content as XmlSchemaSimpleContentExtension;
                    if (xmlSimleContent.Attributes.Count > 0)
                    {
                        data.Childrens = data.Childrens + xmlSimleContent.Attributes.Count;
                        foreach (XmlSchemaAttribute attribute in xmlSimleContent.Attributes)
                        {
                            ObjectNode elementdata = new ObjectNode();
                            elementdata.Name = attribute.Name;
                            elementdata.DisplayName = attribute.Name;
                            elementdata.NodeType = "Attribute";
                            data.Children.Add(elementdata);
                        }
                    }
                }
                XmlSchemaSequence sequence = complexType.Particle as XmlSchemaSequence;

                if (sequence != null)
                {
                    data.Childrens = data.Childrens + sequence.Items.Count;
                    foreach (var childElement in sequence.Items)
                    {
                        ObjectNode elementdata = new ObjectNode();

                        var xmlSchemaElement = childElement as XmlSchemaElement;
                        if (xmlSchemaElement != null)
                        {
                            RecursiveElementAnalyser(xmlSchemaElement, elementdata);
                            data.Children.Add(elementdata);
                        }
                        else
                        {
                            var choice = childElement as XmlSchemaChoice;
                            if (choice != null)
                            {
                                foreach (var choiceElement in choice.Items)
                                {
                                    var xmlChoiceSchemaElement = choiceElement as XmlSchemaElement;
                                    if (xmlChoiceSchemaElement != null)
                                    {
                                        RecursiveElementAnalyser(xmlChoiceSchemaElement, elementdata);
                                        data.Children.Add(elementdata);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            XmlSchemaSimpleType simpleType = element.SchemaType as XmlSchemaSimpleType;
            if (simpleType != null)
            {
                if (simpleType.LinePosition != 0)
                {
                    XmlSchemaSimpleTypeRestriction restriction = simpleType.Content as XmlSchemaSimpleTypeRestriction;

                    if (restriction != null)
                    {
                        if (restriction.Facets.Count > 0)
                        {
                            foreach (var child in restriction.Facets)
                            {
                                var XmlSchemaFacet = child as XmlSchemaFacet;
                                if (XmlSchemaFacet != null)
                                {

                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
