using System;
using System.Collections.Generic;
using System.Text;

namespace XsdToObjectTreeLibrary.Model
{
    public class Node
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string NodePath { get; set; }
        public List<Node> Children { get; set; } = new List<Node>();
        public List<string> Attributes { get; set; } = new List<string>();
        public NodeTypeEnum NodeType { get; set; }
    }
}
