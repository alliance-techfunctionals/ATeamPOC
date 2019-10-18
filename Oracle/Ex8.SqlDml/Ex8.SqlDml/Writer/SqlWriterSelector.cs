using Ex8.EtlModel.DatabaseJobManifest;
using Ex8.SqlDml.Writer.Dbms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ex8.SqlDml.Writer
{
    public class SqlWriterSelector : ISqlWriterSelector
    {
        private IEnumerable<ISqlWriter> _writers;

        public SqlWriterSelector(IEnumerable<ISqlWriter> writers)
        {
            _writers = writers;
        }

        public ISqlWriter GetWriter(DatabaseTypeEnum databaseType)
        {
            return _writers.Single(b => b.DatabaseType == databaseType);
        }
    }
}
