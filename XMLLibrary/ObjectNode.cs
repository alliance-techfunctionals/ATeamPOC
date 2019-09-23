using System.Collections.Generic;

namespace XMLLibrary
{
    public class ObjectNode
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string NodePath { get; set; }
        public int Childrens { get; set; }
        public string NodeType { get; set; }
        public List<ObjectNode> Children { get; set; }

    }
}
