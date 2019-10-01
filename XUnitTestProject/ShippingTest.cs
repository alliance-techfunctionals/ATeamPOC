using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using XsdToObjectTree.UnitTest.Helper;
using XsdToObjectTreeLibrary;
using XsdToObjectTreeLibrary.Model;
using Xunit;

namespace XUnitTestProject
{
    public class ShippingDetailsTest
    {
        [Fact]
        public void CountTest()
        {
            var expectedNodes = GetTestData();
            var path = Path.Combine(AssemblyHelper.GetCurrentExecutingAssemblyPath(), "TestData\\Input\\shipping.xsd");
            string xml = File.ReadAllText(path);
            var reader = XmlReader.Create(new StringReader(xml));
            XmlSchemaSet xss = new XmlSchemaSet();
            XmlUrlResolver xur = new XmlUrlResolver();
            xur.Credentials = System.Net.CredentialCache.DefaultCredentials;
            xss.XmlResolver = xur;
            xss.Add(null, reader);
            xss.Compile();
            var xsd2Tree = new XsdToTree();
            var nodes = xsd2Tree.AnalyseSchema(xss);
            Assert.Equal((double)expectedNodes.Count, (double)nodes.Count, 1);
        }

        [Fact]
        public void TreeTest()
        {
            var expectedNodes = GetTestData();
            var path = Path.Combine(AssemblyHelper.GetCurrentExecutingAssemblyPath(), "TestData\\Input\\shipping.xsd");
            string xml = File.ReadAllText(path);
            var reader = XmlReader.Create(new StringReader(xml));

            XmlSchemaSet xss = new XmlSchemaSet();
            XmlUrlResolver xur = new XmlUrlResolver();
            xur.Credentials = System.Net.CredentialCache.DefaultCredentials;

            xss.XmlResolver = xur;
            xss.Add(null, reader);
            xss.Compile();

            var xsd2Tree = new XsdToTree();
            var nodes = xsd2Tree.AnalyseSchema(xss);
            Assert.Equal(expectedNodes.Count, nodes.Count);
            NodeLoop(expectedNodes, nodes);
        }

        private void NodeLoop(List<Node> expectedNodes, List<Node> nodes)
        {
            for (int i = 0; i < expectedNodes.Count; i++)
            {
                var expectedNode = expectedNodes[i];
                var node = nodes[i];

                Assert.Equal(expectedNode.Name, node.Name);
                Assert.Equal(expectedNode.NodePath, node.NodePath);
                Assert.Equal(expectedNode.NodeType, node.NodeType);
                Assert.Equal(expectedNode.DisplayName, node.DisplayName);

                //if (expectedNode.Attributes != null && node.Attributes != null)
                //{
                //    Assert.Equal(expectedNode.Attributes.Count, node.Attributes.Count);

                //    if (expectedNode.Attributes.Count == node.Attributes.Count)
                //    {
                //        for (int j = 0; j < expectedNode.Attributes.Count; j++)
                //        {
                //            var expectedAttr = expectedNode.Attributes[j];
                //            var nodeAttr = node.Attributes[j];
                //            Assert.Equal(expectedAttr, nodeAttr);
                //        }
                //    }
                //}

                if (expectedNode.Children != null && node.Children != null)
                {
                    Assert.Equal(expectedNode.Children.Count, node.Children.Count);
                    if (expectedNode.Children.Count == node.Children.Count)
                    {
                        NodeLoop(expectedNode.Children, node.Children);
                    }
                }
            }
        }

