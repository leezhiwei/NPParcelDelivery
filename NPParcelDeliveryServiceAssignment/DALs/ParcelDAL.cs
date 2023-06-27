using NPParcelDeliveryServiceAssignment.Models;
using System.Data.SqlClient;

namespace NPParcelDeliveryServiceAssignment.DALs
{
    public class ParcelDAL
    {
        private IConfiguration Configuration { get; }
        private SqlConnection conn;
        private int? CheckNull(SqlDataReader r, int num)
        {
            if (r.IsDBNull(num))
            {
                return null;
            }
            else
            {
                return r.GetInt32(num);
            }
        }
        //Constructor
        public ParcelDAL()
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
        public List<Parcel> GetAllParcel()
        { //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand(); //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM Parcel ORDER BY ParcelID"; //Open a database connection
            conn.Open(); //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            //Read all records until the end, save data into a staff list
            List<Parcel> parcellist = new List<Parcel>();
            while (reader.Read())
            {
                parcellist.Add(new Parcel
                {
                    ParcelID = reader.GetInt32(0), //0: 1st column
                    ItemDescription = reader.GetString(1), //1: 2nd column 
                    //Get the first character of a string
                    SenderName = reader.GetString(2), //2: 3rd column
                    SenderTelNo = reader.GetString(3), //3: 4th column
                    ReceiverName = reader.GetString(4), //4: 4th column
                    ReceiverTelNo = reader.GetString(5), //6: 5th column
                    DeliveryAddress = reader.GetString(6), //9: 6th column
                    FromCity = reader.GetString(7),
                    FromCountry = reader.GetString(8),
                    ToCity = reader.GetString(9),
                    ToCountry = reader.GetString(10),
                    ParcelWeight = reader.GetDouble(11),
                    DeliveryCharge = reader.GetDecimal(12),
                    Currency = reader.GetString(13),
                    TargetDeliveryDate = reader.GetDateTime(14),
                    DeliveryStatus = reader.GetString(15),
                    DeliveryManID = CheckNull(reader, 16),
                });
            }
            return parcellist;
        }
    }
}
