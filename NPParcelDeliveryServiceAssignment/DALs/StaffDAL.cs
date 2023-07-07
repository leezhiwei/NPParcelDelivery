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
            return staffList;
        }
        public Staff CheckStaff(string LoginID, string Password)
        {
            List<Staff> stafflist = GetAllStaff();
            foreach (Staff staff in stafflist)
            {
                if (staff.LoginID == LoginID)
                {
                    if (staff.Password == Password)
                    {
                        return staff;
                    }
                }
            }
            return null;
        }
        public bool IfStaffExist(int sid)
        {
            List<Staff> stafflist = GetAllStaff();
            foreach (Staff s in stafflist)
            {
                if (s.StaffID == sid)
                {
                    return true;
                }
            }
            return false;
        }
        public int ReturnStaffID(string LoginID)
        {
            List<Staff> slist = GetAllStaff();
            foreach (Staff s in slist)
            {
                if (s.LoginID == LoginID)
                {
                    return s.StaffID;
                }
            }
            return -1;
        }
        public Staff GetOneStaff(int StaffID)
        {
            List<Staff> staff = GetAllStaff();
            foreach(Staff s in staff)
            {
                if (s.StaffID == StaffID)
                {
                    return s;
                }
            }
            return null;
        }
    }
}
