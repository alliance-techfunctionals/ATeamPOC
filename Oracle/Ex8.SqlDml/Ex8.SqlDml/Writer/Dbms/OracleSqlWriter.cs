using Dapper;
using Ex8.EtlModel.DatabaseJobManifest;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;


namespace Ex8.SqlDml.Writer.Dbms
{
    public class OracleSqlWriter : ISqlWriter
    {
        public DatabaseTypeEnum DatabaseType => DatabaseTypeEnum.Oracle;

        public void ExecuteSqlText(string connectionString, List<string> sqlList)
        {
            using (var targetConn = new OracleConnection(connectionString))
            {
                foreach (var sql in sqlList)
                {
                    targetConn.Execute(sql);
                }
            }
        }

        public DataSet GetData(string connectionString, string selectSql)
        {
            var ds = new DataSet();
            using (var connection = new OracleConnection(connectionString))
            using (var adapter = new OracleDataAdapter(selectSql, connectionString))
            {
                adapter.Fill(ds);
                return ds;
            }
        }

        public int BulkCopy(string connectionString, string destinationTableName, Table tableInfo, DataTable data)
        {
            int noOfRecord = 0;
            using (var connection = new OracleConnection(connectionString))
            {
                connection.Open();
                int[] ids = new int[data.Rows.Count];
                string[] fnames = new string[data.Rows.Count];
                string[] lnames = new string[data.Rows.Count];

                for (int j = 0; j < data.Rows.Count; j++)
                {
                    ids[j] =    Convert.ToInt32(data.Rows[j]["PERSON_ID"]);
                    fnames[j] = Convert.ToString(data.Rows[j]["FIRST_NAME"]);
                    lnames[j] = Convert.ToString(data.Rows[j]["LAST_NAME"]);
                }

                var id = new OracleParameter { OracleDbType = OracleDbType.Int32, Value = ids };
                var name = new OracleParameter { OracleDbType = OracleDbType.Varchar2, Value = fnames };
                var lname = new OracleParameter { OracleDbType = OracleDbType.Varchar2, Value = lnames };

                // create command and set properties  
                OracleCommand cmd = connection.CreateCommand();                                
                cmd.CommandText = $"INSERT INTO {destinationTableName} (PERSON_ID, FIRST_NAME, LAST_NAME) VALUES (:1, :2, :3)";                            
                cmd.ArrayBindCount = ids.Length;
                cmd.Parameters.Add(id);
                cmd.Parameters.Add(name);
                cmd.Parameters.Add(lname);

                noOfRecord = cmd.ExecuteNonQuery();
            }

            return noOfRecord;
        } 
    }
}
