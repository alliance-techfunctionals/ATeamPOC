﻿using System;
using System.Data;
using Npgsql;
using NpgsqlTypes;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using System.Data.Common;

namespace Ex8.SqlDml.Builder.Command
{
    public class PostgreSqlCommandStringBuilder : ISqlCommandStringBuilder
    {
        private const string DatetimeFormatRoundtrip = "o";
        public string GetCommandText(DbCommand command)
        {
            var sqc = command as NpgsqlCommand;
            var sbCommandText = new StringBuilder();

            for (var i = 0; i < sqc.Parameters.Count; i++)
            {
                LogParameterToSqlBatch(sqc.Parameters[i], sbCommandText);
            }

            if (sqc.CommandType == CommandType.StoredProcedure)
            {
                sbCommandText.Append("EXEC ");

                var hasReturnValue = false;
                for (var i = 0; i < sqc.Parameters.Count; i++)
                {
                    if (sqc.Parameters[i].Direction == ParameterDirection.ReturnValue)
                    {
                        hasReturnValue = true;
                    }
                }

                if (hasReturnValue)
                {
                    sbCommandText.Append("@returnValue = ");
                }

                sbCommandText.Append(sqc.CommandText);

                var hasPrev = false;
                for (var i = 0; i < sqc.Parameters.Count; i++)
                {
                    var cParam = sqc.Parameters[i];
                    if (cParam.Direction != ParameterDirection.ReturnValue)
                    {
                        if (hasPrev)
                        {
                            sbCommandText.Append(", ");
                        }

                        sbCommandText.Append(cParam.ParameterName);
                        sbCommandText.Append(" = ");
                        sbCommandText.Append(cParam.ParameterName);

                        if (cParam.Direction.HasFlag(ParameterDirection.Output))
                        {
                            sbCommandText.Append(" OUTPUT");
                        }

                        hasPrev = true;
                    }
                }
            }
            else
            {
                sbCommandText.AppendLine(sqc.CommandText);
            }

            for (var i = 0; i < sqc.Parameters.Count; i++)
            {
                var cParam = sqc.Parameters[i];

                if (cParam.Direction == ParameterDirection.ReturnValue)
                {
                    sbCommandText.Append(", @returnValue as ReturnValue");
                }
                else if (cParam.Direction.HasFlag(ParameterDirection.Output))
                {
                    sbCommandText.Append(", ");
                    sbCommandText.Append(cParam.ParameterName);
                    sbCommandText.Append(" as [");
                    sbCommandText.Append(cParam.ParameterName);
                    sbCommandText.Append(']');
                }
            }

            sbCommandText.AppendLine(";");

            return sbCommandText.ToString();
        }

        public string GetCommandTextInline(DbCommand command)
        {
            var sqlCommand = command as NpgsqlCommand;
            for (var i = 0; i < sqlCommand.Parameters.Count; i++)
            {
                var returnText = ParameterValue(sqlCommand.Parameters[i].Value);

                sqlCommand.CommandText =
                    sqlCommand.CommandText.Replace(sqlCommand.Parameters[i].ParameterName, returnText);
            }

            return sqlCommand.CommandText;
        }

