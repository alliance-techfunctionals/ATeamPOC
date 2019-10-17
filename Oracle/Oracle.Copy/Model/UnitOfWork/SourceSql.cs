using System;
using System.Collections.Generic;
using System.Text;

namespace Oracle.Copy.Model.UnitOfWork
{
    public class SourceSql
    {
        public string SelectPkDml { get; set; }
        public string SelectRowCountDml { get; set; }
    }
}
