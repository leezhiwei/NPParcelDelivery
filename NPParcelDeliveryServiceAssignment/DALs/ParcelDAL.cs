using Microsoft.AspNetCore.Mvc.Rendering;
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
                    ParcelID = reader.GetInt32(0),
                    ItemDescription = !reader.IsDBNull(1) ?
                    reader.GetString(1) : (string)null,
                    SenderName = reader.GetString(2),
                    SenderTelNo = reader.GetString(3),
                    ReceiverName = reader.GetString(4),
                    ReceiverTelNo = reader.GetString(5),
                    DeliveryAddress = reader.GetString(6),
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
            //Close DataReader
            reader.Close();
            //Close database connection
            conn.Close();
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
                                ParcelWeight, DeliveryCharge, Currency, TargetDeliveryDate)
                                OUTPUT INSERTED.ParcelID
                                VALUES(@itemDesc, @senderName, @senderTelNo, @receiverName, @receiverTelNo, @deliveryAddrs, 
                                @fromCity, @fromCountry, @toCity, @toCountry, @parcelWeight, @deliveryCharge, @currency, @targetDeliveryDate)";
            //Define the parameters used in SQL statement, value for each parameter
            //is retrieved from respective class's property.
            SqlParameter itmd = cmd.Parameters.AddWithValue("@itemDesc", parcel.ItemDescription);
            if (parcel.ItemDescription is null)// Checks if item desc is null, if so it adds a null value to the db directly
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
        public List<Parcel> CheckAssigned(int StaffID)
        {
            SqlCommand cmd = conn.CreateCommand(); //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM Parcel WHERE DeliveryManID = @sid AND DeliveryStatus = '1'"; //Open a database connection
            cmd.Parameters.AddWithValue("@sid", StaffID);
            conn.Open(); //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            //Read all records until the end, save data into a staff list
            List<Parcel> parcellist = new List<Parcel>();
            while (reader.Read())
            {
                parcellist.Add(new Parcel
                {
                    ParcelID = reader.GetInt32(0),
                    ItemDescription = !reader.IsDBNull(1) ?
                    reader.GetString(1) : (string)null,
                    SenderName = reader.GetString(2),
                    SenderTelNo = reader.GetString(3),
                    ReceiverName = reader.GetString(4),
                    ReceiverTelNo = reader.GetString(5),
                    DeliveryAddress = reader.GetString(6),
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
            //Close DataReader
            reader.Close();
            //Close database connection
            conn.Close();
            return parcellist;
        }
        public List<Parcel> GetParcelFromMember(Member m)
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand(); //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM Parcel WHERE (SenderName = @sn AND SenderTelNo = @stelno) OR (ReceiverName = @sn AND ReceiverTelNo = @stelno)"; //Open a database connection
            cmd.Parameters.AddWithValue("@sn", m.Name);
            cmd.Parameters.AddWithValue("@stelno", m.TelNo);
            conn.Open(); //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            //Read all records until the end, save data into a staff list
            List<Parcel> parcelList = new List<Parcel>();
            while (reader.Read())
            {
                parcelList.Add(new Parcel
                {
                    ParcelID = reader.GetInt32(0),
                    ItemDescription = !reader.IsDBNull(1) ?
                    reader.GetString(1) : (string)null,
                    SenderName = reader.GetString(2),
                    SenderTelNo = reader.GetString(3),
                    ReceiverName = reader.GetString(4),
                    ReceiverTelNo = reader.GetString(5),
                    DeliveryAddress = reader.GetString(6),
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
            //Close DataReader
            reader.Close();
            //Close the database connection
            conn.Close();
            return parcelList;
        }

        public Parcel GetPIDByPID(int parid)
        {
            SqlCommand cmd = conn.CreateCommand(); //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM Parcel WHERE ParcelID = @pid"; //Open a database connection
            cmd.Parameters.AddWithValue("@pid", parid);
            if (conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Open(); //Execute the SELECT SQL through a DataReader
            Parcel p = null;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                p = new Parcel
                {
                    ParcelID = reader.GetInt32(0),
                    ItemDescription = !reader.IsDBNull(1) ?
                    reader.GetString(1) : (string)null,
                    SenderName = reader.GetString(2),
                    SenderTelNo = reader.GetString(3),
                    ReceiverName = reader.GetString(4),
                    ReceiverTelNo = reader.GetString(5),
                    DeliveryAddress = reader.GetString(6),
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
                };
            }
            reader.Close();
            conn.Close();
            return p;
            
        }
        public int GetCountFromStaffID(int staffID)
        {
            SqlCommand cmd = conn.CreateCommand(); //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT COUNT(*) FROM Parcel WHERE DeliveryManID = @dmanid AND DeliveryStatus = '1'"; //Open a database connection
            cmd.Parameters.AddWithValue("@dmanid", staffID);
            if (conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Open(); //Execute the SELECT SQL through a DataReader
            int count = (int)cmd.ExecuteScalar();
            conn.Close();
            return count;
        }
        public List<SelectListItem> GetDManCount()
        {
            List<SelectListItem> list = new List<SelectListItem> ();
            SqlCommand cmd = conn.CreateCommand(); //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT StaffName, StaffID FROM Staff LEFT JOIN (SELECT ParcelID, DeliveryManID FROM Parcel WHERE DeliveryStatus = '1') AS ""a"" ON a.DeliveryManID = Staff.StaffID WHERE Appointment = 'Delivery Man' GROUP BY StaffName, StaffID HAVING COUNT(*) < 5;"; //Open a database connection
            if (conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Open(); //Execute the SELECT SQL through a DataReader
            SqlDataReader r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new SelectListItem
                {
                    Text = r.GetString(0),
                    Value = r.GetInt32(1).ToString()
                });
            }
            //Close DataReader
            r.Close();
            //Close database connection
            conn.Close();
            return list;
        }
        public Parcel GetEitherByParcelIDReceiverNameOrSenderName(int parid, string rname, string sname)
        {
            SqlCommand cmd = conn.CreateCommand(); //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM Parcel WHERE ParcelID = @pid OR ReceiverName = @rname OR SenderName = @sname"; //Open a database connection
            cmd.Parameters.AddWithValue("@pid", parid);
            cmd.Parameters.AddWithValue("@rname", rname);
            cmd.Parameters.AddWithValue("@sname", sname);
            if (conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Open(); //Execute the SELECT SQL through a DataReader
            Parcel p = null;
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                p = new Parcel
                {
                    ParcelID = reader.GetInt32(0),
                    ItemDescription = !reader.IsDBNull(1) ?
                    reader.GetString(1) : (string)null,
                    SenderName = reader.GetString(2),
                    SenderTelNo = reader.GetString(3),
                    ReceiverName = reader.GetString(4),
                    ReceiverTelNo = reader.GetString(5),
                    DeliveryAddress = reader.GetString(6),
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
                };
            }
            reader.Close();
            conn.Close();
            return p;

        }
    }
}
