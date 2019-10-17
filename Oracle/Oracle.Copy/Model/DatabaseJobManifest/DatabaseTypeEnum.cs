using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Oracle.Copy.Model.DatabaseJobManifest
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DatabaseTypeEnum
    {
        SqlServer = 1,
        Oracle = 2,
        Postgres = 3,
        MsSql = 4
    }

}
