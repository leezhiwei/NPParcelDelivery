using NPParcelDeliveryServiceAssignment.Models;
using System.Data.SqlClient;
using System.Reflection.Metadata;

namespace NPParcelDeliveryServiceAssignment.DALs
{
    public class MemberDAL
    {
        private IConfiguration Configuration { get; }
        private SqlConnection conn;
        //Constructor
        public MemberDAL()
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
        public List<Member> GetAllMember()
        { //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand(); //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM Member ORDER BY MemberID"; //Open a database connection
            conn.Open(); //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            //Read all records until the end, save data into a staff list
            List<Member> memberList = new List<Member>();
            while (reader.Read())
            {
                memberList.Add(new Member
                {
                    MemberID = reader.GetInt32(0), //0: 1st column
                    Name = reader.GetString(1), //1: 2nd column 
                    //Get the first character of a string
                    Salutation = reader.GetString(2), //2: 3rd column
                    TelNo = reader.GetString(3), //3: 4th column
                    EmailAddr = reader.GetString(4), //4: 4th column
                    Password = reader.GetString(5), //6: 5th column
                    BirthDate = reader.GetDateTime(6), //9: 6th column
                    City = reader.GetString(7),
                    Country = reader.GetString(8),
                });
            }
            return memberList;
        }
        public bool CheckMember(string email, string password)
        {
            List<Member> mlist = GetAllMember();
            foreach (Member m in mlist)
            {
                if (m.EmailAddr == email)
                {
                    if (m.Password == password)
                    {
                        return true;
                    }
                    return false;
                }
            }
            return false;
        }
    }
}
