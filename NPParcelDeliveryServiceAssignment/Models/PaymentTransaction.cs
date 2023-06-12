namespace NPParcelDeliveryServiceAssignment.Models
{
    public class PaymentTransaction
    {
        public int TransactionID { get; set; }

        public int ParcelID { get; set; }

        public decimal AmtTran { get; set; }

        public string Currency { get; set; }

        public string TranType { get; set; }

        public DateTime TranDate { get; set; }

    }
}
