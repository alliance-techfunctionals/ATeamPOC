using Exate.Rules.WebApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

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
                element.Namespaces = customerSchema.Namespaces;
                if (element.Namespaces.Count > 0)
                {
                    rootNode.Namespace = GetXsdNamespace(customerSchema.Namespaces);
                }
                RecursiveElementAnalyser(element, ref rootNode);
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

        private void RecursiveElementAnalyser(XmlSchemaElement element, ref ManifestTreeNode node)
        {
            string elementName = element.Name ?? element.RefName.ToString();
            var children = new List<ManifestTreeNode>();
            string nameSpacePrefix="";

            if (element.Namespaces.Count > 0)
            {
                nameSpacePrefix = element.Namespaces.ToArray()[0].Name;  // Neha: I am assuming here that we have one namespace in our XSD. we can have more but that has to be dealt then              
            }

            //string dataType = element.ElementSchemaType.TypeCode.ToString(); // we are not using this anymore. 

            node.Name = element.Name ?? element.RefName.ToString();
            node.DisplayName = elementName;
            //node.NodeDataType = dataType;
            node.NodeType = XmlNodeTypeEnum.Element;           
            var nameSpaceXPath = !String.IsNullOrEmpty(element.Name) ? !String.IsNullOrEmpty(nameSpacePrefix) ? $"{nameSpacePrefix}:" : "" : "";
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
                        childElement.Namespaces = element.Namespaces;
                        
                        var xmlSchemaElement = childElement as XmlSchemaElement;
                        if (xmlSchemaElement != null)
                        {
                            if (xmlSchemaElement.RefName == null)
                            {
                                RecursiveElementAnalyser(xmlSchemaElement, ref node);
                            }
                            else if (xmlSchemaElement.RefName != null)
                            {
                                string seqDataType = sequence.GetType().ToString();
                                nameSpaceXPath = !String.IsNullOrEmpty(nameSpacePrefix) ? node.NodePath + "/" + nameSpacePrefix + ':' + xmlSchemaElement.RefName :"";
                                var childNode = new ManifestTreeNode
                                {                                    
                                    Name = xmlSchemaElement.RefName.Name,
                                    DisplayName = xmlSchemaElement.RefName.Name,
                                    //NodeDataType = seqDataType,
                                    NodeType = XmlNodeTypeEnum.Element,
                                    NodePath = xmlSchemaElement.RefName.Name != "" ? nameSpaceXPath : node.NodePath + "/" + xmlSchemaElement.RefName
                                };

                                RecursiveElementAnalyser(xmlSchemaElement, ref childNode);
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
                                            RecursiveElementAnalyser(xmlChoiceSchemaElement, ref node);
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


        private static ManifestXmlNamespace GetXsdNamespace(XmlSerializerNamespaces customerNamespaces)
        {
            XmlQualifiedName[] xmlQualifiedNames = customerNamespaces.ToArray();
            return new ManifestXmlNamespace
            {
                Prefix = xmlQualifiedNames[0].Name,     // we picking up the first namespace only and so put 0! Can there be chances of multiple namespaces ?
                Value = xmlQualifiedNames[0].Namespace  // we picking up the first namespace only and so put 0! assumed that we have just one namespace only ?
            };
        }


    }
}
