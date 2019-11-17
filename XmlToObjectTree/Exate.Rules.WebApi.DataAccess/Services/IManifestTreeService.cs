using Exate.Rules.WebApi.Models;

namespace Exate.Rules.WebApi.DataAccess.Services
{
    public interface IManifestTreeService
    {
        ManifestTreeNode GetManifestTree(string samplePayload, SamplePayloadTypeEnum payloadType);
    }
}