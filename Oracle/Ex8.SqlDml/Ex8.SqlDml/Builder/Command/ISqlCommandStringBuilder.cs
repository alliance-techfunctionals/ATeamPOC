using System.Data.Common;
using System.Data.SqlClient;

namespace Ex8.SqlDml.Builder.Command
{
    public interface ISqlCommandStringBuilder
    {
        string GetCommandText(DbCommand sqc);
        string GetCommandTextInline(DbCommand sqc);
    }
}