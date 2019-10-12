using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using XsdToObjectTreeLibrary.Model;

namespace XsdToObjectTreeLibrary.Xml
{
    public class XmlToTree : IXmlToTree
    {
        public Node GetTree(string xml)
        {
            var rootNode = new Node();
            XmlDocument document = new XmlDocument();
            document.Load(new StringReader(xml));
            RecurseXmlDocument((XmlNode)document.DocumentElement, ref rootNode, null);
            return rootNode;
        }
        private static XmlNode RecurseXmlDocument(XmlNode nodeofxml, ref Node node, XmlNode previousnode)
        {
            node.Name = nodeofxml.Name;
            node.DisplayName = nodeofxml.Name;
            //node.NodeDataType = root.GetType().Name.ToString();
            node.NodeType = NodeTypeEnum.Element;


            var children = new List<Node>();
            int CountSimilarChildren = 0, countskip = 0;

            foreach (XmlNode similarchildren in nodeofxml.ParentNode.ChildNodes)
            {
                if (similarchildren.Name == nodeofxml.Name)
                {
                    CountSimilarChildren++;
                }
            }


            if (CountSimilarChildren > 1)
                node.NodePath = node.NodePath + "/" + nodeofxml.Name + "[*]";
            else
                node.NodePath = node.NodePath + "/" + nodeofxml.Name;           

            if (nodeofxml.Attributes.Count > 0)
            {
                for (int attr = 0; attr < nodeofxml.Attributes.Count; attr++)
                {
                    if (countskip == 0)
                    {
                        var childNode = new Node
                        {
                            Name = nodeofxml.Attributes[attr].Name,
                            DisplayName = nodeofxml.Attributes[attr].Name,
                            //NodeDataType = root.GetType().Name.ToString(),
                            NodeType = NodeTypeEnum.Attribute,
                            NodePath = node.NodePath + "/@" + nodeofxml.Attributes[attr].Name
                        };
                        children.Add(childNode);
                    }
                }
            }

            if (nodeofxml is XmlElement)
            {
                XmlNodeList listofchildren = nodeofxml.ChildNodes;
                foreach (XmlNode child in listofchildren)
                {
                    if (child.Name != "#text")
                    {
                        if (previousnode != null)
                        {
                            if (previousnode.Name == child.Name)
                                countskip = 1;
                            else
                                countskip = 0;
                        }
                        else
                        {
                            countskip = 0;
                        }

                        if (countskip == 0)
                        {
                            var childNode = new Node
                            {
                                Name = child.Name,
                                DisplayName = child.Name,
                                //NodeDataType = child.GetType().Name.ToString(),
                                NodeType = NodeTypeEnum.Element,
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
