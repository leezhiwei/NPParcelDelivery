using System.ComponentModel.DataAnnotations;

namespace NPParcelDeliveryServiceAssignment.Models
{
    public class PaymentTransaction
    {
        [Display(Name = "Transaction ID")]
        public int TransactionID { get; set; }

        [Display(Name = "Parcel ID")]

        [RegularExpression(@"^\d+$", ErrorMessage = "Please enter integer values and numbers.")]
        public int ParcelID { get; set; }


        [Display(Name = "Transaction Amount")]
        public decimal AmtTran { get; set; }

        [Display(Name = "Currency")]

        [StringLength(3, ErrorMessage = "Too long currency code")]
        public string Currency { get; set; }

        [Display(Name = "Transaction Type")]
        public string TranType { get; set; }

        [Display(Name = "Transaction Date")]
        public DateTime TranDate { get; set; }

    }
}
