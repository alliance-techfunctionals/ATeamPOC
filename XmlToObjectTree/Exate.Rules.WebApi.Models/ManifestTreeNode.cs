using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Exate.Rules.WebApi.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum XmlNodeTypeEnum
    {
        Attribute = 1,
        Element = 2
    }

    public class ManifestXmlNamespace
    {
        public string Prefix { get; set; }
        public string Value { get; set; }
    }

    public class ManifestTreeNode
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        //public string NodeDataType { get; set; }
        public ManifestXmlNamespace Namespace { get; set; }
        public string NodePath { get; set; }
        public XmlNodeTypeEnum NodeType { get; set; }
        public List<ManifestTreeNode> Children { get; set; } = new List<ManifestTreeNode>();
    }
}
