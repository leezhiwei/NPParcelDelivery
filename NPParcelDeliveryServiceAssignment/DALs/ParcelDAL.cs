using NPParcelDeliveryServiceAssignment.Models;
using System.Data.SqlClient;

namespace NPParcelDeliveryServiceAssignment.DALs
{
    public class ParcelDAL
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
        public ParcelDAL()
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
        public List<Parcel> GetAllParcel()
        { //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand(); //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM Parcel ORDER BY ParcelID"; //Open a database connection
            conn.Open(); //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            //Read all records until the end, save data into a staff list
            List<Parcel> parcellist = new List<Parcel>();
            while (reader.Read())
            {
                parcellist.Add(new Parcel
                {
                    ParcelID = reader.GetInt32(0), //0: 1st column
                    ItemDescription = !reader.IsDBNull(1) ? //1: 2nd column
                    reader.GetString(1) : (string)null, 
                    SenderName = reader.GetString(2), //2: 3rd column
                    SenderTelNo = reader.GetString(3), //3: 4th column
                    ReceiverName = reader.GetString(4), //4: 4th column
                    ReceiverTelNo = reader.GetString(5), //6: 5th column
                    DeliveryAddress = reader.GetString(6), //9: 6th column
                    FromCity = reader.GetString(7),
                    FromCountry = reader.GetString(8),
                    ToCity = reader.GetString(9),
                    ToCountry = reader.GetString(10),
                    ParcelWeight = reader.GetDouble(11),
                    DeliveryCharge = reader.GetDecimal(12),
                    Currency = reader.GetString(13),
                    TargetDeliveryDate = reader.GetDateTime(14),
                    DeliveryStatus = reader.GetString(15),
                    DeliveryManID = CheckNull(reader, 16),
                });
            }
            return parcellist;
        }

