using Exate.Rules.WebApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Exate.Rules.WebApi.DataAccess.Services.ManifestTreeBuilder
{
    public class XmlManifestTreeBuilder : IManifestTreeBuilder
    {
        public SamplePayloadTypeEnum PayloadType => SamplePayloadTypeEnum.Xml;

        public ManifestTreeNode GetTree(string xml)
        {
            var rootNode = new ManifestTreeNode();
            var document = new XmlDocument();
            document.Load(new StringReader(xml));
            RecurseXmlDocument(document.DocumentElement, ref rootNode, null);
            return rootNode;
        }

        private static XmlNode RecurseXmlDocument(XmlNode nodeofxml, ref ManifestTreeNode node, XmlNode previousnode)
        {
            node.Name = nodeofxml.Name;
            node.DisplayName = nodeofxml.Name;
            //node.NodeDataType = root.GetType().Name.ToString();
            node.NodeType = XmlNodeTypeEnum.Element;

            var children = new List<ManifestTreeNode>();
            int countSimilarChildren = 0, countskip = 0;

            foreach (XmlNode similarchildren in nodeofxml.ParentNode.ChildNodes)
            {
                if (similarchildren.Name == nodeofxml.Name)
                {
                    countSimilarChildren++;
                }
            }

            var arrayXPath = countSimilarChildren > 1 ? "[*]" : string.Empty;
            node.NodePath = $"{node.NodePath}/{nodeofxml.Name}{arrayXPath}";

            for (int index = 0; index < nodeofxml.Attributes.Count; index++)
            {
                if (countskip == 0)
                {
                    var childNode = new ManifestTreeNode
                    {
                        Name = nodeofxml.Attributes[index].Name,
                        DisplayName = nodeofxml.Attributes[index].Name,
                        //NodeDataType = root.GetType().Name.ToString(),
                        NodeType = XmlNodeTypeEnum.Attribute,
                        NodePath = node.NodePath + "/@" + nodeofxml.Attributes[index].Name
                    };
                    children.Add(childNode);
                }
            }

            if (nodeofxml is XmlElement)
            {
                XmlNodeList listofchildren = nodeofxml.ChildNodes;
                foreach (XmlNode child in listofchildren)
                {
                    if (child.Name != "#text")
                    {
                        countskip = previousnode != null && previousnode.Name == child.Name ? 1 : 0;
                        if (countskip == 0)
                        {
                            var childNode = new ManifestTreeNode
                            {
                                Name = child.Name,
                                DisplayName = child.Name,
                                //NodeDataType = child.GetType().Name.ToString(),
                                NodeType = XmlNodeTypeEnum.Element,
                                NodePath = node.NodePath
                            };

                            previousnode = RecurseXmlDocument(child, ref childNode, previousnode);
                            children.Add(childNode);
                        }
                    }
                }
            }

            node.Children = children;
            return nodeofxml;
        }
    }
}
