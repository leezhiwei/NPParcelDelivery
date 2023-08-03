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
        public string? ItemDescription { get; set; }

        [Required(ErrorMessage = "Please enter a sender name.")]
        [Display(Name = "Sender Name")]
        [StringLength(50, ErrorMessage = "Sender name is too long")]
        public string SenderName { get; set; }

        [Required(ErrorMessage = "Please enter a valid phone number.")]
        [Display(Name = "Sender Phone Number")]
        // Previously used before JS method
        // [RegularExpression(@"^\+[0-9]{1,20}$", ErrorMessage = "Invalid phone number format. Please start with country code, E.g. +65 followed by the phone number.")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Invalid phone number. The phone number should be between 8 and 20 characters in length.")] // Minimum length is 10 due to country code(2 nums) + phone number(8 nums)
        public string SenderTelNo { get; set; }

        [Required(ErrorMessage = "Please enter a receiver name.")]
        [Display(Name = "Receiver Name")]
        [StringLength(50, ErrorMessage = "Name is too long")]
        public string ReceiverName { get; set; }

        [Required(ErrorMessage = "Please enter a receiver's phone number.")]
        [Display(Name = "Receiver Phone Number")]
        // Previously used before JS method
        // [RegularExpression(@"^\+[0-9]{1,20}$", ErrorMessage = "Invalid phone number format. Please start with country code, E.g. +65 followed by the phone number.")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Invalid phone number. The phone number should be between 8 and 20 characters in length.")] // Minimum length is 10 due to country code(2 nums) + phone number(8 nums)
        public string ReceiverTelNo { get; set; }

        [Required(ErrorMessage = "Please enter a delivery address.")]
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

        [Required(ErrorMessage = "Parcel to city destination is required.")]
        [Display(Name = "To City")]
        [StringLength(50, ErrorMessage = "City is too long")]
        public string ToCity { get; set; }

        [Required(ErrorMessage = "Parcel to country destination is required.")]
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

        [Required(ErrorMessage = "Delivery Status is required. <br> Values: <br> 0 - pending delivery <br> 1 - delivery to destination in progress <br> 2 - delivery to airport in progress <br> 3 - delivery completed <br> 4 - delivery failed")]
        [Display(Name = "Delivery Status")]
        [RegularExpression("^[0-4]{1}$", ErrorMessage = "Delivery Status accept values between 0 and 4.")] //Validates if value entered is within 0 - 4
        [StringLength(1, ErrorMessage = "Invalid Status. <br> Values: <br> 0 - pending delivery <br> 1 - delivery to destination in progress; <br> 2 - delivery to airport in progress; <br> 3 - delivery completed; <br> 4 - delivery failed")]
        public string DeliveryStatus { get; set; }

        [Display(Name = "Delivery Man ID")]
        public int? DeliveryManID { get; set; }

    }
}
