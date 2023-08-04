using Microsoft.CodeAnalysis.Diagnostics;
using System.ComponentModel.DataAnnotations;

namespace NPParcelDeliveryServiceAssignment.Models
{
    public class ShippingRate
    {
        [Display(Name = "Shipping Rate ID")]
        public int ShippingRateID { get; set; }
        [Display(Name = "From City")]
        public string FromCity { get; set; }
        [Display(Name = "From Country")]
		public string FromCountry { get; set; }
        [Display(Name = "To City")]
		public string ToCity { get; set; }
        [Display(Name = "To Country")]
		public string ToCountry { get; set; }
        [Display(Name = "Shipping Rate")]
        public decimal ShipRate { get; set; }
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        public string Currency { get; set; }
        [Display(Name = "Transit Time")]
        public int TransitTime { get; set; }
        [Display(Name = "Last Updated By")]
        public int LastUpdatedBy { get; set; }

    }
}
