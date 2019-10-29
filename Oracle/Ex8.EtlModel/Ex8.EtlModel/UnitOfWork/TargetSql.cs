using System;
using System.Collections.Generic;
using System.Text;

namespace Ex8.EtlModel.UnitOfWork
{
    public class TargetSql
    {
        public string SelectDml { get; set; }
        public List<string> SetupTempDml { get; set; } = new List<string>();
        public string UpdateFromTempDml { get; set; }
        public string ClearTempDml { get; set; }
    }
}
