using Dapper;
using Ex8.EtlModel.DatabaseJobManifest;
//using Oracle.DataAccess.Client;
using OracleConnection = Oracle.ManagedDataAccess.Client.OracleConnection;
using OracleDataAdapter = Oracle.ManagedDataAccess.Client.OracleDataAdapter;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Oracle.ManagedDataAccess.Client;

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

        public int BulkCopy(string connectionString, string destinationTableName, DataTable data)
        {
            int noOfRecord = 0;
            try
            {
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

                    OracleParameter id = new OracleParameter();
                    id.OracleDbType = OracleDbType.Int32;
                    id.Value = ids;

                    OracleParameter name = new OracleParameter();
                    name.OracleDbType = OracleDbType.Varchar2;
                    name.Value = fnames;

                    OracleParameter lname = new OracleParameter();
                    lname.OracleDbType = OracleDbType.Varchar2;
                    lname.Value = lnames;

                    // create command and set properties  
                    OracleCommand cmd = connection.CreateCommand();                                
                    cmd.CommandText = $"INSERT INTO {destinationTableName} (PERSON_ID, FIRST_NAME, LAST_NAME) VALUES (:1, :2, :3)";                            
                    cmd.ArrayBindCount = ids.Length;
                    cmd.Parameters.Add(id);
                    cmd.Parameters.Add(name);
                    cmd.Parameters.Add(lname);
                    int i = cmd.ExecuteNonQuery();
                    OracleCommand cmd1 = connection.CreateCommand();
                    cmd1.CommandText = $"select * from {destinationTableName}";
                    OracleDataReader reader = cmd1.ExecuteReader();                    
                    while(reader.Read())
                    {
                        noOfRecord++;
                    }
               }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return noOfRecord;
        } 
    }
}
