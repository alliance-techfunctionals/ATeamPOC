using System;

namespace Ex8.EtlModel.ManifestInfo
{
    public class Job
    {
        public int ManifestDetailsJobId { get; set; }
        public DateTime SnapshotDate { get; set; }
        public string AnonymisationKey { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}