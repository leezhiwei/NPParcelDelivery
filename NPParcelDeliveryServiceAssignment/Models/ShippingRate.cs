using System.ComponentModel.DataAnnotations;

namespace NPParcelDeliveryServiceAssignment.Models
{
    public class ShippingRate
    {
        public int ShippingRateID { get; set; }
        public string FromCity { get; set; }
		public string FromCountry { get; set; }
		public string ToCity { get; set; }

		public string ToCountry { get; set; }

        public decimal ShipRate { get; set; }
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only please")]
        public string Currency { get; set; }

        public int TransitTime { get; set; }

        public int LastUpdatedBy { get; set; }

    }
}
