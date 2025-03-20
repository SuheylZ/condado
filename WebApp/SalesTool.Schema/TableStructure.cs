using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace SalesTool.Schema
{
     public class TableStructure
    {
         public DataTable GetDatatable(String connnectionString,String query)
         {
             string connString = connnectionString;
             SqlConnection sqlconn = new SqlConnection(connString);
             SqlCommand cmd = new SqlCommand(query, sqlconn);
             SqlDataAdapter da = new SqlDataAdapter(cmd);
             DataSet DS = new DataSet();
             da.Fill(DS, "mytable");
             return DS.Tables[0];
         }
         public bool UpdateFilterOrderNumbers(String connnectionString, int deletedOrderNumber, int parentKey, short parentType)
         {
             try
             {
                 string connString = connnectionString;
                 SqlConnection sqlconn = new SqlConnection(connString);
                 sqlconn.Open();
                 SqlCommand cmd = new SqlCommand();
                 cmd.Connection = sqlconn;
                 cmd.CommandType = CommandType.StoredProcedure;
                 cmd.CommandText = "proj_ReorderFilters";
                 cmd.Parameters.Add(new SqlParameter("@parentKey", SqlDbType.Int));
                 cmd.Parameters.Add(new SqlParameter("@parentType", SqlDbType.SmallInt));
                 cmd.Parameters.Add(new SqlParameter("@OrderNumber", SqlDbType.Int));
                 cmd.Parameters["@parentKey"].Value = parentKey;
                 cmd.Parameters["@parentType"].Value = parentType;
                 cmd.Parameters["@OrderNumber"].Value = deletedOrderNumber;


                 int rowsAffected= cmd.ExecuteNonQuery();
                 sqlconn.Close();
                 return false;
             }
             catch 
             {
                 return false;
             }
             
         }

         // Salestool.Direct.TableStructure:
         // SZ [Apr 9, 2013] This is a function that aviods crunching the CPU to itas limit.
         public bool AccountExists(String connnectionString, String query, long accId)
         {
             bool bAns = false;
             const string K_WHERE = "where ";
             const string K_WHERE_2 = " where (accounts.act_key = {0}) AND ";

             query = query.ToLower();
             int index = query.IndexOf(K_WHERE);
             query = index != -1 ? query.Replace(K_WHERE, string.Format(K_WHERE_2, accId.ToString())) : query + string.Format(K_WHERE_2, accId.ToString());

             query = query.EndsWith(" where ") ? query.Remove(query.Length - 7) : query;
             query = query.EndsWith(" AND ") ? query.Remove(query.Length - 5) : query;
             query = query.EndsWith(" OR ") ? query.Remove(query.Length - 4) : query;

             string connString = connnectionString;
             using (IDbConnection cnn = new SqlConnection(connString))
             {
                 cnn.Open();
                 using (IDbCommand cmd = cnn.CreateCommand())
                 {
                     cmd.CommandText = query;
                     cmd.CommandType = CommandType.Text;
                     using (IDataReader rd = cmd.ExecuteReader())
                     {
                         while (rd.Read())
                         {
                             bAns = true;
                             break;
                         }
                         rd.Close();
                     }
                 }
                 cnn.Close();
             }
             return bAns;
         }
		 
    }
}
