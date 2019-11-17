using Exate.Rules.WebApi.Models;

namespace Exate.Rules.WebApi.DataAccess.Services.ManifestTreeBuilder
{
    public interface IManifestTreeBuilder
    {
        SamplePayloadTypeEnum PayloadType { get; }

        ManifestTreeNode GetTree(string xml);
    }
}