using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;
using Ex8.Helper.FileSystem;

namespace Ex8.Helper.Dsv
{
    public static class DsvConverter
    {
        public static DataTable ReadDsvToDatable(string fullPath, string delimeter)
        {
            var dt = new DataTable();

            var encoding = FileHelper.GetFileEncoding(fullPath);

            using (var fs = File.OpenRead(fullPath))
            {
                using (var reader = new StreamReader(fs, encoding, true))
                {
                    using (var csv = new CsvReader(reader))
                    {
                        csv.Configuration.Delimiter = delimeter;
                        csv.Configuration.IgnoreQuotes = false;
                        csv.Configuration.DetectColumnCountChanges = true;
                        csv.Configuration.TrimOptions = CsvHelper.Configuration.TrimOptions.Trim;

                        using (var dr = new CsvDataReader(csv))
                        {
                            dt.Load(dr);

                            foreach (DataColumn col in dt.Columns)
                            {
                                col.ReadOnly = false;
                            }
                        }
                    }
                }
            }

            return dt;
        }

        public static void WriteDatableDsv(DataTable dataTable, string fullPath, string delimeter)
        {
            using (var streamWriter = new StreamWriter(fullPath, false, Encoding.UTF8))
            {
                streamWriter.AutoFlush = true;

                using (var csv = new CsvWriter(streamWriter))
                {
                    csv.Configuration.Delimiter = delimeter;
                    csv.Configuration.ShouldQuote = (string field, WritingContext context) => field.Contains(delimeter);

                    foreach (DataColumn column in dataTable.Columns)
                    {
                        csv.WriteField(column.ColumnName);
                    }

                    csv.NextRecord();

                    foreach (DataRow row in dataTable.Rows)
                    {
                        for (var i = 0; i < dataTable.Columns.Count; i++)
                        {
                            csv.WriteField(row[i]);
                        }

                        csv.NextRecord();
                    }
                }

                streamWriter.Close();
            }
        }

        public static DataTable ConvertStringToDataTable(this string csvString, string delimeterString)
        {
            try
            {
                var dataTable = new DataTable();

                var rowData = csvString.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                var col = from cl in rowData[0].Split(delimeterString.ToCharArray())
                    select new DataColumn(cl);

                dataTable.Columns.AddRange(col.ToArray());

                rowData.Skip(1).Select(st => dataTable.Rows.Add(st.Split(delimeterString.ToCharArray()))).ToList();

                return dataTable;
            }
            catch (Exception error)
            {
                return null;
            }
        }

        public static string ConvertDataTableToString(this DataTable dataTable, string delimeterString)
        {
            var sb = new StringBuilder();

            var columnNames = dataTable.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
            sb.AppendLine(string.Join(delimeterString, columnNames));

            foreach (DataRow row in dataTable.Rows)
            {
                var fields = row.ItemArray.Select(field => field.ToString());
                sb.AppendLine(string.Join(",", fields));
            }

            return sb.ToString();
        }
    }
}