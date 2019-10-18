using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ex8.EtlModel
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum JobTypeEnum
    {
        Anonymise = 1,
        Pseudonymise = 2,
        Reconstruct = 3,
        Restrict = 4

    }
}