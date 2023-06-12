namespace NPParcelDeliveryServiceAssignment.Models
{
    public class CashVoucher
    {
        public int CashVoucherID { get; set; }

        public int StaffID { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public string IssuingCode { get; set; }

        public string ReceiverName { get; set; }

        public string ReceiverTelNo { get; set; }

        public DateTime DateTimeIssued { get; set; }

        public string Status { get; set; }

    }
}
