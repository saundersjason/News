using System;
using System.Data;
using System.Data.SqlClient;
namespace SavannahState.News
{
    public class DBConnection
    {
        public SqlConnection dataConn;
        public DBConnection()
        {}
        
        public SqlConnection GetConnection(String connStringName)
        {
            try
            {
                dataConn = new SqlConnection("Your SQL Connection String Goes Here");
            }
            catch
            {
                throw new Exception("Could not connect to database.");
            }
            return dataConn;
        }
    }
}