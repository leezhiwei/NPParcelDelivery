using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections;
using System.Collections.Generic;

using System.Data.SqlClient;
using System.Reflection.PortableExecutable;
using NPParcelDeliveryServiceAssignment.Models;

namespace NPParcelDeliveryServiceAssignment.DALs
{
    public class ShippingRateDAL
    {
        private IConfiguration Configuration { get; }
        private SqlConnection conn;
        
        //Constructor
        public ShippingRateDAL()
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
        public List<ShippingRate> GetAllShippingRate()
        { //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand(); //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM ShippingRate ORDER BY ShippingRateID"; //Open a database connection
            conn.Open(); //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            //Read all records until the end, save data into a ShippingRate list
            List<ShippingRate> shippingRateList = new List<ShippingRate>();
            while (reader.Read())
            {
                shippingRateList.Add(new ShippingRate
                {
                    ShippingRateID = reader.GetInt32(0), //0: 1st colu
                                                         //mn
                    FromCity = reader.GetString(1), //1: 2nd column 
                    //Get the first character of a string
                    FromCountry = reader.GetString(2), //2: 3rd column
                    ToCity = reader.GetString(3), //3: 4th column
                    ToCountry = reader.GetString(4), //4: 4th column
                    ShipRate = reader.GetDecimal(5), //6: 5th column
                    Currency = reader.GetString(6), //9: 6th column 
                    TransitTime = reader.GetInt32(7), //9: 7th column
                    LastUpdatedBy = reader.GetInt32(8), //9: 8th column
                });
            }
            reader.Close();
            conn.Close();
            return shippingRateList;
        }

        public int Add(ShippingRate shippingRate)
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify an INSERT SQL statement which will
            //return the auto-generated StaffID after insertion
            cmd.CommandText = @"INSERT INTO ShippingRate (  FromCity, FromCountry, ToCity, 
                                ToCountry, ShippingRate, Currency, TransitTime,LastUpdatedBy) 
                                                    OUTPUT INSERTED.ShippingRateID 
                                                    VALUES( @fromCity, @fromCountry,@toCity, 
                                                   @toCountry, @shippingRate, @currency, @transitTime,@lastUpdatedBy)";
            //Define the parameters used in SQL statement, value for each parameter
            //is retrieved from respective class's property.
            cmd.Parameters.AddWithValue("@fromCity", shippingRate.FromCity);
            cmd.Parameters.AddWithValue("@fromCountry", shippingRate.FromCountry);
            cmd.Parameters.AddWithValue("@toCity", shippingRate.ToCity);
            cmd.Parameters.AddWithValue("@toCountry", shippingRate.ToCountry);
            cmd.Parameters.AddWithValue("@shippingRate", shippingRate.ShipRate);
            cmd.Parameters.AddWithValue("@currency", shippingRate.Currency);
            cmd.Parameters.AddWithValue("@transitTime", shippingRate.TransitTime);
            cmd.Parameters.AddWithValue("@lastUpdatedBy", shippingRate.LastUpdatedBy);
            //A connection to database must be opened before any operations made.
            conn.Open();
			try
			{
				//shipping rate  after executing the INSERT SQL statement
				shippingRate.ShippingRateID = (int)cmd.ExecuteScalar();
			}

			catch
			{
				return 0;
			}
			//A connection should be closed after operations.
			conn.Close();

