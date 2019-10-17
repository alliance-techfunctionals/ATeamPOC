using System;
using System.Collections.Generic;
using System.Text;

namespace Oracle.Copy.Model
{
    public class Column
    {
        public string columnName { get; set; }

        public string dataType { get; set; }

        public string maxLength { get; set; }

        public string attributeTypeEnum { get; set; }

        public string attributeName { get; set; }
    }
}
