using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using XsdToObjectTreeLibrary.Model;

namespace XsdToObjectTreeLibrary
{
    public class XsdToTree
    {
        public List<Node> AnalyseSchema(XmlSchemaSet set)
        {
            var nodes = new List<Node>();
            XmlSchema customerSchema = null;
            foreach (XmlSchema schema in set.Schemas())
            {
                customerSchema = schema;
            }

            foreach (XmlSchemaElement element in customerSchema.Elements.Values)
            {
                RecursiveElementAnalyser("-", element, ref nodes);
                Console.WriteLine("\r\n");
            }
            return nodes;
        }

        protected void RecursiveElementAnalyser(string prefix, XmlSchemaElement element, ref List<Node> nodes)
        {
            string elementName = prefix + element.Name;
            var children = new List<string>();
            var attributes = new List<string>();
            var childNodes = new List<Node>();
            string dataType = element.ElementSchemaType.TypeCode.ToString();

            var root = elementName + " (" + dataType + ")";
            Console.WriteLine(root);
            var node = new Node();
            node.Name = element.Name;
            node.DisplayName = root;
            node.NodeType = NodeTypeEnum.Element;
            node.NodePath = element.Name;

            XmlSchemaComplexType complexType = element.ElementSchemaType as XmlSchemaComplexType;
            if (complexType != null)
            {
                if (complexType.AttributeUses.Count > 0)
                {
                    IDictionaryEnumerator enumerator = complexType.AttributeUses.GetEnumerator();

                    while (enumerator.MoveNext())
                    {
                        XmlSchemaAttribute attribute = (XmlSchemaAttribute)enumerator.Value;
                        string attrDataType = attribute.AttributeSchemaType.TypeCode.ToString();
                        string attrName = attribute.Name ?? attribute.RefName.ToString();
                        string attrDesc = string.Format(prefix + "(Attr:: {0} ({1}))", attrName, attrDataType);
                        attributes.Add(attrDesc);
                        Console.WriteLine(attrDesc);
                    }
                }

                XmlSchemaSequence sequence = complexType.ContentTypeParticle as XmlSchemaSequence;

                if (sequence != null)
                {
                    foreach (var childElement in sequence.Items)
                    {
                        var xmlSchemaElement = childElement as XmlSchemaElement;
                        if (xmlSchemaElement != null)
                            if (xmlSchemaElement != null && xmlSchemaElement.RefName == null)
                            {
                                RecursiveElementAnalyser(prefix, xmlSchemaElement, ref nodes);
                            }
                            else if (xmlSchemaElement.RefName != null)
                            {
                                var child = prefix + "-" + xmlSchemaElement.RefName;
                                var childNode = new Node();
                                childNode.Name = xmlSchemaElement.RefName.Name;
                                childNode.DisplayName = xmlSchemaElement.RefName.Name;
                                childNode.NodeType = NodeTypeEnum.Attribute;
                                childNode.NodePath = element.Name + "/" + xmlSchemaElement.RefName;
                                childNodes.Add(childNode);

                                children.Add(xmlSchemaElement.RefName.Name);
                                Console.WriteLine(child);
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
                                            RecursiveElementAnalyser(prefix, xmlChoiceSchemaElement, ref nodes);
                                        }
                                    }
                                }
                            }
                    }
                }
            }

            node.Attributes = attributes;
            node.Children = childNodes;
            nodes.Add(node);

            //nodes.Add(root.Replace("-", ""), children);
        }
    }
}
