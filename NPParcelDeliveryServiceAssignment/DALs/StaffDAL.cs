using System.Data.SqlClient;
using System.Reflection.PortableExecutable;
using NPParcelDeliveryServiceAssignment.Models;
namespace NPParcelDeliveryServiceAssignment.DALs
{
    public class StaffDAL
    {
        private IConfiguration Configuration { get; }
        private SqlConnection conn;
        //Constructor
        public StaffDAL()
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
        public List<Staff> GetAllStaff()
        { //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand(); //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM Staff ORDER BY StaffID"; //Open a database connection
            conn.Open(); //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            //Read all records until the end, save data into a staff list
            List<Staff> staffList = new List<Staff>();
            while (reader.Read())
            {
                staffList.Add(new Staff
                {
                    StaffID = reader.GetInt32(0), //0: 1st column
                    StaffName = reader.GetString(1), //1: 2nd column 
                    //Get the first character of a string
                    LoginID = reader.GetString(2), //2: 3rd column
                    Password = reader.GetString(3), //3: 4th column
                    Appointment = reader.GetString(4), //4: 4th column
                    OfficeTelNo = reader.GetString(5), //6: 5th column
                    Location = reader.GetString(6), //9: 6th column 
                });
            }
            reader.Close();
            conn.Close();
            return staffList;
        }
        public Staff CheckStaff(string LoginID, string Password)
        {
            SqlCommand cmd = conn.CreateCommand(); //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM Staff WHERE LoginID = @lid AND Password = @pwd"; //Open a database connection
            cmd.Parameters.AddWithValue("@lid", LoginID);
            cmd.Parameters.AddWithValue("@pwd", Password);
            conn.Open(); //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            //Read all records until the end, save data into a staff list
            Staff s = null;
            while (reader.Read())
            {
                s = new Staff
                {
                    StaffID = reader.GetInt32(0), //0: 1st column
                    StaffName = reader.GetString(1), //1: 2nd column 
                    //Get the first character of a string
                    LoginID = reader.GetString(2), //2: 3rd column
                    Password = reader.GetString(3), //3: 4th column
                    Appointment = reader.GetString(4), //4: 4th column
                    OfficeTelNo = reader.GetString(5), //6: 5th column
                    Location = reader.GetString(6), //9: 6th column 
                };
            }
            reader.Close();
            conn.Close();
            return s;
        }
        public bool IfStaffExist(int sid)
        {
            SqlCommand cmd = conn.CreateCommand(); //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT COUNT(*) FROM Staff WHERE StaffID = @sid"; //Open a database connection
            cmd.Parameters.AddWithValue("@sid", sid);
            conn.Open(); //Execute the SELECT SQL through a DataReader
            int staffcount = (int)cmd.ExecuteScalar();
            conn.Close();
            if (staffcount == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public int ReturnStaffID(string LoginID)
        {
            SqlCommand cmd = conn.CreateCommand(); //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT StaffID FROM Staff WHERE LoginID = @lid"; //Open a database connection
            cmd.Parameters.AddWithValue("@lid", LoginID);
            if (conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Open(); //Execute the SELECT SQL through a DataReader
            var result = cmd.ExecuteScalar();
            conn.Close();
            if (result is null)
            {
                return -1;
            }
            return (int)result;
        }
        public Staff GetOneStaff(int StaffID)
        {
            SqlCommand cmd = conn.CreateCommand(); //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM Staff WHERE StaffID = @sid"; //Open a database connection
            cmd.Parameters.AddWithValue("@sid", StaffID);
            conn.Open(); //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            //Read all records until the end, save data into a staff list
            Staff s = null;
            while (reader.Read())
            {
                s = new Staff
                {
                    StaffID = reader.GetInt32(0), //0: 1st column
                    StaffName = reader.GetString(1), //1: 2nd column 
                    //Get the first character of a string
                    LoginID = reader.GetString(2), //2: 3rd column
                    Password = reader.GetString(3), //3: 4th column
                    Appointment = reader.GetString(4), //4: 4th column
                    OfficeTelNo = reader.GetString(5), //6: 5th column
                    Location = reader.GetString(6), //9: 6th column 
                };
            }
            reader.Close();
            conn.Close();
            return s;
        }

        public Staff GetSIDByID(int id)
        {
            SqlCommand cmd = conn.CreateCommand(); //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM Staff WHERE StaffID = @sid "; //Open a database connection
            cmd.Parameters.AddWithValue("@sid", id);
            if (conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Open(); //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Staff sta = new Staff
                {
                    StaffID = reader.GetInt32(0), //0: 1st column
                    StaffName = reader.GetString(1), //1: 2nd column 
                    //Get the first character of a string
                    LoginID = reader.GetString(2), //2: 3rd column
                    Password = reader.GetString(3), //3: 4th column
                    Appointment = reader.GetString(4), //4: 4th column
                    OfficeTelNo = reader.GetString(5), //6: 5th column
                    Location = reader.GetString(6), //9: 6th column 
                };
                return sta;
            }
            return null;
        }
        public int Update(Staff s)
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify an UPDATE SQL statement
            cmd.CommandText = @"update Staff
                                set StaffName = @sn,
                                LoginID = @lid,
                                Password = @pwd,
                                Appointment = @apt,
                                OfficeTelNo = @otn,
                                Location = @loc,
                                OriginalPassword = NULL
                                WHERE StaffID = @sid";
            //Define the parameters used in SQL statement, value for each parameter
            //is retrieved from respective class's property.
            cmd.Parameters.AddWithValue("@sn", s.StaffName);
            cmd.Parameters.AddWithValue("@lid",s.LoginID);
            cmd.Parameters.AddWithValue("@pwd", s.Password);
            cmd.Parameters.AddWithValue("@apt",s.Appointment);
            cmd.Parameters.AddWithValue("@otn",s.OfficeTelNo);
            cmd.Parameters.AddWithValue("@loc", s.Location);
            cmd.Parameters.AddWithValue("@sid", s.StaffID);
            //Open a database connection
            conn.Open();
            //ExecuteNonQuery is used for UPDATE and DELETE
            int count = cmd.ExecuteNonQuery();
            //Close the database connection
            conn.Close();
            return count;
        }
    }
}
