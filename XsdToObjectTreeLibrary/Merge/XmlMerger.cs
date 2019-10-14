using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using XsdToObjectTreeLibrary;
using XsdToObjectTreeLibrary.Merge.Models;

namespace XsdToObjectTreeLibrary.Merge
{
    public class XmlMerger : IXmlMerger
    {
        public void Merge(IEnumerable<string> sourcePaths, string targetPath)
        {
            List<string> sourcePath = sourcePaths.ToList();
            XmlElementNode node = GetRepeatingElementParent(sourcePath[0]);
        }

        public XmlElementNode GetRepeatingElementParent(string uri)
        {
            var tree = GetElementTree(uri);
            return tree;
        }

        internal XmlElementNode GetElementTree(string uri)
        {
            XmlElementNode root = null;
            XmlElementNode previousNode = null;
            string value = null;
            var settings = new XmlReaderSettings { Async = false };

            using (XmlReader reader = XmlReader.Create(uri, settings))
            {
                while (reader.Read())
                {                   
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                        case XmlNodeType.Text:
                            String nodetype = "element";
                          
                             var node = GetNode(previousNode, reader.Name, reader.Depth,value,nodetype);
                            //TODO could optimise by returning when more than one element in nodes has count > 1
                            //we only need detail of first repeating node and count of repeating nodes
                            if (node.Depth == 0)
                            {
                                root = node;
                            }
                            if(previousNode!=null)
                              previousNode.Value = reader.NodeType == XmlNodeType.Text? reader.Value.ToString():null;
                            if (reader.HasAttributes)
                            {

                                for (int index = 0; index < reader.AttributeCount; index++)
                                {
                                    reader.MoveToAttribute(index);
                                    XmlElementNode attribute = new XmlElementNode
                                    {
                                        Name = reader.Name,
                                        Count = 1,
                                        Depth = reader.Depth,
                                        Value = reader.Value,
                                        NodeType = "attribute",
                                        Parent = node
                                    };
                                    node.Children.Add(attribute);
                                }
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
             
        internal XmlElementNode GetNode(XmlElementNode previousNode, string name, int depth,string value,string nodeType)
        {
            XmlElementNode node = null;

            if (previousNode == null)
            {
                previousNode = CreateNode(name, depth,value,nodeType);
                return previousNode;
            }
            else if (previousNode.Depth == depth)
            {
                //sibling node
                //node = previousNode.Parent.Children.Find(n => n.Name == name && n.Depth == depth);
                if (node == null)
                {
                    node = CreateNode(name, depth, value, nodeType, previousNode.Parent);
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

                //node = parentNode.Parent.Children.Find(n => n.Name == name && n.Depth == depth);
                if (node == null)
                {
                    node = CreateNode(name, depth, value, nodeType, parentNode.Parent);
                    parentNode.Parent.Children.Add(node);
                }
                else
                {
                    node = CreateNode(name, depth, value, nodeType, parentNode.Parent);
                    node.Count++;
                }
            }
            else if (previousNode.Depth < depth)
            {
                //node = previousNode.Children.Find(n => n.Name == name && n.Depth == depth);
                if (node == null)
                {
                    //child of previous node
                    node = CreateNode(name, depth, value, nodeType, previousNode);
                    previousNode.Children.Add(node);
                }
                else
                {
                    node.Count++;
                }                           
            }

            return node;
        }

        internal static XmlElementNode CreateNode(string name, int depth, string value, string nodeType, XmlElementNode parent = null, long count = 1)
        {
            return new XmlElementNode { Name = name, Count = count, Parent = parent, Depth = depth,Value = value,NodeType = nodeType };
        }
    }
}
