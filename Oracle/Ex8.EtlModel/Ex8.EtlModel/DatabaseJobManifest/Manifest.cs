﻿namespace Ex8.EtlModel.DatabaseJobManifest
{
    public class Manifest
    {
        public ManifestTypeEnum manifestType { get; set; }

        public string manifestName { get; set; }

        public DatabaseTypeEnum databaseType { get; set; }

        public int jobInstanceId { get; set; }

        public JobTypeEnum jobType { get; set; }

        public string sourceConnectionString { get; set; }

        public string targetConnectionString { get; set; }

        public Table[] tables { get; set; } = { };
    }
}