using NPParcelDeliveryServiceAssignment.Models;
using System.Data.SqlClient;

namespace NPParcelDeliveryServiceAssignment.DALs
{
    public class DeliveryDAL
    {

        /*
        private IConfiguration Configuration { get; }
        private SqlConnection conn; //Constructor
        public StaffDAL() 
        { 
            //Read ConnectionString from appsettings.json file
            var builder = new ConfigurationBuilder() .SetBasePath(Directory.GetCurrentDirectory()) .AddJsonFile("appsettings.json"); 
            Configuration = builder.Build(); 
            string strConn = Configuration.GetConnectionString( "NPBookConnectionString"); 
            //Instantiate a SqlConnection object with the 
            //Connection String read.
            conn = new SqlConnection(strConn);
        }
        public List<Staff> GetAllStaff()
        { //Create a SqlCommand object from connection object SqlCommand cmd = conn.CreateCommand(); //Specify the SELECT SQL statement cmd.CommandText = @"SELECT * FROM Staff ORDER BY StaffID"; //Open a database connection conn.Open(); //Execute the SELECT SQL through a DataReader SqlDataReader reader = cmd.ExecuteReader();
          //Read all records until the end, save data into a staff list*/
    }
}
