using Ex8.EtlModel.DatabaseJobManifest;
using Ex8.SqlDml.Builder.TextSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ex8.SqlDml.Builder
{
    public class SqlBuilderSelector : ISqlBuilderSelector
    {
        private IEnumerable<ISqlBuilder> _builders;

        public SqlBuilderSelector(IEnumerable<ISqlBuilder> builders)
        {
            _builders = builders;
        }

        public ISqlBuilder GetBuilder(DatabaseTypeEnum databaseType)
        {
            return _builders.Single(b => b.DatabaseType == databaseType);
        }
    }
}
