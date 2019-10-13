using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using XsdToObjectTreeLibrary.Merge.Models;

namespace XsdToObjectTreeLibrary
{
    public class XmlElementInfo : IXmlElementInfo
    {
        public XmlElementNode GetRepeatingElementParent(string uri)
        {
            var tree = GetElementTree(uri);
            return GetRepeatingElementParent(tree);
        }

        internal XmlElementNode GetElementTree(string uri)
        {
            XmlElementNode root = null;
            XmlElementNode previousNode = null;
            var settings = new XmlReaderSettings { Async = false };

            using (XmlReader reader = XmlReader.Create(uri, settings))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            var node = GetNode(previousNode, reader.Name, reader.Depth);
                            //TODO could optimise by returning when more than one element in nodes has count > 1
                            //we only need detail of first repeating node and count of repeating nodes
                            if (node.Depth == 0)
                            {
                                root = node;
                            }

                            previousNode = node;
                            break;
                        default:
                            break;
                    }
                }
            }

            return root;
        }

        internal XmlElementNode GetRepeatingElementParent(XmlElementNode root)
        {
            foreach (var node in root.Children)
            {
                if (node.Count > 1)
                {
                    return root;
                }
                else
                {
                    return GetRepeatingElementParentRecursive(node);
                }
            }

            return null;
        }

        internal XmlElementNode GetRepeatingElementParentRecursive(XmlElementNode parent)
        {
            foreach (var node in parent.Children)
            {
                if (node.Count > 1)
                {
                    return parent;
                }
            }

            return null;
        }

        internal XmlElementNode GetNode(XmlElementNode previousNode, string name, int depth)
        {
            XmlElementNode node = null;

            if (previousNode == null)
            {
                previousNode = CreateNode(name, depth);
                return previousNode;
            }
            else if (previousNode.Depth == depth)
            {
                //sibling node
                node = previousNode.Parent.Children.Find(n => n.Name == name && n.Depth == depth);
                if (node == null)
                {
                    node = CreateNode(name, depth, previousNode.Parent);
                    previousNode.Parent.Children.Add(node);
                }
                else
                {
                    node.Count++;
                }
            }
            else if (previousNode.Depth > depth)
            {
                XmlElementNode parentNode = previousNode.Parent;

                //walk up the tree
                while (parentNode.Depth != depth)
                {
                    parentNode = parentNode.Parent;
                }

                node = parentNode.Parent.Children.Find(n => n.Name == name && n.Depth == depth);
                if (node == null)
                {
                    node = CreateNode(name, depth, parentNode.Parent);
                    parentNode.Parent.Children.Add(node);
                }
                else
                {
                    node.Count++;
                }
            }
            else if (previousNode.Depth < depth)
            {
                node = previousNode.Children.Find(n => n.Name == name && n.Depth == depth);
                if (node == null)
                {
                    //child of previous node
                    node = CreateNode(name, depth, previousNode);
                    previousNode.Children.Add(node);
                }
                else
                {
                    node.Count++;
                }
            }

            return node;
        }

        internal static XmlElementNode CreateNode(string name, int depth, XmlElementNode parent = null, long count = 1)
        {
            return new XmlElementNode { Name = name, Count = count, Parent = parent, Depth = depth };
        }
    }
}
