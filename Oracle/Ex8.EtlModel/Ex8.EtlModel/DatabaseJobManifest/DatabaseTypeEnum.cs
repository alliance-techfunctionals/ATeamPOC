using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ex8.EtlModel.DatabaseJobManifest
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DatabaseTypeEnum
    {
        SqlServer = 1,
        Oracle = 2,
        Postgres = 3,
        MySql = 4
    }

}
