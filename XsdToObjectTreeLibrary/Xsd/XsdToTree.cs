using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using XsdToObjectTreeLibrary.Model;

namespace XsdToObjectTreeLibrary.Xsd
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
            var children = new List<Node>();

            string dataType = element.ElementSchemaType.TypeCode.ToString();
            node.Name = element.Name ?? element.RefName.ToString();
            node.DisplayName = elementName;
            node.NodeDataType = dataType;
            node.NodeType = NodeTypeEnum.Element;
            node.NodePath = node.NodePath + element.Name;

            var complexType = element.ElementSchemaType as XmlSchemaComplexType;
            if (complexType != null)
            {
                if (complexType.AttributeUses.Count > 0)
                {
                    var enumerator = complexType.AttributeUses.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        var attribute = (XmlSchemaAttribute)enumerator.Value;

                        string attrDataType = attribute.AttributeSchemaType.TypeCode.ToString();
                        var childNode = new Node
                        {
                            Name = attribute.QualifiedName.Name,
                            DisplayName = attribute.QualifiedName.Name,
                            NodeDataType = attrDataType,
                            NodeType = NodeTypeEnum.Attribute,
                            NodePath = node.NodePath + "/@" + attribute.QualifiedName.Name
                        };
                        children.Add(childNode);
                    }
                }

                var sequence = complexType.ContentTypeParticle as XmlSchemaSequence;
                if (sequence != null)
                {
                    foreach (var childElement in sequence.Items)
                    {
                        var xmlSchemaElement = childElement as XmlSchemaElement;
                        if (xmlSchemaElement != null)
                        {
                            if (xmlSchemaElement.RefName == null)
                            {
                                RecursiveElementAnalyser(prefix, xmlSchemaElement, ref node);
                            }
                            else if (xmlSchemaElement.RefName != null)
                            {
                                string seqDataType = sequence.GetType().ToString();
                                var childNode = new Node
                                {
                                    Name = xmlSchemaElement.RefName.Name,
                                    DisplayName = xmlSchemaElement.RefName.Name,
                                    NodeDataType = seqDataType,
                                    NodeType = NodeTypeEnum.Element,
                                    NodePath = node.NodePath + "/" + xmlSchemaElement.RefName
                                };

                                RecursiveElementAnalyser(prefix, xmlSchemaElement, ref childNode);
                                children.Add(childNode);
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
            }

            node.Children = children;
        }
    }
}
