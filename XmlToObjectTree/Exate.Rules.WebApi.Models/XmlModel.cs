using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Exate.Rules.WebApi.Models
{
    public class ManifestXMLPayloadRequest
    {
        [Required]
        public string XmlPayload { get; set; }
    }

    public class XPathResponseModel : IEquatable<XPathResponseModel>
    {
        public string NodeName { get; set; }
        public string XPathValue { get; set; }

        public bool Equals(XPathResponseModel other)
        {
            return (XPathValue == other.XPathValue);
        }

        public override int GetHashCode()
        {
            return XPathValue.GetHashCode();
        }
    }
}
