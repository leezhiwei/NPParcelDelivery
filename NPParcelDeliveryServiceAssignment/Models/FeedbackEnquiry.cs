using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace NPParcelDeliveryServiceAssignment.Models
{
    public class FeedbackEnquiry
    {
        [Display(Name = "Feedback Enquiry ID")]
        public int FeedbackEnquiryID { get; set; }
        [Display(Name = "Member ID")]
        public int MemberID { get; set; }

        public string Content { get; set; }
        [Display(Name = "Date Of Posting")]
        [DataType(DataType.Date)]
        public DateTime DateTimePosted { get; set; }
        [Display(Name = "Staff ID")]
        public int? StaffID { get; set; }

        public string Response { get; set; }

        public string Status { get; set; }

    }
}