        private string ParameterValue(object value)
        {
            var sbCommandText = new StringBuilder();
            try
            {
                if (value == null)
                {
                    return "NULL";
                }

                value = UnboxNullable(value);

                if (value is string
                    || value is char
                    || value is char[]
                    || value is XElement
                    || value is XDocument)
                {
                    sbCommandText.Append("N'");
                    sbCommandText.Append(value.ToString().Replace("'", "''"));
                    sbCommandText.Append('\'');
                }
                else if (value is bool)
                {
                    // True -> 1, False -> 0
                    sbCommandText.Append(Convert.ToInt32(value));
                }
                else if (value is sbyte
                         || value is byte
                         || value is short
                         || value is ushort
                         || value is int
                         || value is uint
                         || value is long
                         || value is ulong
                         || value is float
                         || value is double
                         || value is decimal)
                {
                    sbCommandText.Append(value);
                }
                else if (value is DateTime)
                {
                    sbCommandText.Append("CAST('");
                    sbCommandText.Append(((DateTime)value).ToString(DatetimeFormatRoundtrip));
                    sbCommandText.Append("' as datetime2)");
                }
                else if (value is DateTimeOffset)
                {
                    sbCommandText.Append('\'');
                    sbCommandText.Append(((DateTimeOffset)value).ToString(DatetimeFormatRoundtrip));
                    sbCommandText.Append('\'');
                }
                else if (value is Guid)
                {
                    sbCommandText.Append('\'');
                    sbCommandText.Append(((Guid)value).ToString());
                    sbCommandText.Append('\'');
                }
                else if (value is byte[])
                {
                    var data = (byte[])value;
                    if (data.Length == 0)
                    {
                        sbCommandText.Append("NULL");
                    }
                    else
                    {
                        sbCommandText.Append("0x");
                        for (var i = 0; i < data.Length; i++)
                        {
                            sbCommandText.Append(data[i].ToString("h2"));
                        }
                    }
                }
                else
                {
                    sbCommandText.Append("/* UNKNOWN DATATYPE: ");
                    sbCommandText.Append(value.GetType());
                    sbCommandText.Append(" *" + "/ N'");
                    sbCommandText.Append(value);
                    sbCommandText.Append('\'');
                }
            }

            catch (Exception ex)
            {
                sbCommandText.AppendLine("/* Exception occurred while converting parameter: ");
                sbCommandText.AppendLine(ex.ToString());
                sbCommandText.AppendLine("*/");
            }

            return sbCommandText.ToString();
        }

        private void LogParameterToSqlBatch(NpgsqlParameter param, StringBuilder sbCommandText)
        {
            sbCommandText.Append("DECLARE ");
            if (param.Direction == ParameterDirection.ReturnValue)
            {
                sbCommandText.AppendLine("@returnValue INT;");
            }
            else
            {
                sbCommandText.Append(param.ParameterName);

                sbCommandText.Append(' ');
                if (param.NpgsqlDbType != NpgsqlDbType.Bytea)
                {
                    LogParameterType(param, sbCommandText);
                    sbCommandText.Append(" = ");
                    LogQuotedParameterValue(param.Value, sbCommandText);

                    sbCommandText.AppendLine(";");
                }
                else
                {
                    LogStructuredParameter(param, sbCommandText);
                }
            }
        }

        private void LogStructuredParameter(NpgsqlParameter param, StringBuilder sbCommandText)
        {
            sbCommandText.AppendLine(" {List Type};");
            var dataTable = (DataTable)param.Value;

            for (var rowNo = 0; rowNo < dataTable.Rows.Count; rowNo++)
            {
                sbCommandText.Append("INSERT INTO ");
                sbCommandText.Append(param.ParameterName);
                sbCommandText.Append(" VALUES (");

                var hasPrev = false;
                for (var colNo = 0; colNo < dataTable.Columns.Count; colNo++)
                {
                    if (hasPrev)
                    {
                        sbCommandText.Append(", ");
                    }

                    LogQuotedParameterValue(dataTable.Rows[rowNo].ItemArray[colNo], sbCommandText);
                    hasPrev = true;
                }

                sbCommandText.AppendLine(");");
            }
        }

