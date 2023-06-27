using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace NPParcelDeliveryServiceAssignment.Models
{
    public class Parcel
    {
        [Display(Name = "Parcel ID")]
        public int ParcelID { get; set; }
        [Display(Name = "Item Description")]
        [StringLength(255, ErrorMessage = "Item description too long")]
        public string ItemDescription { get; set; }
        [Required]
        [Display(Name = "Sender Name")]
        [StringLength(50, ErrorMessage = "Sender name is too long")]
        public string SenderName { get; set; }
        [Required]
        [Display(Name = "Sender Phone Number")]
        [RegexStringValidator("/\\+65(6|8|9)\\d{7}/g")]
        [StringLength(20, ErrorMessage = "Invalid Phone Number")]
        public string SenderTelNo { get; set; }
        [Required]
        [Display(Name = "Receiver Name")]
        [StringLength(50, ErrorMessage = "Name is too long")]
        public string ReceiverName { get; set; }
        [Required]
        [Display(Name = "Receiver Phone Number")]
        [RegexStringValidator("/\\+65(6|8|9)\\d{7}/g")]
        [StringLength(20, ErrorMessage = "Invalid Phone Number")]
        public string ReceiverTelNo { get; set; }
        [Required]
        [Display(Name = "Delivery Address")]
        [StringLength(255, ErrorMessage = "Address too long")]
        public string DeliveryAddress { get; set; }
        [Required]
        [Display(Name = "From City")]
        [StringLength(50, ErrorMessage = "City is too long")]
        public string FromCity { get; set; }
        [Required]
        [Display(Name = "From Country")]
        [StringLength(50, ErrorMessage = "Country is too long")]
        public string FromCountry { get; set; }
        [Required]
        [Display(Name = "To City")]
        [StringLength(50, ErrorMessage = "City is too long")]
        public string ToCity { get; set; }
        [Required]
        [Display(Name = "To Country")]
        [StringLength(50, ErrorMessage = "Country is too long")]
        public string ToCountry { get; set; }
        [Required]
        [Display(Name = "Parcel Weight (kg)")]
        public double ParcelWeight { get; set; }
        [Required]
        [Display(Name = "Delivery Charge")]
        public decimal DeliveryCharge { get; set; }
        [Required]
        [StringLength(3, ErrorMessage = "Too long currency code")]
        public string Currency { get; set; }
        [Display(Name = "Target Delivery Date")]
        [DataType(DataType.Date)]
        public DateTime? TargetDeliveryDate { get; set; }
        [Required]
        [Display(Name = "Delivery Status")]
        [StringLength(1, ErrorMessage = "Invalid Status")]
        public string DeliveryStatus { get; set; }
        [Display(Name = "Delivery Man ID")]
        public int? DeliveryManID { get; set; }
    }
}
