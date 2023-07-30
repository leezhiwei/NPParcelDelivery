using NPParcelDeliveryServiceAssignment.Models;
using System.Data.SqlClient;
using System.Diagnostics.Metrics;
using System.Reflection.Metadata;
using System.Xml.Linq;

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
            if (conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
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
            conn.Close();
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
			SqlParameter bd = cmd.Parameters.AddWithValue("@bd", member.BirthDate);
            if(member.BirthDate is null)
            {
                bd.Value = DBNull.Value;
            }
			SqlParameter ct = cmd.Parameters.AddWithValue("@ci", member.City);
            if (member.City is null)
            {
                ct.Value = DBNull.Value;
            }
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
            SqlCommand cmd = conn.CreateCommand(); //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM Member WHERE EmailAddr = @LoginID AND Password = @pwd"; //Open a database connection
            cmd.Parameters.AddWithValue("@LoginID", email);
            cmd.Parameters.AddWithValue("@pwd", password);
            if (conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Open(); //Execute the SELECT SQL through a DataReader
            Member m = null;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                m = new Member
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
                };
            }
            return m;
        }
        public List<Member> GetMemberDOBMonth()
        {
            List<Member> members = new List<Member>();
            SqlCommand cmd = conn.CreateCommand(); //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM Member WHERE MONTH(BirthDate) = MONTH(GETDATE())"; //Open a database connection
            if (conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Open(); //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                members.Add(new Member
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
            return members;
        }

		public Member GetMemberfromLoginID(string LoginID)
		{
			SqlCommand cmd = conn.CreateCommand(); //Specify the SELECT SQL statement
			cmd.CommandText = @"SELECT * FROM Member WHERE EmailAddr = @LoginID"; //Open a database connection
            cmd.Parameters.AddWithValue("@LoginID", LoginID);
			if (conn.State == System.Data.ConnectionState.Open)
			{
				conn.Close();
			}
			conn.Open(); //Execute the SELECT SQL through a DataReader
            Member m = null;
			SqlDataReader reader = cmd.ExecuteReader();
			while (reader.Read())
			{
				m = new Member
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
				};
			}
			return m;
		}
        public Member GetMIDByID(int id)
        {
            SqlCommand cmd = conn.CreateCommand(); //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM Member WHERE MemberID = @mid "; //Open a database connection
            cmd.Parameters.AddWithValue("@mid", id);
            if (conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Open(); //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Member mem = new Member
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
                };
                return mem;
            }
            return null;
        }
        public bool CheckEmail(string email)
        {
            SqlCommand cmd = conn.CreateCommand(); //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT COUNT(*) FROM Member WHERE EmailAddr = @ea "; //Open a database connection
            cmd.Parameters.AddWithValue("@ea", email);
            if (conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Open(); //Execute the SELECT SQL through a DataReader
            int count = (int)cmd.ExecuteScalar();
            conn.Close();
            return count >= 1;
        }
        public int Update(Member m)
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify an UPDATE SQL statement
            cmd.CommandText = @"update Member
                                set Name = @n,
                                Salutation = @s,
                                TelNo = @tn,
                                EmailAddr = @ea,
                                Password = @p,
                                BirthDate = @bd,
                                City = @ci,
                                Country = @co,
                                OriginalPassword = null
                                WHERE MemberID = @mid";
            //Define the parameters used in SQL statement, value for each parameter
            //is retrieved from respective class's property.
            cmd.Parameters.AddWithValue("@n", m.Name);
            cmd.Parameters.AddWithValue("@s", m.Salutation);
            cmd.Parameters.AddWithValue("@tn", m.TelNo);
            cmd.Parameters.AddWithValue("@ea", m.EmailAddr);
            cmd.Parameters.AddWithValue("@p", m.Password);
            cmd.Parameters.AddWithValue("@bd", m.BirthDate);
            cmd.Parameters.AddWithValue("@co", m.Country);
            cmd.Parameters.AddWithValue("@ci", m.City);
            cmd.Parameters.AddWithValue("@mid", m.MemberID);
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
