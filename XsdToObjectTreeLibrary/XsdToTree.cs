using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using XsdToObjectTreeLibrary.Model;

namespace XsdToObjectTreeLibrary
{
    public class XsdToTree : IXsdToTree
    {
        public Node GetTree(XmlSchemaSet set)
        {
            XmlSchema customerSchema = null;
            foreach (XmlSchema schema in set.Schemas())
            {
                customerSchema = schema;
            }

            var rootNode = new Node();
            foreach (XmlSchemaElement element in customerSchema.Elements.Values)
            {
                rootNode.NodePath = "/";
                RecursiveElementAnalyser("", element, ref rootNode);
            }

            return rootNode;
        }

        protected void RecursiveElementAnalyser(string prefix, XmlSchemaElement element, ref Node node)
        {
            string elementName = prefix + (element.Name ?? element.RefName.ToString());
            var children = new List<string>();
            var attributes = new List<string>();
            var childNodes = new List<Node>();

            string dataType = element.ElementSchemaType.TypeCode.ToString();
            var root = elementName; // + " (" + dataType + ")";
            node.Name = element.Name ?? element.RefName.ToString();
            node.DisplayName = root;
            node.NodeType = NodeTypeEnum.Element;
            node.NodePath = node.NodePath + element.Name;

            XmlSchemaComplexType complexType = element.ElementSchemaType as XmlSchemaComplexType;

            if (complexType != null)
            {
                if (complexType.AttributeUses.Count > 0)
                {
                    var enumerator = complexType.AttributeUses.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        XmlSchemaAttribute attribute = (XmlSchemaAttribute)enumerator.Value;

                        string attrDataType = attribute.AttributeSchemaType.TypeCode.ToString();
                        string attrName = attribute.Name ?? attribute.RefName.ToString();
                        string attrDesc = string.Format(prefix + "(Attr:: {0} ({1}))", attrName, attrDataType);

                        attributes.Add(attrDesc);
                        var childNode = new Node
                        {
                            Name = attribute.QualifiedName.Name,
                            DisplayName = attribute.QualifiedName.Name,
                            NodeType = NodeTypeEnum.Attribute,
                            NodePath = node.NodePath + "/@" + attribute.QualifiedName.Name,
                            Children = new List<Node>()
                        };
                        childNodes.Add(childNode);
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
                                RecursiveElementAnalyser(prefix, xmlSchemaElement, ref node);
                            }
                            else if (xmlSchemaElement.RefName != null)
                            {
                                var child = prefix + "-" + xmlSchemaElement.RefName;

                                var childNode = new Node();
                                childNode.Name = xmlSchemaElement.RefName.Name;
                                childNode.DisplayName = xmlSchemaElement.RefName.Name;
                                childNode.NodeType = NodeTypeEnum.Attribute;
                                childNode.NodePath = node.NodePath + "/" + xmlSchemaElement.RefName;

                                RecursiveElementAnalyser(prefix, xmlSchemaElement, ref childNode);
                                childNodes.Add(childNode);

                                children.Add(xmlSchemaElement.RefName.Name);
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
                                            RecursiveElementAnalyser(prefix, xmlChoiceSchemaElement, ref node);
                                        }
                                    }
                                }
                            }
                    }
                }
            }

            node.Children = childNodes;
        }
    }
}
