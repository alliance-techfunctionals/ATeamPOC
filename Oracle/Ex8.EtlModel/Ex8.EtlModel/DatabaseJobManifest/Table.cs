namespace Ex8.EtlModel.DatabaseJobManifest
{
    public class Table
    {
        public string schema_name { get; set; }

        public string table_name { get; set; }

        public Column[] columns { get; set; }

        public string pk_column_name { get; set; }

        public long record_count { get; set; }

        public string temp_name { get; set; }

        public string qualified_table_name { get { return $"{this.schema_name}.{this.table_name}"; } }
    }
}