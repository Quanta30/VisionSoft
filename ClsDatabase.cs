namespace VisionSoft;
using System.Data;
using Microsoft.Data.SqlClient;
using System;

public class ClsDatabase
    {
        Config config = new Config();
        
       
        SqlConnection con = new SqlConnection(Config.ConnectionString);


        //Selecting Table from Database and returning DataTable Master Query Function
        public DataTable GetDataTable(string sqlQuery){
                DataTable Dt = new DataTable();

                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand(sqlQuery, con);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(Dt);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
                finally
                {
                    con.Close();
                }
                return Dt;
        }

        //Inserting or Updating or Deleting Data in Database, Master Query Function
        public bool ExecuteQuery(string sqlQuery)
        {   
            Console.Write("Executing Query : " + sqlQuery + "\n");
        try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(sqlQuery, con);
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return false;
            }
            finally
            {
                con.Close();
            }
        }


        //Inserting into Database
        public Boolean SaveRecord(string TableName, string ColumnString, string ValueString)
        {
            Console.WriteLine("SaveRecord Called");
            string sqlQuery = $"INSERT INTO {TableName} ({ColumnString}) VALUES ({ValueString})";
            return ExecuteQuery(sqlQuery);
        }


        //Delete records from Given Table Satisfying the condition
        public Boolean DeleteRecord(string TableName, string Condition)
        {
            string sqlQuery = $"DELETE FROM {TableName} WHERE {Condition}";
            return ExecuteQuery(sqlQuery);
        }

        //Generate Next ID for a given table
        public string GenerateNextNo(string TableName, string PrimaryKey, string Condition = "")
        {
            try
            {
                string query;
                if (string.IsNullOrEmpty(Condition))
                {
                    query = $"SELECT MAX({PrimaryKey}) FROM {TableName}";
                }
                else
                {
                    query = $"SELECT MAX({PrimaryKey}) FROM {TableName} WHERE {Condition}";
                }

                DataTable dt = GetDataTable(query);

                if (dt.Rows.Count == 0 || dt.Rows[0][0] == DBNull.Value)
                {
                    return "1";
                }
                else
                {
                    return (Convert.ToInt64(dt.Rows[0][0]) + 1).ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error generating next number: " + ex.Message);
                return "1"; // Return default value in case of error
            }
        }
    
        //Get value of a specific column from a table based on condition
        public string GetScalar(string Query)
        {
            try
            {
                string sqlQuery = Query;
                DataTable dt = GetDataTable(sqlQuery);
                return dt.Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving column value: " + ex.Message);
                return string.Empty; // Return empty string in case of error
            }
        }

        
}
