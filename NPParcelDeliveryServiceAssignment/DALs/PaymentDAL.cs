using NPParcelDeliveryServiceAssignment.Models;
using System.Data.SqlClient;

namespace NPParcelDeliveryServiceAssignment.DALs
{
    public class PaymentDAL
    {

        private IConfiguration Configuration { get; }
        private SqlConnection conn;

        public PaymentDAL()
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

        public List<PaymentTransaction> GetAllPayment()
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand(); //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM PaymentTransaction ORDER BY TransactionID"; //Open a database connection
            conn.Open(); //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            //Read all records until the end, save data into a staff list
            List<PaymentTransaction> paymentList = new List<PaymentTransaction>();
            while (reader.Read())
            {
                paymentList.Add(new PaymentTransaction
                {
                    TransactionID = reader.GetInt32(0), //0: 1st column
                    ParcelID = reader.GetInt32(1), //1: 2nd column 
                    AmtTran = reader.GetDecimal(2), //2: 3rd column
                    Currency = reader.GetString(3), //2: 4th column
                    TranType = reader.GetString(4), //2: 5th column
                    TranDate = reader.GetDateTime(5), //2: 6th column
                });
            }
            //Close DataReader
            reader.Close();
            //Close the database connection
            conn.Close();
            return paymentList;
        }

        public int Add(PaymentTransaction payment)
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify an INSERT SQL statement which will
            //return the auto-generated StaffID after insertion
            cmd.CommandText = @"INSERT INTO PaymentTransaction (ParcelID, AMtTran, Currency, TranType, TranDate)
                                OUTPUT INSERTED.TransactionID
                                VALUES(@parcelID, @amtTran, @currency, @tranType, @tranDate)";
            //Define the parameters used in SQL statement, value for each parameter
            //is retrieved from respective class's property.
            cmd.Parameters.AddWithValue("@parcelID", payment.ParcelID);
            cmd.Parameters.AddWithValue("@amtTran", payment.AmtTran);
            cmd.Parameters.AddWithValue("@currency", payment.Currency);
            cmd.Parameters.AddWithValue("@tranType", payment.TranType);
            cmd.Parameters.AddWithValue("@tranDate", payment.TranDate);
            //A connection to database must be opened before any operations made.
            conn.Open();
            //ExecuteScalar is used to retrieve the auto-generated
            //StaffID after executing the INSERT SQL statement
            payment.TransactionID = (int)cmd.ExecuteScalar();
            //A connection should be closed after operations.
            conn.Close();
            //Return id when no error occurs.
            return payment.TransactionID;
        }

        public PaymentTransaction GetPTByID(string id)
        {
            SqlCommand cmd = conn.CreateCommand(); 
            cmd.CommandText = @"SELECT * FROM PaymentTransaction WHERE TransactionID = @tid"; 
            cmd.Parameters.AddWithValue("@tid", id);
            if (conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Open(); 
            PaymentTransaction pt = new PaymentTransaction();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                pt = new PaymentTransaction
                {
                    TransactionID = reader.GetInt32(0), //0: 1st column
                    ParcelID = reader.GetInt32(1), //1: 2nd column 
                    AmtTran = reader.GetDecimal(2), //2: 3rd column
                    Currency = reader.GetString(3), //3: 4th column
                    TranType = reader.GetString(4), //4: 4th column
                    TranDate = reader.GetDateTime(5), //6: 5th column
                };
            }
            //Close DataReader
            reader.Close();
            //Close database connection
            conn.Close();
            return pt;
        }

        public decimal? GetSumAmtByID(int id)
        {
            SqlCommand cmd = conn.CreateCommand(); 
            cmd.CommandText = @"SELECT SUM(AmtTran) FROM PaymentTransaction WHERE ParcelID = @pid"; 
            cmd.Parameters.AddWithValue("@pid", id);
            if (conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Open(); 
            decimal? totalAmt = 0;
            var totalAmtValue = cmd.ExecuteScalar();
            conn.Close();
            if (totalAmtValue == DBNull.Value)
            {
                totalAmt = null;
                return totalAmt;
            }
            totalAmt = (decimal?)totalAmtValue;
            return totalAmt;
        }

        public PaymentTransaction GetPTHByID(int id)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT * FROM PaymentTransaction WHERE TransactionID = @tid";
            cmd.Parameters.AddWithValue("@tid", id);
            if (conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Open();
            PaymentTransaction pt = null;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                pt = new PaymentTransaction
                {
                    TransactionID = reader.GetInt32(0), //0: 1st column
                    ParcelID = reader.GetInt32(1), //1: 2nd column 
                    AmtTran = reader.GetDecimal(2), //2: 3rd column
                    Currency = reader.GetString(3), //3: 4th column
                    TranType = reader.GetString(4), //4: 4th column
                    TranDate = reader.GetDateTime(5), //6: 5th column
                };
            }
            //Close DataReader
            reader.Close();
            //Close database connection
            conn.Close();
            return pt;
        }

    }
}
