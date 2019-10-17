using Oracle.Copy.Model.DatabaseJobManifest;
using Oracle.Copy.SqlDml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Oracle.Copy
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
