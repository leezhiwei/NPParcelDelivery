using NPParcelDeliveryServiceAssignment.Models;
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
    }
}
