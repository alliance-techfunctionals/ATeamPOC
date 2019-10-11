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
            RecurseXmlDocument((XmlNode)document.DocumentElement, ref rootNode, 0);
            return rootNode;
        }
        private static int RecurseXmlDocument(XmlNode root, ref Node node, int countskip)
        {
            var children = new List<Node>();
            int CountSimilarChild = 0;
            foreach (XmlNode getsimilarchild in root.ParentNode.ChildNodes)
            {
                if (getsimilarchild.Name == root.Name)
                {
                    CountSimilarChild++;
                }
            }
            node.Name = root.Name;
            node.DisplayName = root.Name;
            //node.NodeDataType = root.GetType().Name.ToString();
            node.NodeType = NodeTypeEnum.Element;
            if (CountSimilarChild > 1)
            {
                node.NodePath = node.NodePath + "/" + root.Name + "[*]";
            }
            else
            {
                node.NodePath = node.NodePath + "/" + root.Name;
                countskip = 0;
            }
            if (root.Attributes.Count > 0)
            {
                for (int attr = 0; attr < root.Attributes.Count; attr++)
                {
                    if (countskip == 0)
                    {
                        var childNode = new Node
                        {
                            Name = root.Attributes[attr].Name,
                            DisplayName = root.Attributes[attr].Name,
                            //NodeDataType = root.GetType().Name.ToString(),
                            NodeType = NodeTypeEnum.Attribute,
                            NodePath = node.NodePath + "/@" + root.Attributes[attr].Name
                        };
                        children.Add(childNode);
                    }
                }
            }
            if (root is XmlElement)
            {
                XmlNodeList getchildren = root.ChildNodes;
                foreach (XmlNode child in getchildren)
                {
                    if (child.Name != "#text")
                    {
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
                            countskip = RecurseXmlDocument(child, ref childNode, countskip);
                            children.Add(childNode);
                        }
                    }
                }
            }
            if (CountSimilarChild > 1)
                countskip++;
            node.Children = children;
            return countskip;
        }
    }
}
