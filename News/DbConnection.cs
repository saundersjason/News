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
                //dataConn = new SqlConnection(connStringName);
                dataConn = new SqlConnection("Data Source=CSIT-SVR-SQL01;Initial Catalog=SSU_NEWS;User Id=dbapps;Password=movrdom2;");
            }
            catch
            {
                throw new Exception("Could not connect to database.");
            }
            return dataConn;
        }
    }
}