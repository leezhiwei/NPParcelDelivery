using System.ComponentModel.DataAnnotations;

namespace NPParcelDeliveryServiceAssignment.Models
{
    public class PaymentTransaction
    {
        public int TransactionID { get; set; }

        [RegularExpression(@"^\d+$", ErrorMessage = "Please enter integer values and numbers.")]
        public int ParcelID { get; set; }

        public decimal AmtTran { get; set; }

        [StringLength(3, ErrorMessage = "Too long currency code")]
        public string Currency { get; set; }

        public string TranType { get; set; }

        public DateTime TranDate { get; set; }

    }
}
