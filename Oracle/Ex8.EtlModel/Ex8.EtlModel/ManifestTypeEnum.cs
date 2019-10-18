using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ex8.EtlModel
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ManifestTypeEnum
    {
        Database = 1,
        Application = 2,
        Api = 3,
        File = 4,
        Fileshare = 5
    }
}
