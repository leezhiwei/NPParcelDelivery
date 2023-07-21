using NPParcelDeliveryServiceAssignment.Models;
using System.Data.SqlClient;

namespace NPParcelDeliveryServiceAssignment.DALs
{
    public class FeedbackDAL
    {
        private IConfiguration Configuration { get; }
        private SqlConnection conn;
        private StaffDAL sd = new StaffDAL();
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
        public FeedbackDAL()
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
        public List<FeedbackEnquiry> GetAllFeedback()
        { //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand(); //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM FeedbackEnquiry ORDER BY FeedbackEnquiryID"; //Open a database connection
            conn.Open(); //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            //Read all records until the end, save data into a staff list
            List<FeedbackEnquiry> feedbacklist = new List<FeedbackEnquiry>();
            while (reader.Read())
            {
                feedbacklist.Add(new FeedbackEnquiry
                {
                    FeedbackEnquiryID = reader.GetInt32(0),
                    MemberID = reader.GetInt32(1),
                    Content = reader.GetString(2),
                    DateTimePosted = reader.GetDateTime(3),
                    StaffID = CheckNull(reader, 4),
                    Response = !reader.IsDBNull(5) ?
                    reader.GetString(5) : (string)null,
                    Status = reader.GetString(6),
                });
            }
            return feedbacklist;
        }

        public int Add(FeedbackEnquiry fb)
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify an INSERT SQL statement which will
            //return the auto-generated StaffID after insertion
            cmd.CommandText = @"INSERT INTO FeedbackEnquiry (MemberID, Content, DateTimePosted, StaffID,
                                Response, Status)
                                OUTPUT INSERTED.FeedbackEnquiryID
                                VALUES(@mid, @c, @dtp, @sid, @r, @s)";
            //Define the parameters used in SQL statement, value for each parameter
            //is retrieved from respective class's property.


            cmd.Parameters.AddWithValue("@mid", fb.MemberID);
            cmd.Parameters.AddWithValue("@c", fb.Content);
            cmd.Parameters.AddWithValue("@dtp", fb.DateTimePosted);
            cmd.Parameters.AddWithValue("@s", fb.Status);
            SqlParameter itmd1 = cmd.Parameters.AddWithValue("@sid", fb.StaffID);
            if (fb.StaffID is null)// Checks if item desc is null, if so it adds a null value to the db directly
            {
                itmd1.Value = DBNull.Value;
            }
            SqlParameter itmd2 = cmd.Parameters.AddWithValue("@r", fb.Response);
            if (fb.Response is null)// Checks if item desc is null, if so it adds a null value to the db directly
            {
                itmd2.Value = DBNull.Value;
            }

            //A connection to database must be opened before any operations made.
            conn.Open();
            //ExecuteScalar is used to retrieve the auto-generated
            //StaffID after executing the INSERT SQL statement
            fb.FeedbackEnquiryID = (int)cmd.ExecuteScalar();
            //A connection should be closed after operations.
            conn.Close();
            //Return id when no error occurs.
            return fb.FeedbackEnquiryID;
        }
        public int? Update(FeedbackEnquiry f)
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify an UPDATE SQL statement
            cmd.CommandText = @"UPDATE FeedbackEnquiry SET MemberID=@mid,
            Content=@c, DateTimePosted=@dtp, StaffID=@sid, Response=@r, Status=@s                               
            WHERE FeedbackEnquiryID = @fid";
            //Define the parameters used in SQL statement, value for each parameter
            //is retrieved from respective class's property.
            cmd.Parameters.AddWithValue("@fid", f.FeedbackEnquiryID);
            cmd.Parameters.AddWithValue("@mid", f.MemberID);
            cmd.Parameters.AddWithValue("@c", f.Content);
            cmd.Parameters.AddWithValue("@dtp", f.DateTimePosted);
            cmd.Parameters.AddWithValue("@s", f.Status);
            SqlParameter itmd1 = cmd.Parameters.AddWithValue("@sid", f.StaffID);
            if (f.StaffID is null)// Checks if item desc is null, if so it adds a null value to the db directly
            {
                itmd1.Value = DBNull.Value;
            }
            SqlParameter itmd2 = cmd.Parameters.AddWithValue("@r", f.Response);
            if (f.Response is null)// Checks if item desc is null, if so it adds a null value to the db directly
            {
                itmd2.Value = DBNull.Value;
            }
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
        public List<FeedbackEnquiry> GetMemberFeedback(Member m)
        { //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand(); //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM FeedbackEnquiry WHERE MemberID = @m ORDER BY FeedbackEnquiryID"; //Open a database connection
            cmd.Parameters.AddWithValue("@m", m.MemberID);
            conn.Open(); //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            //Read all records until the end, save data into a staff list
            List<FeedbackEnquiry> feedbacklist = new List<FeedbackEnquiry>();
            while (reader.Read())
            {
                feedbacklist.Add(new FeedbackEnquiry
                {
                    FeedbackEnquiryID = reader.GetInt32(0),
                    MemberID = reader.GetInt32(1),
                    Content = reader.GetString(2),
                    DateTimePosted = reader.GetDateTime(3),
                    StaffID = CheckNull(reader, 4),
                    Response = !reader.IsDBNull(5) ?
                    reader.GetString(5) : (string)null,
                    Status = reader.GetString(6),
                });
            }
            return feedbacklist;

        }
        public FeedbackEnquiry GetOneFeedback(int fid)
        { //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand(); //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM FeedbackEnquiry WHERE FeedbackEnquiryID = @fid"; //Open a database connection
            cmd.Parameters.AddWithValue("@fid", fid);
            conn.Open(); //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            //Read all records until the end, save data into a staff list
            FeedbackEnquiry f = new FeedbackEnquiry();
            while (reader.Read())
            {
                f = new FeedbackEnquiry
                {
                    FeedbackEnquiryID = reader.GetInt32(0),
                    MemberID = reader.GetInt32(1),
                    Content = reader.GetString(2),
                    DateTimePosted = reader.GetDateTime(3),
                    StaffID = CheckNull(reader, 4),
                    Response = !reader.IsDBNull(5) ?
                    reader.GetString(5) : (string)null,
                    Status = reader.GetString(6),
                };
            }
            return f;

        }
    }
}
