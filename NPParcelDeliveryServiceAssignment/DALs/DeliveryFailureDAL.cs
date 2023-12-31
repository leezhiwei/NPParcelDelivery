﻿using NPParcelDeliveryServiceAssignment.Models;
using System.Data.SqlClient;

namespace NPParcelDeliveryServiceAssignment.DALs
{
    public class DeliveryFailureDAL
    {
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
        private string CheckNullString(SqlDataReader r, int num)
        {
            if (r.IsDBNull(num)) // check if null
            {
                return null; // return null
            }
            else // if not null
            {
                return r.GetString(num); // put in object
            }
        }
        private IConfiguration Configuration { get; }
        private SqlConnection conn;
        //Constructor
        public DeliveryFailureDAL()
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
        public List<DeliveryFailure> GetAllFailureReport()
        { //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand(); //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM DeliveryFailure ORDER BY ReportID"; //Open a database connection
            conn.Open(); //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            //Read all records until the end, save data into a staff list
            List<DeliveryFailure> dflist = new List<DeliveryFailure>();
            while (reader.Read())
            {
                dflist.Add(new DeliveryFailure
                {
                    ReportID = reader.GetInt32(0), //0: 1st column
                    ParcelID = reader.GetInt32(1), //1: 2nd column
                    DeliveryManID = reader.GetInt32(2), //2: 3rd column
                    FailureType = reader.GetString(3), //3: 4th column
                    Description = reader.GetString(4), //4: 4th column
                    StationMgrID = CheckNull(reader,5), //6: 5th column
                    FollowUpAction = CheckNullString(reader,6), //9: 6th column
                    DateCreated = reader.GetDateTime(7),
                });
            }
            reader.Close();
            conn.Close();
            return dflist;
        }

        public int Add(DeliveryFailure df)
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify an INSERT SQL statement which will
            //return the auto-generated StaffID after insertion
            cmd.CommandText = @"INSERT INTO DeliveryFailure (ParcelID, DeliveryManID, FailureType,
                                Description, StationMgrID, FollowUpAction, DateCreated)
                                OUTPUT INSERTED.ReportID
                                VALUES(@pid, @did, @fty, @desc, @smgrid, @fua, 
                                @dc)";
            //Define the parameters used in SQL statement, value for each parameter
            //is retrieved from respective class's property.
            cmd.Parameters.AddWithValue("@pid", df.ParcelID);
            cmd.Parameters.AddWithValue("@did", df.DeliveryManID);
            cmd.Parameters.AddWithValue("@fty", df.FailureType);
            cmd.Parameters.AddWithValue("@desc", df.Description);
            SqlParameter smgrid = cmd.Parameters.AddWithValue("@smgrid", df.StationMgrID);
            if (df.StationMgrID is null)// Checks if item desc is null, if so it adds a null value to the db directly
            {
                smgrid.Value = DBNull.Value;
            }
            SqlParameter fua = cmd.Parameters.AddWithValue("@fua", df.FollowUpAction);
            if (df.FollowUpAction is null)// Checks if item desc is null, if so it adds a null value to the db directly
            {
                fua.Value = DBNull.Value;
            }
            cmd.Parameters.AddWithValue("@dc", df.DateCreated);
            //A connection to database must be opened before any operations made.
            if (conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Open();
            //ExecuteScalar is used to retrieve the auto-generated
            //StaffID after executing the INSERT SQL statement
            df.ReportID = (int)cmd.ExecuteScalar();
            //A connection should be closed after operations.
            conn.Close();
            //Return id when no error occurs.
            return df.ReportID;
        }

        public int Update(DeliveryFailure df)
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify an UPDATE SQL statement
            cmd.CommandText = @"update DeliveryFailure
                                set
                                FollowUpAction = @FUpaction
                                WHERE ReportID = @ReportId";
            //Define the parameters used in SQL statement, value for each parameter
            //is retrieved from respective class's property.
            cmd.Parameters.AddWithValue("@FUpaction", df.FollowUpAction);
            cmd.Parameters.AddWithValue("@ReportId", df.ReportID);
            //Open a database connection
            if (conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Open();
            //ExecuteNonQuery is used for UPDATE and DELETE
            int count = cmd.ExecuteNonQuery();
            //Close the database connection
            conn.Close();
            return count;
        }
        public DeliveryFailure GetOne(int pid)
        {
            SqlCommand cmd = conn.CreateCommand(); //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM DeliveryFailure WHERE ReportID = @pid"; //Open a database connection
            cmd.Parameters.AddWithValue("@pid", pid);
            if (conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Open(); //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                DeliveryFailure df = new DeliveryFailure
                {
                    ReportID = reader.GetInt32(0), //0: 1st column
                    ParcelID = reader.GetInt32(1), //1: 2nd column
                    DeliveryManID = reader.GetInt32(2), //2: 3rd column
                    FailureType = reader.GetString(3), //3: 4th column
                    Description = reader.GetString(4), //4: 4th column
                    StationMgrID = CheckNull(reader, 5), //6: 5th column
                    FollowUpAction = CheckNullString(reader, 6), //9: 6th column
                    DateCreated = reader.GetDateTime(7),
                };
                return df;
            }
            return null;
        }
        public DeliveryFailure GetOneByParcelID (int pid)
        {
            SqlCommand cmd = conn.CreateCommand(); //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM DeliveryFailure WHERE ParcelID = @pid"; //Open a database connection
            cmd.Parameters.AddWithValue("@pid", pid);
            if (conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Open(); //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                DeliveryFailure df = new DeliveryFailure
                {
                    ReportID = reader.GetInt32(0), //0: 1st column
                    ParcelID = reader.GetInt32(1), //1: 2nd column
                    DeliveryManID = reader.GetInt32(2), //2: 3rd column
                    FailureType = reader.GetString(3), //3: 4th column
                    Description = reader.GetString(4), //4: 4th column
                    StationMgrID = CheckNull(reader, 5), //6: 5th column
                    FollowUpAction = CheckNullString(reader, 6), //9: 6th column
                    DateCreated = reader.GetDateTime(7),
                };
                return df;
            }
            return null;
        }
        public bool CheckIfSimilar(DeliveryFailure d)
        {
            SqlCommand cmd = conn.CreateCommand(); //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT COUNT(*) FROM DeliveryFailure WHERE ParcelID = @pid AND DeliveryManID = @dmanid AND FailureType = @ft AND Description = @desc"; //Open a database connection
            cmd.Parameters.AddWithValue("@pid", d.ParcelID);
            cmd.Parameters.AddWithValue("@dmanid", d.DeliveryManID);
            cmd.Parameters.AddWithValue("@ft", d.FailureType);
            cmd.Parameters.AddWithValue("@desc", d.Description);
            if (conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Open(); //Execute the SELECT SQL through a DataReader
            int count = (int)cmd.ExecuteScalar();
            conn.Close();
            if (count > 0)
            {
                return true;
            }
            return false;
        }
    }
}
