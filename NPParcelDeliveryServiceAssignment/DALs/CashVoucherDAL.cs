using NPParcelDeliveryServiceAssignment.Models;
using System;
using System.Data.SqlClient;

namespace NPParcelDeliveryServiceAssignment.DALs
{
    public class CashVoucherDAL
    {
        private IConfiguration Configuration { get; }
        private SqlConnection conn;

        public CashVoucherDAL()
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
        public List<CashVoucher> GetAllCashVoucher()
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand(); //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM CashVoucher ORDER BY CashVoucherID"; //Open a database connection
            conn.Open(); //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            //Read all records until the end, save data into a staff list
            List<CashVoucher> cashvoucherList = new List<CashVoucher>();
            while (reader.Read())
            {
                cashvoucherList.Add(new CashVoucher
                {
                    CashVoucherID = reader.GetInt32(0), //0: 1st column
                    StaffID = reader.GetInt32(1), //1: 2nd column 
                    Amount = reader.GetDecimal(2), //2: 3rd column
                    Currency = reader.GetString(3), //2: 4th column
                    IssuingCode = reader.GetString(4), //2: 5th column
                    ReceiverName = reader.GetString(5), //2: 6th column
                    ReceiverTelNo = reader.GetString(6), //2: 7th column
                    DateTimeIssued = reader.GetDateTime(7),//2: 8th column
                    Status = reader.GetString(8),//2: 9th column
                });
            }
            //Close DataReader
            reader.Close();
            //Close the database connection
            conn.Close();
            return cashvoucherList;
        }
        public int Update(CashVoucher cashVoucher)
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify an UPDATE SQL statement
            cmd.CommandText = @"update CashVoucher 
                                set Status=@nstatus
                                WHERE CashVoucherID = @cvID";
            //Define the parameters used in SQL statement, value for each parameter
            //is retrieved from respective class's property.
            cmd.Parameters.AddWithValue("@nstatus", cashVoucher.Status);
            cmd.Parameters.AddWithValue("@cvID", cashVoucher.CashVoucherID);
            //Open a database connection
            conn.Open();
            //ExecuteNonQuery is used for UPDATE and DELETE
            int count = cmd.ExecuteNonQuery();
            //Close the database connection
            conn.Close();
            return count;
        }

		public int Add(CashVoucher cashVoucher)
		{
			//Create a SqlCommand object from connection object
			SqlCommand cmd = conn.CreateCommand();
			//Specify an INSERT SQL statement which will
			//return the auto-generated StaffID after insertion
			cmd.CommandText = @"INSERT INTO CashVoucher (  StaffID, Amount, 
                                Currency, IssuingCode,ReceiverName, ReceiverTelNo,DateTimeIssued,Status) 
                                                    OUTPUT INSERTED.CashVoucherID 
                                                    VALUES(  @sid,@amount, 
                                                   @currency, @ic, @rn, @rtn,@dti, @status)";
			//Define the parameters used in SQL statement, value for each parameter
			//is retrieved from respective class's property.
			cmd.Parameters.AddWithValue("@sid", cashVoucher.StaffID);
			cmd.Parameters.AddWithValue("@amount", cashVoucher.Amount);
			cmd.Parameters.AddWithValue("@currency", cashVoucher.Currency);
			cmd.Parameters.AddWithValue("@ic", cashVoucher.IssuingCode);
			cmd.Parameters.AddWithValue("@rn", cashVoucher.ReceiverName);
			cmd.Parameters.AddWithValue("@rtn", cashVoucher.ReceiverTelNo);
			cmd.Parameters.AddWithValue("@dti", cashVoucher.DateTimeIssued);
			cmd.Parameters.AddWithValue("@status", cashVoucher.Status);
			//A connection to database must be opened before any operations made.
			conn.Open();

				//shipping rate  after executing the INSERT SQL statement
				cashVoucher.CashVoucherID = (int)cmd.ExecuteScalar();

			//A connection should be closed after operations.
			conn.Close();

			return cashVoucher.CashVoucherID;
		}
        public CashVoucher GetCVIDByNameAndTelNum(string rname, string tnum)
        {
            SqlCommand cmd = conn.CreateCommand(); //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM CashVoucher WHERE ReceiverName = @rn AND ReceiverTelNo = @tu"; //Open a database connection
            cmd.Parameters.AddWithValue("@rn", rname);
            cmd.Parameters.AddWithValue("@tu", tnum);
            if (conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Open(); //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                CashVoucher cv = new CashVoucher
                {
                    CashVoucherID = reader.GetInt32(0), //0: 1st column
                    StaffID = reader.GetInt32(1), //1: 2nd column 
                    Amount = reader.GetDecimal(2), //2: 3rd column
                    Currency = reader.GetString(3), //2: 4th column
                    IssuingCode = reader.GetString(4), //2: 5th column
                    ReceiverName = reader.GetString(5), //2: 6th column
                    ReceiverTelNo = reader.GetString(6), //2: 7th column
                    DateTimeIssued = reader.GetDateTime(7),//2: 8th column
                    Status = reader.GetString(8),//2: 9th column
                };
                return cv;
            }
            return null;
        }
        public CashVoucher GetCVIDByID(int id)
        {
            SqlCommand cmd = conn.CreateCommand(); //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM CashVoucher WHERE CashVoucherID = @cid "; //Open a database connection
            cmd.Parameters.AddWithValue("@cid", id);
            if (conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Open(); //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                CashVoucher cv = new CashVoucher
                {
                    CashVoucherID = reader.GetInt32(0), //0: 1st column
                    StaffID = reader.GetInt32(1), //1: 2nd column 
                    Amount = reader.GetDecimal(2), //2: 3rd column
                    Currency = reader.GetString(3), //2: 4th column
                    IssuingCode = reader.GetString(4), //2: 5th column
                    ReceiverName = reader.GetString(5), //2: 6th column
                    ReceiverTelNo = reader.GetString(6), //2: 7th column
                    DateTimeIssued = reader.GetDateTime(7),//2: 8th column
                    Status = reader.GetString(8),//2: 9th column
                };
                return cv;
            }
            return null;
        }
    }
}

