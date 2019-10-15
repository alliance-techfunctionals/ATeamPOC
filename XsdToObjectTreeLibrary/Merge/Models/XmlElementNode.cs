using System;
using System.Collections.Generic;
using System.Text;

namespace XsdToObjectTreeLibrary.Merge.Models
{
    public class XmlElementNode
    {
        public string Name { get; set; }
        public long Count { get; set; }
        public int Depth { get; set; }      
        public XmlElementNode Parent { get; set; }
        public List<XmlElementNode> Children { get; set; } = new List<XmlElementNode>();
    }
}