        private void LogQuotedParameterValue(object value, StringBuilder sbCommandText)
        {
            try
            {
                if (value == null)
                {
                    sbCommandText.Append("NULL");
                }
                else
                {
                    value = UnboxNullable(value);

                    if (value is string
                        || value is char
                        || value is char[]
                        || value is XElement
                        || value is XDocument)
                    {
                        sbCommandText.Append("N'");
                        sbCommandText.Append(value.ToString().Replace("'", "''"));
                        sbCommandText.Append('\'');
                    }
                    else if (value is bool)
                    {
                        // True -> 1, False -> 0
                        sbCommandText.Append(Convert.ToInt32(value));
                    }
                    else if (value is sbyte
                             || value is byte
                             || value is short
                             || value is ushort
                             || value is int
                             || value is uint
                             || value is long
                             || value is ulong
                             || value is float
                             || value is double
                             || value is decimal)
                    {
                        sbCommandText.Append(value);
                    }
                    else if (value is DateTime)
                    {
                        sbCommandText.Append("CAST('");
                        sbCommandText.Append(((DateTime)value).ToString(DatetimeFormatRoundtrip));
                        sbCommandText.Append("' as datetime2)");
                    }
                    else if (value is DateTimeOffset)
                    {
                        sbCommandText.Append('\'');
                        sbCommandText.Append(((DateTimeOffset)value).ToString(DatetimeFormatRoundtrip));
                        sbCommandText.Append('\'');
                    }
                    else if (value is Guid)
                    {
                        sbCommandText.Append('\'');
                        sbCommandText.Append(((Guid)value).ToString());
                        sbCommandText.Append('\'');
                    }
                    else if (value is byte[])
                    {
                        var data = (byte[])value;
                        if (data.Length == 0)
                        {
                            sbCommandText.Append("NULL");
                        }
                        else
                        {
                            sbCommandText.Append("0x");
                            for (var i = 0; i < data.Length; i++)
                            {
                                sbCommandText.Append(data[i].ToString("h2"));
                            }
                        }
                    }
                    else
                    {
                        sbCommandText.Append("/* UNKNOWN DATATYPE: ");
                        sbCommandText.Append(value.GetType());
                        sbCommandText.Append(" *" + "/ N'");
                        sbCommandText.Append(value);
                        sbCommandText.Append('\'');
                    }
                }
            }

            catch (Exception ex)
            {
                sbCommandText.AppendLine("/* Exception occurred while converting parameter: ");
                sbCommandText.AppendLine(ex.ToString());
                sbCommandText.AppendLine("*/");
            }
        }

        private object UnboxNullable(object value)
        {
            var typeOriginal = value.GetType();
            if (typeOriginal.IsGenericType
                && typeOriginal.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return typeOriginal.InvokeMember("GetValueOrDefault",
                    BindingFlags.Public |
                    BindingFlags.Instance |
                    BindingFlags.InvokeMethod,
                    null, value, null);
            }

            return value;
        }

        private void LogParameterType(NpgsqlParameter param, StringBuilder sbCommandText)
        {
            switch (param.NpgsqlDbType)
            {
                // variable length
                case NpgsqlDbType.Char:
                case NpgsqlDbType.Varchar:
                case NpgsqlDbType.Bytea:
                //case NpgsqlDbType.BFile:
                case NpgsqlDbType.Text:
                    {
                        sbCommandText.Append(param.NpgsqlDbType.ToString().ToUpper());
                        sbCommandText.Append('(');
                        sbCommandText.Append(param.Size);
                        sbCommandText.Append(')');
                        sbCommandText.Append("(MAX )");
                    }
                    break;

                case NpgsqlDbType.Smallint:
                case NpgsqlDbType.Integer:
                case NpgsqlDbType.Bigint:
                case NpgsqlDbType.Real:
                case NpgsqlDbType.Double:
                case NpgsqlDbType.Numeric:
                case NpgsqlDbType.Money:
                case NpgsqlDbType.Citext:
                case NpgsqlDbType.Boolean:
                case NpgsqlDbType.Xml:
                case NpgsqlDbType.Point:
                case NpgsqlDbType.Bit:
                case NpgsqlDbType.Varbit:
                case NpgsqlDbType.Date:
                case NpgsqlDbType.Time:
                case NpgsqlDbType.Timestamp:
                case NpgsqlDbType.Json:
                    {
                        sbCommandText.Append(param.NpgsqlDbType.ToString().ToUpper());
                    }
                    break;

                default:
                    {
                        sbCommandText.Append("/* UNKNOWN DATATYPE: ");
                        sbCommandText.Append(param.NpgsqlDbType.ToString().ToUpper());
                        sbCommandText.Append(" *" + "/ ");
                        sbCommandText.Append(param.NpgsqlDbType.ToString().ToUpper());
                    }
                    break;
            }
        }
    }
}
