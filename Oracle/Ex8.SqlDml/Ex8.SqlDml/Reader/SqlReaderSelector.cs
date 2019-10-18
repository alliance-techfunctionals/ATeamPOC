using Ex8.EtlModel.DatabaseJobManifest;
using Ex8.SqlDml.Builder.TextSql;
using Ex8.SqlDml.Reader.Dbms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ex8.SqlDml.Reader
{
    public class SqlReaderSelector : ISqlReaderSelector
    {
        private IEnumerable<ISqlReader> _readers;

        public SqlReaderSelector(IEnumerable<ISqlReader> builders)
        {
            _readers = builders;
        }

        public ISqlReader GetReader(DatabaseTypeEnum databaseType)
        {
            return _readers.Single(b => b.DatabaseType == databaseType);
        }
    }
}
