using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Humanizer;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NPParcelDeliveryServiceAssignment.DALs;

namespace NPParcelDeliveryServiceAssignment.Models
{
	public class ValidateShipRateInfoExists : ValidationAttribute
	{/*
		private ShippingRateDAL srd = new ShippingRateDAL();
		protected override ValidationResult IsValid(
		object value, ValidationContext validationContext)
		{
			// Get the info value to validate
			string fromCity = Convert.ToString(value);
			string fromCountry = Convert.ToString(value);
			string toCity = Convert.ToString(value);
			string toCountry = Convert.ToString(value);

			// Casting the validation context to the "Staff" model class
			ShippingRate shippingRate = (ShippingRate)validationContext.ObjectInstance;
			// Get the Staff Id from the staff instance
			int shippingRateID = shippingRate.ShippingRateID;
			if (srd.IsInfoExist(fromCity, fromCountry, toCity, toCountry, shippingRateID))
			{
				// validation failed
				return new ValidationResult
				("Shipping Rate Info already exists!");
			}
			else
				// validation passed 
				return ValidationResult.Success;
		}
		*/
	}
}