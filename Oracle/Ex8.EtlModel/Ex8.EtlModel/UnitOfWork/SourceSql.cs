using System;
using System.Collections.Generic;
using System.Text;

namespace Ex8.EtlModel.UnitOfWork
{
    public class SourceSql
    {
        public string SelectPkDml { get; set; }
        public string SelectRowCountDml { get; set; }
    }
}
