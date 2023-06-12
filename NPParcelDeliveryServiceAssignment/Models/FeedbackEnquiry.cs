namespace NPParcelDeliveryServiceAssignment.Models
{
    public class FeedbackEnquiry
    {
        public int FeedbackEnquiryID { get; set; }

        public int MemberID { get; set; }

        public string Content { get; set; }

        public DateTime DateTimePosted { get; set; }

        public int? StaffID { get; set; }

        public string Response { get; set; }

        public string Status { get; set; }

    }
}