			return shippingRate.ShippingRateID;
		}

        public int Update(ShippingRate shippingRate)
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify an UPDATE SQL statement
            cmd.CommandText = @"update ShippingRate 
                                set ShippingRate=@sRate,
                                TransitTime=@tTime,
                                LastUpdatedBy=@luBy
                                WHERE ShippingRateID = @sRateID";
            //Define the parameters used in SQL statement, value for each parameter
            //is retrieved from respective class's property.
            cmd.Parameters.AddWithValue("@sRate", shippingRate.ShipRate);
            cmd.Parameters.AddWithValue("@tTime", shippingRate.TransitTime);
            cmd.Parameters.AddWithValue("@luBy", shippingRate.LastUpdatedBy);
            cmd.Parameters.AddWithValue("@sRateID", shippingRate.ShippingRateID);
            //Open a database connection
            conn.Open();
            //ExecuteNonQuery is used for UPDATE and DELETE
            int count = cmd.ExecuteNonQuery();
            //Close the database connection
            conn.Close();
            return count;
        }

		public int Delete(int shippingRateID)
		{
			//Instantiate a SqlCommand object, supply it with a DELETE SQL statement
			//to delete a staff record specified by a Staff ID
			SqlCommand cmd = conn.CreateCommand();
			cmd.CommandText = @"DELETE FROM ShippingRate
            WHERE ShippingRateID = @selectShippingRateID";
			cmd.Parameters.AddWithValue("@selectShippingRateID", shippingRateID);
			//Open a database connection
			conn.Open();
			int rowAffected = 0;
			//Execute the DELETE SQL to remove the staff record
			rowAffected += cmd.ExecuteNonQuery();
			//Close database connection
			conn.Close();
			//Return number of row of staff record updated or deleted
			return rowAffected;
		}

        
		public bool IsInfoExist(ShippingRate sr)
		{
			bool infoFound = false;
			//Create a SqlCommand object and specify the SQL statement 
			//to get a staff record with the email address to be validated
			SqlCommand cmd = conn.CreateCommand();
			cmd.CommandText = @"SELECT COUNT(*) FROM ShippingRate 
                                WHERE FromCity = @selectedFromCity AND FromCountry = @selectedFromCountry
                                AND ToCity = @selectedToCity AND ToCountry = @selectedToCountry";
			cmd.Parameters.AddWithValue("@selectedFromCity", sr.FromCity);
			cmd.Parameters.AddWithValue("@selectedFromCountry", sr.FromCountry);
			cmd.Parameters.AddWithValue("@selectedToCity", sr.ToCity);
			cmd.Parameters.AddWithValue("@selectedToCountry", sr.ToCountry);
			//Open a database connection and execute the SQL statement
			conn.Open();
            int count = (int)cmd.ExecuteScalar();
			conn.Close();
            if (count >= 1)
            {
                infoFound = true;
            }
			return infoFound;
		}

        public ShippingRate GetSRIDByID(string id)
        {
            SqlCommand cmd = conn.CreateCommand(); //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM ShippingRate WHERE ShippingRateID = @idd"; //Open a database connection
            cmd.Parameters.AddWithValue("@idd", id);
            if (conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Open(); //Execute the SELECT SQL through a DataReader
            ShippingRate sr = new ShippingRate();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                sr = new ShippingRate
                {
                    ShippingRateID = reader.GetInt32(0), //0: 1st column
                    FromCity = reader.GetString(1), //1: 2nd column 
                    //Get the first character of a string
                    FromCountry = reader.GetString(2), //2: 3rd column
                    ToCity = reader.GetString(3), //3: 4th column
                    ToCountry = reader.GetString(4), //4: 4th column
                    ShipRate = reader.GetDecimal(5), //6: 5th column
                    Currency = reader.GetString(6), //9: 6th column 
                    TransitTime = reader.GetInt32(7), //9: 7th column
                    LastUpdatedBy = reader.GetInt32(8), //9: 8th column
                };
            }
            reader.Close();
            conn.Close();
            return sr;
        }

        public ShippingRate GetSRbyCC(string toCity, string toCountry)
        {
            SqlCommand cmd = conn.CreateCommand(); //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM ShippingRate WHERE ToCity = @tocity AND ToCountry = @tocountry"; //Open a database connection
            cmd.Parameters.AddWithValue("@tocity", toCity);
            cmd.Parameters.AddWithValue("@tocountry", toCountry);
            if (conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
            conn.Open(); //Execute the SELECT SQL through a DataReader
            ShippingRate sr = new ShippingRate();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                sr = new ShippingRate
                {
                    ShippingRateID = reader.GetInt32(0), //0: 1st column
                    FromCity = reader.GetString(1), //1: 2nd column 
                    FromCountry = reader.GetString(2), //2: 3rd column
                    ToCity = reader.GetString(3), //3: 4th column
                    ToCountry = reader.GetString(4), //4: 4th column
                    ShipRate = reader.GetDecimal(5), //6: 5th column
                    Currency = reader.GetString(6), //9: 6th column 
                    TransitTime = reader.GetInt32(7), //9: 7th column
                    LastUpdatedBy = reader.GetInt32(8), //9: 8th column
                };
            }
            reader.Close();
            conn.Close();
            return sr;
        }
    }
}
