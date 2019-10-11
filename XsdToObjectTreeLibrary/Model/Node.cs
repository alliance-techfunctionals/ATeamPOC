using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace XsdToObjectTreeLibrary.Model
{
    public class Node
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        //public string NodeDataType { get; set; }
        public string NodePath { get; set; }    
        [JsonConverter(typeof(StringEnumConverter))]
        public NodeTypeEnum NodeType { get; set; }
        public List<Node> Children { get; set; } = new List<Node>();
    }
}
