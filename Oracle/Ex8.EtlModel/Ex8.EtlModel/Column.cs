namespace Ex8.EtlModel
{
    public class Column
    {
        public string columnName { get; set; }

        public string dataType { get; set; }

        public string maxLength { get; set; }

        public string attributeTypeEnum { get; set; }

        public string subjectEntityMappingId { get; set; }

        public string attributeName { get; set; }

        public bool isReconstructable { get; set; }

        public bool isComposite { get; set; }

        public object formatExpression { get; set; }

        public string formatType { get; set; }

        public Composition composition { get; set; }

        public bool isSensitive { get; set; }
    }
}