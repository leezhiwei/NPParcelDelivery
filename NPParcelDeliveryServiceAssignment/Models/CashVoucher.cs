using System.ComponentModel.DataAnnotations;

namespace NPParcelDeliveryServiceAssignment.Models
{
    public class CashVoucher
    {
        [Display(Name = "Cash Voucher ID")]
        public int CashVoucherID { get; set; }
        [Display(Name = "Staff ID")]
        public int StaffID { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }
        [Display(Name = "Issuing Code")]
        public string IssuingCode { get; set; }
        [Display(Name = "Receiver Name")]
        public string ReceiverName { get; set; }
        [Display(Name = "Receiver Telephone Number")]
        public string ReceiverTelNo { get; set; }
        [Display(Name = "Date Of Issue")]
        public DateTime DateTimeIssued { get; set; }

        public string Status { get; set; }

    }
}
