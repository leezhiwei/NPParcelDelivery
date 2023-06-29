using NPParcelDeliveryServiceAssignment.Models;
using System.Data.SqlClient;

namespace NPParcelDeliveryServiceAssignment.DALs
{
    public class DeliveryDAL
    {
        private IConfiguration Configuration { get; }
        private SqlConnection conn;
        private int? CheckNull(SqlDataReader r, int num)
        {
            if (r.IsDBNull(num)) // check if null
            {
                return null; // return null
            }
            else // if not null
            {
                return r.GetInt32(num); // put in object
            }
        }
        //Constructor
        public DeliveryDAL()
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
        public List<DeliveryHistory> GetAllHistory()
        { //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand(); //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM DeliveryHistory ORDER BY ParcelID"; //Open a database connection
            conn.Open(); //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            //Read all records until the end, save data into a staff list
            List<DeliveryHistory> historyList = new List<DeliveryHistory>();
            while (reader.Read())
            {
                historyList.Add(new DeliveryHistory
                {
                    RecordID = reader.GetInt32(0), //0: 1st column
                    ParcelID = reader.GetInt32(1), //1: 2nd column 
                    Description = reader.GetString(2), //2: 3rd column
                });
            }
            return historyList;
        }

        public int Add(DeliveryHistory history)
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify an INSERT SQL statement which will
            //return the auto-generated StaffID after insertion
            cmd.CommandText = @"INSERT INTO DeliveryHistory (ParcelID, Description)
                                OUTPUT INSERTED.RecordID
                                VALUES(@parcelID, @desc)";
            //Define the parameters used in SQL statement, value for each parameter
            //is retrieved from respective class's property.
            cmd.Parameters.AddWithValue("@parcelID", history.ParcelID);
            cmd.Parameters.AddWithValue("@desc", history.Description);
            //A connection to database must be opened before any operations made.
            conn.Open();
            //ExecuteScalar is used to retrieve the auto-generated
            //StaffID after executing the INSERT SQL statement
            history.RecordID = (int)cmd.ExecuteScalar();
            //A connection should be closed after operations.
            conn.Close();
            //Return id when no error occurs.
            return history.RecordID;
        }
    }
}
