using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace NPParcelDeliveryServiceAssignment.Models
{
    public class ShippingCalculatorViewModel
    {
        [Required]
        [Display(Name = "From City")]
        [StringLength(50, ErrorMessage = "City is too long")]
        public string FromCity { get; set; }

        [Required]
        [Display(Name = "From Country")]
        [StringLength(50, ErrorMessage = "Country is too long")]
        public string FromCountry { get; set; }

        [Required(ErrorMessage = "Parcel to city destination is required.")]
        [Display(Name = "To City")]
        [StringLength(50, ErrorMessage = "City is too long")]
        public string ToCity { get; set; }

        [Required(ErrorMessage = "Parcel to country destination is required.")]
        [Display(Name = "To Country")]
        [StringLength(50, ErrorMessage = "Country is too long")]
        public string ToCountry { get; set; }

        [Required]
        [RegularExpression(@"^(?!0+(?:\.0+)?$)\d+(\.\d+)?$", ErrorMessage = "Invalid parcel weight, the weight should not be zero or contain any special character or letters.")]
        [Display(Name = "Parcel Weight (kg)")]
        public double ParcelWeight { get; set; }

        [Display(Name = "Delivery Charge")]
        public decimal DeliveryCharge { get; set; }
    }
}
