using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Exate.Rules.WebApi.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SamplePayloadTypeEnum
    {
        Json = 1,
        Xml = 2,
        Xsd = 3,
        Unknown = 4
    }
}
