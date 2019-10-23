using Dapper;
using Ex8.EtlModel;
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

        public int BulkCopy(string connectionString, string destinationTableName, Table tableInfo, DataTable data)
        {
            using (var connection = new OracleConnection(connectionString))
            {
                int[] ids = new int[data.Rows.Count]; //TODO type could be any type. Should be based on tableInfo.pk_data_type
                //TODO create other arrays here. Type should be based on tableInfo.columns[0].dataType

                for (int j = 0; j < data.Rows.Count; j++)
                {
                    ids[j] = Convert.ToInt32(data.Rows[j][tableInfo.pk_column_name]);
                    //TODO initialise other array values here so we only loop through data.Rows.Count once
                }

                // create command and set properties  
                OracleCommand cmd = connection.CreateCommand();
                cmd.CommandText = GetCommandText(destinationTableName, tableInfo);
                cmd.ArrayBindCount = ids.Length;

                cmd.Parameters.Add(new OracleParameter { OracleDbType = OracleDbType.Int32, Value = ids });  //TODO type could be any type. Should be based on tableInfo.pk_data_type

                // for datatable columns
                foreach (var tabinfocol in tableInfo.columns)
                {
                    // get other columns name and its data type exclude primary key column
                    var columnName = tabinfocol.columnName;
                    var columntype = tabinfocol.dataType;
                    // for datatable rows
                    OracleParameter p = new OracleParameter();                                        
                    List<dynamic> colValueArr = new List<dynamic>(data.Rows.Count);
                    foreach (DataRow dr in data.Rows)
                    {
                        var colValue = dr[columnName].ToString();                       
                       // case type switch
                        switch (columntype.ToLower())
                        {
                            case "number":
                                p.OracleDbType = OracleDbType.Decimal;
                                colValueArr.Add(Convert.ToInt32(colValue));
                                break;
                            case "varchar2":
                                p.DbType = DbType.String;
                                colValueArr.Add(Convert.ToString(colValue));
                                break;                           
                            case "datetime":
                            case "date":
                                p.DbType = DbType.DateTime;
                                colValueArr.Add(Convert.ToDateTime(colValue));
                                break;                            
                            case "text":
                                p.DbType = DbType.String;
                                colValueArr.Add(Convert.ToString(colValue));
                                break;
                            default:
                                break;
                        }                                             
                    }
                    p.Value = colValueArr.ToArray();                   
                    cmd.Parameters.Add(p);
                }

                connection.Open();
                return cmd.ExecuteNonQuery();
            }
        }


        /// <summary>
        /// Expected command text = INSERT INTO {destinationTableName} (PERSON_ID, FIRST_NAME, LAST_NAME) VALUES (:1, :2, :3)
        /// </summary>
        /// <param name="destinationTableName"></param>
        /// <param name="tableInfo"></param>
        /// <returns></returns>
        internal string GetCommandText(string destinationTableName, Table tableInfo)
        {
            var sql = new StringBuilder("INSERT INTO " + destinationTableName + " (" + tableInfo.pk_column_name);
            var values = new StringBuilder("VALUES ( :" + tableInfo.pk_column_name); // TODO does this match expected result? Eg: VALUES (:1, :2, :3)??

            foreach (var col in tableInfo.columns)
            {
                sql.Append($",{col.columnName}");
                values.Append($",:{col.columnName}");
            }

            sql.Append(") ");
            sql.Append(values.ToString());
            sql.Append(")");

            return sql.ToString();
        }


        //public int UpdateBulkData(string connectionString,string UpdateFromTempDml)
        //{
        //    using (var connection = new OracleConnection(connectionString))
        //    {
        //        connection.Open();
        //        OracleCommand cmd = connection.CreateCommand();
        //        cmd.CommandText = UpdateFromTempDml;
        //        return cmd.ExecuteNonQuery();
        //    }               
        //}
    }
}
