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

        public int AddMember(Member member)
        {
            // Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify an INSERT SQL statement which will
            //return the auto-generated StaffID after insertion
            cmd.CommandText = @"INSERT INTO Member (Name, Salutation, TelNo, EmailAddr, Password, BirthDate, City, Country)
                                OUTPUT INSERTED.MemberID
                                VALUES(@name, @sal, @tel, @ema, @pa, @bd, @ci, @co)";
            //Define the parameters used in SQL statement, value for each parameter
            //is retrieved from respective class's property.
            cmd.Parameters.AddWithValue("@name", member.Name);
            cmd.Parameters.AddWithValue("@sal", member.Salutation);
			cmd.Parameters.AddWithValue("@tel", member.TelNo);
			cmd.Parameters.AddWithValue("@ema", member.EmailAddr);
			cmd.Parameters.AddWithValue("@pa", member.Password);
			cmd.Parameters.AddWithValue("@bd", member.BirthDate);
			cmd.Parameters.AddWithValue("@ci", member.City);
			cmd.Parameters.AddWithValue("@co", member.Country);
			//A connection to database must be opened before any operations made.
            if (conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
			conn.Open();
            //ExecuteScalar is used to retrieve the auto-generated
            //StaffID after executing the INSERT SQL statement
            member.MemberID = (int)cmd.ExecuteScalar();
            //A connection should be closed after operations.
            conn.Close();
            //Return id when no error occurs.
            return member.MemberID;
		 }
        
        public Member CheckMember(string email, string password)
        {
            List<Member> mlist = GetAllMember();
            foreach (Member m in mlist)
            {
                if (m.EmailAddr == email)   
                {
                    if (m.Password == password)
                    {
                        return m;
                    }
                }
            }
            return null;
        }
    }
}
