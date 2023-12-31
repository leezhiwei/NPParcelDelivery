﻿using NPParcelDeliveryServiceAssignment.Models;
using System.Data.SqlClient;

namespace NPParcelDeliveryServiceAssignment.DALs
{
    public class DeliveryHistoryDAL
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
        public DeliveryHistoryDAL()
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
        { 
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand(); //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM DeliveryHistory ORDER BY RecordID"; //Open a database connection
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
            //Close DataReader
            reader.Close(); 
            //Close the database connection
            conn.Close();
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

        public DeliveryHistory GetOne(int dhid)
        {
            SqlCommand cmd = conn.CreateCommand(); //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM DeliveryHistory WHERE RecordID = @rid"; //Open a database connection
            cmd.Parameters.AddWithValue("@rid", dhid);
            if (conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Open(); //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                DeliveryHistory dh = new DeliveryHistory
                {
                    RecordID = reader.GetInt32(0), //0: 1st column
                    ParcelID = reader.GetInt32(1), //1: 2nd column
                    Description = reader.GetString(2), //1: 3rd column
                };
                return dh;
            }
            return null;
        }
		public List<DeliveryHistory> GetParcelHistory(int parcelid)
		{
			//Create a SqlCommand object from connection object
			SqlCommand cmd = conn.CreateCommand(); //Specify the SELECT SQL statement
			cmd.CommandText = @"SELECT * FROM DeliveryHistory WHERE ParcelID = @PID ORDER BY RecordID"; //Open a database connection
            cmd.Parameters.AddWithValue("@PID", parcelid);
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
			//Close DataReader
			reader.Close();
			//Close the database connection
			conn.Close();
			return historyList;
		}

        public DeliveryHistory GetDHByID(int dhid)
        {
            SqlCommand cmd = conn.CreateCommand(); //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM DeliveryHistory WHERE RecordID = @rid"; //Open a database connection
            cmd.Parameters.AddWithValue("@rid", dhid);
            if (conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Open(); //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            DeliveryHistory dh = null;
            while (reader.Read())
            {
                dh = new DeliveryHistory
                {
                    RecordID = reader.GetInt32(0), //0: 1st column
                    ParcelID = reader.GetInt32(1), //1: 2nd column
                    Description = reader.GetString(2), //1: 3rd column
                };
            }
            reader.Close();
            conn.Close();
            return dh;
        }
    }
}