        private List<Node> GetTestData()
        {
            var nodes = new List<Node>();

            var node1 = new Node();
            node1.DisplayName = "-orderperson (String)";
            node1.Name = "orderperson";
            node1.NodePath = "orderperson";
            node1.NodeType = NodeTypeEnum.Element;
            nodes.Add(node1);

            var node2 = new Node();
            node2.DisplayName = "-name (String)";
            node2.Name = "name";
            node2.NodePath = "name";
            node2.NodeType = NodeTypeEnum.Element;
            nodes.Add(node2);

            var node3 = new Node();
            node3.DisplayName = "-address (String)";
            node3.Name = "address";
            node3.NodePath = "address";
            node3.NodeType = NodeTypeEnum.Element;
            nodes.Add(node3);

            var node4 = new Node();
            node4.DisplayName = "-city (String)";
            node4.Name = "city";
            node4.NodePath = "city";
            node4.NodeType = NodeTypeEnum.Element;
            nodes.Add(node4);

            var node5 = new Node();
            node5.DisplayName = "-country (String)";
            node5.Name = "country";
            node5.NodePath = "country";
            node5.NodeType = NodeTypeEnum.Element;
            nodes.Add(node5);

            var node6 = new Node();
            node6.DisplayName = "-title (String)";
            node6.Name = "title";
            node6.NodePath = "title";
            node6.NodeType = NodeTypeEnum.Element;
            nodes.Add(node6);

            var node7 = new Node();
            node7.DisplayName = "-note (String)";
            node7.Name = "note";
            node7.NodePath = "note";
            node7.NodeType = NodeTypeEnum.Element;
            nodes.Add(node7);

            var node8 = new Node();
            node8.DisplayName = "-quantity (PositiveInteger)";
            node8.Name = "quantity";
            node8.NodePath = "quantity";
            node8.NodeType = NodeTypeEnum.Element;
            nodes.Add(node8);

            var node9 = new Node();
            node9.DisplayName = "-price (Decimal)";
            node9.Name = "price";
            node9.NodePath = "price";
            node9.NodeType = NodeTypeEnum.Element;
            nodes.Add(node9);

            var node10 = new Node();
            node10.DisplayName = "-shipto (None)";
            node10.Name = "shipto";
            node10.NodePath = "shipto";
            node10.NodeType = NodeTypeEnum.Element;

            var node10Children = new List<Node>();

            var node10_1 = new Node();
            node10_1.DisplayName = "name";
            node10_1.Name = "name";
            node10_1.NodePath = "shipto/name";
            node10_1.NodeType = NodeTypeEnum.Attribute;

            var node10_2 = new Node();
            node10_2.DisplayName = "address";
            node10_2.Name = "address";
            node10_2.NodePath = "shipto/address";
            node10_2.NodeType = NodeTypeEnum.Attribute;

            var node10_3 = new Node();
            node10_3.DisplayName = "city";
            node10_3.Name = "city";
            node10_3.NodePath = "shipto/city";
            node10_3.NodeType = NodeTypeEnum.Attribute;

            var node10_4 = new Node();
            node10_4.DisplayName = "country";
            node10_4.Name = "country";
            node10_4.NodePath = "shipto/country";
            node10_4.NodeType = NodeTypeEnum.Attribute;

            node10Children.Add(node10_1);
            node10Children.Add(node10_2);
            node10Children.Add(node10_3);
            node10Children.Add(node10_4);

            node10.Children = node10Children;
            nodes.Add(node10);

            var node11 = new Node();
            node11.DisplayName = "-item (None)";
            node11.Name = "item";
            node11.NodePath = "item";
            node11.NodeType = NodeTypeEnum.Element;

            var node11Children = new List<Node>();

            var node11_1 = new Node();
            node11_1.DisplayName = "title";
            node11_1.Name = "title";
            node11_1.NodePath = "item/title";
            node11_1.NodeType = NodeTypeEnum.Attribute;

            var node11_2 = new Node();
            node11_2.DisplayName = "note";
            node11_2.Name = "note";
            node11_2.NodePath = "item/note";
            node11_2.NodeType = NodeTypeEnum.Attribute;

            var node11_3 = new Node();
            node11_3.DisplayName = "quantity";
            node11_3.Name = "quantity";
            node11_3.NodePath = "item/quantity";
            node11_3.NodeType = NodeTypeEnum.Attribute;

            var node11_4 = new Node();
            node11_4.DisplayName = "price";
            node11_4.Name = "price";
            node11_4.NodePath = "item/price";
            node11_4.NodeType = NodeTypeEnum.Attribute;

            node11Children.Add(node11_1);
            node11Children.Add(node11_2);
            node11Children.Add(node11_3);
            node11Children.Add(node11_4);

            node11.Children = node11Children;
            nodes.Add(node11);

            var node12 = new Node();
            node12.DisplayName = "-shiporder (None)";
            node12.Name = "shiporder";
            node12.NodePath = "shiporder";
            node12.NodeType = NodeTypeEnum.Element;

            var node12Children = new List<Node>();

            var node12_1 = new Node();
            node12_1.DisplayName = "orderperson";
            node12_1.Name = "orderperson";
            node12_1.NodePath = "shiporder/orderperson";
            node12_1.NodeType = NodeTypeEnum.Attribute;

            var node12_2 = new Node();
            node12_2.DisplayName = "shipto";
            node12_2.Name = "shipto";
            node12_2.NodePath = "shiporder/shipto";
            node12_2.NodeType = NodeTypeEnum.Attribute;

            var node12_3 = new Node();
            node12_3.DisplayName = "item";
            node12_3.Name = "item";
            node12_3.NodePath = "shiporder/item";
            node12_3.NodeType = NodeTypeEnum.Attribute;

            node12Children.Add(node12_1);
            node12Children.Add(node12_2);
            node12Children.Add(node12_3);

            node12.Children = node12Children;
            nodes.Add(node12);

            return nodes;
        }
    }
}
