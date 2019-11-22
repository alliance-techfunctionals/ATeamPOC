using Exate.Rules.WebApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace Exate.Rules.WebApi.DataAccess.Services.ManifestTreeBuilder
{
    public class XsdManifestTreeBuilder : IManifestTreeBuilder
    {
        public SamplePayloadTypeEnum PayloadType => SamplePayloadTypeEnum.Xsd;

        public ManifestTreeNode GetTree(string xml)
        {
            var xss = GetXmlSchema(xml);
            return GetTree(xss);
        }

        internal ManifestTreeNode GetTree(XmlSchemaSet set)
        {
            XmlSchema customerSchema = null;
            foreach (XmlSchema schema in set.Schemas())
            {
                customerSchema = schema;
            }
            var rootNode = new ManifestTreeNode();
            foreach (XmlSchemaElement element in customerSchema.Elements.Values)
            {
                rootNode.NodePath = "/";
                if (customerSchema.Namespaces.Count > 0)
                {
                    rootNode.Namespace = GetXsdNamespace(customerSchema.TargetNamespace, customerSchema.Namespaces.ToArray());
                }
                RecursiveElementAnalyser(element, ref rootNode, rootNode.Namespace);
            }

            return rootNode;
        }

        private XmlSchemaSet GetXmlSchema(string xml)
        {
            var reader = XmlReader.Create(new StringReader(xml));
            var xur = new XmlUrlResolver { Credentials = System.Net.CredentialCache.DefaultCredentials };
            var xss = new XmlSchemaSet { XmlResolver = xur };
            xss.Add(null, reader);
            xss.Compile();
            return xss;
        }

        private void RecursiveElementAnalyser(XmlSchemaElement element, ref ManifestTreeNode node, ManifestXmlNamespace xmlNamespace)
        {
            string elementName = element.Name ?? element.RefName.ToString();
            var children = new List<ManifestTreeNode>();
            string nameSpacePrefix = xmlNamespace != null ? xmlNamespace.Prefix : string.Empty;

            //string dataType = element.ElementSchemaType.TypeCode.ToString(); // we are not using this anymore. 

            node.Name = element.Name ?? element.RefName.ToString();
            node.DisplayName = elementName;
            //node.NodeDataType = dataType;
            node.NodeType = XmlNodeTypeEnum.Element;
            var nameSpaceXPath = !string.IsNullOrEmpty(element.Name) ? !string.IsNullOrEmpty(nameSpacePrefix) ? $"{nameSpacePrefix}:" : "" : "";
            node.NodePath = node.NodePath + nameSpaceXPath + element.Name;

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
                        var childNode = new ManifestTreeNode
                        {
                            Name = attribute.QualifiedName.Name,
                            DisplayName = attribute.QualifiedName.Name,
                            //NodeDataType = attrDataType,
                            NodeType = XmlNodeTypeEnum.Attribute,
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
                                RecursiveElementAnalyser(xmlSchemaElement, ref node, xmlNamespace);
                            }
                            else if (xmlSchemaElement.RefName != null)
                            {
                                string seqDataType = sequence.GetType().ToString();
                                nameSpaceXPath = !string.IsNullOrEmpty(nameSpacePrefix) ? $"{nameSpacePrefix}:" : "";
                                var childNode = new ManifestTreeNode
                                {
                                    Name = xmlSchemaElement.RefName.Name,
                                    DisplayName = xmlSchemaElement.RefName.Name,
                                    //NodeDataType = seqDataType,
                                    NodeType = XmlNodeTypeEnum.Element,
                                    NodePath = !string.IsNullOrEmpty(xmlSchemaElement.RefName.Name) ? $"{node.NodePath}/{nameSpaceXPath}{xmlSchemaElement.RefName}" : $"{node.NodePath}/{xmlSchemaElement.RefName}"
                                };

                                RecursiveElementAnalyser(xmlSchemaElement, ref childNode, xmlNamespace);
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
                                            RecursiveElementAnalyser(xmlChoiceSchemaElement, ref node, xmlNamespace);
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

        private static ManifestXmlNamespace GetXsdNamespace(string targetNamespace, XmlQualifiedName[] xsdNamespace)
        {
            var prefix = "";
            for (int i = 0; i < xsdNamespace.Length; i++)
            {
                if ((xsdNamespace[i].Namespace).Equals(targetNamespace))
                {
                    prefix = xsdNamespace[i].Name;
                }
            }

            return targetNamespace == null ? null : new ManifestXmlNamespace
            {
                Prefix = string.IsNullOrEmpty(prefix) ? "ns" : prefix,
                Value = targetNamespace
            };
        }

    }
}
