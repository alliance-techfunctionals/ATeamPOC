using Oracle.Copy.Model.DatabaseJobManifest;
using Oracle.Copy.SqlDml;
using System;
using System.Collections.Generic;
using System.Text;

namespace Oracle.Copy
{
    public interface ISqlBuilderSelector
    {
        ISqlBuilder GetBuilder(DatabaseTypeEnum databaseType);
    }
}
