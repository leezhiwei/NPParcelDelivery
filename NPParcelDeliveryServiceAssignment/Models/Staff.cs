using System.ComponentModel.DataAnnotations;

namespace NPParcelDeliveryServiceAssignment.Models
{
    public class Staff
    {
        public int StaffID { get; set; }
        public string StaffName { get; set; }
        public string LoginID { get; set; }
        public string Password { get; set; }
        public string Appointment { get; set; }
        public string OfficeTelNo { get; set; }
        public string Location { get; set; }
    }
}