        public int Add(Parcel parcel)
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify an INSERT SQL statement which will
            //return the auto-generated StaffID after insertion
            cmd.CommandText = @"INSERT INTO Parcel (ItemDescription, SenderName, SenderTelNo,
                                ReceiverName, ReceiverTelNo, DeliveryAddress, FromCity, FromCountry, ToCity, ToCountry, 
                                ParcelWeight, DeliveryCharge, Currency, TargetDeliveryDate, DeliveryStatus, DeliveryManID)
                                OUTPUT INSERTED.ParcelID
                                VALUES(@itemDesc, @senderName, @senderTelNo, @receiverName, @receiverTelNo, @deliveryAddrs, 
                                @fromCity, @fromCountry, @toCity, @toCountry, @parcelWeight, @deliveryCharge, @currency, @targetDeliveryDate, 
                                @deliveryStatus, @deliveryManID)";
            //Define the parameters used in SQL statement, value for each parameter
            //is retrieved from respective class's property.
            SqlParameter itmd = cmd.Parameters.AddWithValue("@itemDesc", parcel.ItemDescription);
            if(parcel.ItemDescription is null)// Checks if item desc is null, if so it adds a null value to the db directly
            {
                itmd.Value = DBNull.Value;
            }
            cmd.Parameters.AddWithValue("@senderName", parcel.SenderName);
            cmd.Parameters.AddWithValue("@senderTelNo", parcel.SenderTelNo);
            cmd.Parameters.AddWithValue("@receiverName", parcel.ReceiverName);
            cmd.Parameters.AddWithValue("@receiverTelNo", parcel.ReceiverTelNo);
            cmd.Parameters.AddWithValue("@deliveryAddrs", parcel.DeliveryAddress);
            cmd.Parameters.AddWithValue("@fromCity", parcel.FromCity);
            cmd.Parameters.AddWithValue("@fromCountry", parcel.FromCountry);
            cmd.Parameters.AddWithValue("@toCity", parcel.ToCity);
            cmd.Parameters.AddWithValue("@toCountry", parcel.ToCountry);
            cmd.Parameters.AddWithValue("@parcelWeight", parcel.ParcelWeight);
            cmd.Parameters.AddWithValue("@deliveryCharge", parcel.DeliveryCharge);
            cmd.Parameters.AddWithValue("@currency", parcel.Currency);
            cmd.Parameters.AddWithValue("@targetDeliveryDate", parcel.TargetDeliveryDate);
            cmd.Parameters.AddWithValue("@deliveryStatus", parcel.DeliveryStatus);
            SqlParameter dmid = cmd.Parameters.AddWithValue("@deliveryManID", parcel.DeliveryManID);
            if (parcel.DeliveryManID is null)// Checks if item desc is null, if so it adds a null value to the db directly
            {
                dmid.Value = DBNull.Value;
            }
            //A connection to database must be opened before any operations made.
            conn.Open();
            //ExecuteScalar is used to retrieve the auto-generated
            //StaffID after executing the INSERT SQL statement
            parcel.ParcelID = (int)cmd.ExecuteScalar();
            //A connection should be closed after operations.
            conn.Close();
            //Return id when no error occurs.
            return parcel.ParcelID;
        }
        public int? Update(Parcel p)
        {
            if (p.DeliveryManID is not null)
            {
                int dmanid = (int)p.DeliveryManID;
                if (!sd.IfStaffExist(dmanid))
                {
                    return null;
                }
            }
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify an UPDATE SQL statement
            cmd.CommandText = @"UPDATE Parcel SET ItemDescription=@itemdesc,
            SenderName=@sname, SenderTelNo=@stelno, ReceiverName=@rname, ReceiverTelNo=@rtelno,
            DeliveryAddress=@deladdr,FromCity=@fcity,FromCountry=@fcountry,ToCity=@tcity,
            ToCountry=@tcountry,ParcelWeight=@pweight,DeliveryCharge=@dcharge,Currency=@curr,TargetDeliveryDate=@tdd,DeliveryStatus=@ds,
            DeliveryManID=@dmanid
            WHERE ParcelID = @parcelID";
            //Define the parameters used in SQL statement, value for each parameter
            //is retrieved from respective class's property.
            cmd.Parameters.AddWithValue("@itemdesc", p.ItemDescription);
            cmd.Parameters.AddWithValue("@sname", p.SenderName);
            cmd.Parameters.AddWithValue("@stelno", p.SenderTelNo);
            cmd.Parameters.AddWithValue("@rname", p.ReceiverName);
            cmd.Parameters.AddWithValue("@rtelno", p.ReceiverTelNo);
            cmd.Parameters.AddWithValue("@deladdr", p.DeliveryAddress);
            cmd.Parameters.AddWithValue("@fcity", p.FromCity);
            cmd.Parameters.AddWithValue("@fcountry", p.FromCountry);
            cmd.Parameters.AddWithValue("@tcity", p.ToCity);
            cmd.Parameters.AddWithValue("@tcountry", p.ToCountry);
            cmd.Parameters.AddWithValue("@pweight", p.ParcelWeight);
            cmd.Parameters.AddWithValue("@dcharge", p.DeliveryCharge);
            cmd.Parameters.AddWithValue("@curr", p.Currency);
            cmd.Parameters.AddWithValue("@tdd", p.TargetDeliveryDate);
            cmd.Parameters.AddWithValue("@ds", p.DeliveryStatus);
            if (p.DeliveryManID is null)
            {
                cmd.Parameters.AddWithValue("@dmanid", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@dmanid", p.DeliveryManID);
            }
            cmd.Parameters.AddWithValue("@parcelID", p.ParcelID);
            //Open a database connection
            conn.Open();
            //ExecuteNonQuery is used for UPDATE and DELETE
            int count = cmd.ExecuteNonQuery();
            //Close the database connection
            conn.Close();
            return count;
        }
        public List<Parcel> CheckAssigned(int StaffID)
        {
            List<Parcel> allparcel = GetAllParcel();
            List<Parcel> asslist = new List<Parcel>();
            foreach (Parcel p in allparcel)
            {
                if (p.DeliveryManID == StaffID)
                {
                    if (p.DeliveryStatus == "1")
                    {
                        asslist.Add(p);
                    }
                }
            }
            return asslist;
        }
        public Parcel ReturnParcel(int pid)
        {
            List<Parcel> plist = GetAllParcel();
            foreach (Parcel p in plist)
            {
                if (pid == p.ParcelID)
                {
                    return p;
                }
            }
            return null;
        }
    }
}
