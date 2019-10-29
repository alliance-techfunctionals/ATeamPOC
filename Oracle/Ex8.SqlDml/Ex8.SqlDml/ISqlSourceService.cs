using Ex8.EtlModel.DatabaseJobManifest;

namespace Ex8.SqlDml
{
    public interface ISqlSourceService
    {
        void SetTableManifestData(DatabaseJobManifest manifestObject);
    }
}