using Exate.Rules.WebApi.DataAccess.Services.ManifestTreeBuilder;
using Exate.Rules.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Exate.Rules.WebApi.DataAccess.Services
{
    public class ManifestTreeService : IManifestTreeService
    {
        private IEnumerable<IManifestTreeBuilder> _builders;

        public ManifestTreeService(IEnumerable<IManifestTreeBuilder> builders)
        {
            _builders = builders;
        }

        public ManifestTreeNode GetManifestTree(string samplePayload, SamplePayloadTypeEnum payloadType)
        {
            var instance = GetConcreteBuilder(payloadType);
            return instance.GetTree(samplePayload);
        }

        internal IManifestTreeBuilder GetConcreteBuilder(SamplePayloadTypeEnum payloadType)
        {
            var item = _builders.SingleOrDefault(i => i.PayloadType == payloadType);
            if (item == null)
                throw new Exception(string.Format("SamplePayloadType:{0} not supported", payloadType));

            return item;
        }
    }
}
