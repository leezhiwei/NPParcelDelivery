using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace NPParcelDeliveryServiceAssignment.Models
{
    public class Parcel
    {
        public int ParcelID { get; set; }
        [StringLength(255, ErrorMessage = "Item description too long")]
        public string ItemDescription { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Sender name is too long")]
        public string SenderName { get; set; }
        [Required]
        [RegexStringValidator("/\\+65(6|8|9)\\d{7}/g")]
        [StringLength(20, ErrorMessage = "Invalid Phone Number")]
        public string SenderTelNo { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Name is too long")]
        public string ReceiverName { get; set; }
        [Required]
        [RegexStringValidator("/\\+65(6|8|9)\\d{7}/g")]
        [StringLength(20, ErrorMessage = "Invalid Phone Number")]
        public string ReceiverTelNo { get; set; }
        [Required]
        [StringLength(255, ErrorMessage = "Address too long")]
        public string DeliveryAddress { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "City is too long")]
        public string FromCity { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Country is too long")]
        public string FromCountry { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "City is too long")]
        public string ToCity { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Country is too long")]
        public string ToCountry { get; set; }
        [Required]
        public double ParcelWeight { get; set; }
        [Required]
        public decimal DeliveryCharge { get; set; }
        [Required]
        [StringLength(3, ErrorMessage = "Too long currency code")]
        public string Currency { get; set; }

        public DateTime? TargetDeliveryDate { get; set; }
        [Required]
        [StringLength(1, ErrorMessage = "Invalid Status")]
        public string DeliveryStatus { get; set; }

        public int? DeliveryManID { get; set; }
    }
}
