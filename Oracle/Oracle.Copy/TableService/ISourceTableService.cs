using Oracle.Copy.Model.DatabaseJobManifest;

namespace Oracle.Copy.TableService
{
    public interface ISourceTableService
    {
        void SetTableManifestData(DatabaseJobManifest manifestObject);
    }
}