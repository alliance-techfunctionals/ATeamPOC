namespace Oracle.Copy.Model.DatabaseJobManifest
{
    public class Table
    {
        public string schema_name { get; set; }

        public string table_name { get; set; }

        public Column[] columns { get; set; }

        public string PkColumnName { get; set; }

        public long RecordCount { get; set; }

        public string TempName { get; set; }

        public string QualifiedTableName { get { return $"{schema_name}.{table_name}"; } }
    }
}