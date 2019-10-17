using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Oracle.Copy.Model
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
