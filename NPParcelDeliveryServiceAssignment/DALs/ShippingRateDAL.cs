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
            return shippingRateList;
        }

        public int Add(ShippingRate shippingRate)
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify an INSERT SQL statement which will
            //return the auto-generated StaffID after insertion
            cmd.CommandText = @"INSERT INTO ShippingRate ( ShippingRateID, FromCity, FromCountry, ToCity, 
                                ToCountry, ShipRate, Currency, TransitTime, LastUpdatedBy) 
                                                    OUTPUT INSERTED.ShippingRateID 
                                                    VALUES(@shippingRateID, @fromCity, @fromCountry,@toCity, 
                                                   @toCountry, @shipRate, @currency, @transitTime,@lastUpdatedBy)";
            //Define the parameters used in SQL statement, value for each parameter
            //is retrieved from respective class's property.
            cmd.Parameters.AddWithValue("@shippingRateID", shippingRate.ShippingRateID);
            cmd.Parameters.AddWithValue("@fromCity", shippingRate.FromCity);
            cmd.Parameters.AddWithValue("@fromCountry", shippingRate.FromCountry);
            cmd.Parameters.AddWithValue("@toCity", shippingRate.ToCity);
            cmd.Parameters.AddWithValue("@toCountry", shippingRate.ToCountry);
            cmd.Parameters.AddWithValue("@shipRate", shippingRate.ShipRate);
            cmd.Parameters.AddWithValue("@currency", shippingRate.Currency);
            cmd.Parameters.AddWithValue("@transitTime", shippingRate.TransitTime);
            cmd.Parameters.AddWithValue("@lastUpdatedBy", shippingRate.LastUpdatedBy);
            //A connection to database must be opened before any operations made.
            conn.Open();
            //ExecuteScalar is used to retrieve the auto-generated
            //shipping rate ID after executing the INSERT SQL statement
            shippingRate.ShippingRateID = (int)cmd.ExecuteScalar();
            //A connection should be closed after operations.
            conn.Close();
            //Return id when no error occurs.
            return shippingRate.ShippingRateID;
        }

        public int Update(ShippingRate shippingRate)
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify an UPDATE SQL statement
            cmd.CommandText = @"UPDATE ShippingRate SET ShipRate=@shipRate, 
                                TransitTime=@transitTime
                                WHERE ShippingRateID = @shippingRateID";
            //Define the parameters used in SQL statement, value for each parameter
            //is retrieved from respective class's property.
            cmd.Parameters.AddWithValue("@shipRate", shippingRate.ShipRate);
            cmd.Parameters.AddWithValue("@transitTime", shippingRate.TransitTime);
            cmd.Parameters.AddWithValue("@shippingRateID", shippingRate.ShippingRateID);
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
