using System;
using System.Collections.Generic;
using System.Text;

namespace Ex8.EtlModel.UnitOfWork
{
    public class CsvLogger
    {
        public DateTime TestDate { get; set; }
        public Int32 NoOfRecords { get; set; }
        public TimeSpan TimeElapsed { get; set; }
    }
}
