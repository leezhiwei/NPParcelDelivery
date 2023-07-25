using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;

namespace NPParcelDeliveryServiceAssignment.Models
{
    public class PaymentTransaction
    {
        [Required]
        [Display(Name = "Transaction ID")]
        public int TransactionID { get; set; }

        [Required]
        [Display(Name = "Parcel ID")]
        public int ParcelID { get; set; }

        [Required]
        [RegularExpression(@"^\d+$", ErrorMessage = "Please enter numeric values for transaction amount.")]
        [Display(Name = "Transaction Amount")]
        public decimal AmtTran { get; set; }

        [Required]
        [Display(Name = "Currency")]
        [StringLength(3, ErrorMessage = "Too long currency code")]
        public string Currency { get; set; }

        [Required]
        [Display(Name = "Transaction Type")]
        [StringLength(4, ErrorMessage = "Transaction Type requires 4 characters")]
        public string TranType { get; set; }

        [Required]
        [Display(Name = "Transaction Date")]
        public DateTime TranDate { get; set; }

    }
}
