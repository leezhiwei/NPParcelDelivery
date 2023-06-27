using System.Data.SqlClient;
using System.Reflection.PortableExecutable;
using NPParcelDeliveryServiceAssignment.Models;
namespace NPParcelDeliveryServiceAssignment.DALs
{
    public class ShippingRateDAL
    {
        private IConfiguration Configuration { get; }
        private SqlConnection conn;
        //Constructor
        public ShippingRateDAL()
        { //Read ConnectionString from appsettings.json file
            var builder = new ConfigurationBuilder()
                  .SetBasePath(Directory.GetCurrentDirectory())
                  .AddJsonFile("appsettings.json");
            Configuration = builder.Build();
            string strConn = Configuration.GetConnectionString("NPParcelSystemConnStr");
            //Instantiate a SqlConnection object with the 
            //Connection String read.
            conn = new SqlConnection(strConn);
        }
        public List<ShippingRate> GetAllShippingRate()
        { //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand(); //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM ShippingRate ORDER BY ShippingRateID"; //Open a database connection
            conn.Open(); //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            //Read all records until the end, save data into a ShippingRate list
            List<ShippingRate> shippingRateList = new List<ShippingRate>();
            while (reader.Read())
            {
                shippingRateList.Add(new ShippingRate
                {
                    ShippingRateID = reader.GetInt32(0), //0: 1st colu
                                                         //mn
                    FromCity = reader.GetString(1), //1: 2nd column 
                    //Get the first character of a string
                    FromCountry = reader.GetString(2), //2: 3rd column
                    ToCity = reader.GetString(3), //3: 4th column
                    ToCountry = reader.GetString(4), //4: 4th column
                    ShipRate = reader.GetDecimal(5), //6: 5th column
                    Currency = reader.GetString(6), //9: 6th column 
                    TransitTime = reader.GetInt32(7), //9: 7th column
                    LastUpdatedBy = reader.GetInt32(8), //9: 8th column
                });
            }
            return shippingRateList;
        }
    }
}
